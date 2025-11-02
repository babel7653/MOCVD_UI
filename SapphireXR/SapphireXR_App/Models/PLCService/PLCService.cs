using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Windows.Threading;
using TwinCAT.Ads;
using TwinCAT.PlcOpen;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        static PLCService()
        {
            IntializePubSub();
        }

        public static bool Connect()
        {
            if (Ads.IsConnected == true && Ads.ReadState().AdsState == AdsState.Run)
            {
                return true;
            }
          
            DateTime startTime = DateTime.Now;
            while (true)
            {
                if(TryConnect() == true)
                {
                    return true;
                }
                else
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds < AppSetting.ConnectionRetryMilleseconds)
                    {
                        continue;
                    }
                    else
                    {
                        Connected = PLCConnection.Disconnected;
                        return false;
                    }
                }
            }
        }

        private static bool TryConnect()
        {
            try
            {
                if (AppSetting.PLCAddress != "Local")
                {
                    Ads.Connect(new AmsAddress(AppSetting.PLCAddress + ":" + AppSetting.PLCPort));
                }
                else
                {
                    Ads.Connect(AmsNetId.Local, AppSetting.PLCPort);
                }

                if (Ads.IsConnected == true && Ads.ReadState().AdsState == AdsState.Run)
                {
                    CreateHandle();
                    ReadInitialStateValueFromPLC();

                    Connected = PLCConnection.Connected;
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void OnConnected()
        {
            connectionTryTimer?.Stop();
            TryConnectAsync = null;

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(2000000);
                timer.Tick += ReadStateFromPLC;
            }
            timer.Start();
            if (currentActiveRecipeListener == null)
            {
                currentActiveRecipeListener = new DispatcherTimer();
                currentActiveRecipeListener.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 500);
                currentActiveRecipeListener.Tick += (object? sender, EventArgs e) =>
                {
                    try
                    {
                        dCurrentActiveRecipeIssue?.Publish(Ads.ReadAny<short>(hRcpStepN));
                        dRecipeControlPauseTimeIssuer?.Publish(Ads.ReadAny<TIME>(hRecipeControlPauseTime).Time.Seconds);
                        RecipeRunET recipeRunET = Ads.ReadAny<RecipeRunET>(hRecipeRunET);
                        dRecipeRunElapsedTimeIssuer?.Publish((recipeRunET.ElapsedTime / 1000, recipeRunET.Mode));
                        if (RecipeRunEndNotified == false && Ads.ReadAny<short>(hCmd_RcpOperation) == 50)
                        {
                            dRecipeEndedPublisher?.Publish(true);
                            RecipeRunEndNotified = true;
                        }
                        else
                            if (RecipeRunEndNotified == true && Ads.ReadAny<short>(hCmd_RcpOperation) == 0)
                        {
                            RecipeRunEndNotified = false;
                        }
                    }
                    catch (Exception)
                    {
                        Connected = PLCConnection.Disconnected;
                    }
                };
            }
            currentActiveRecipeListener.Start();
        }

        
        private static void OnDisconnected()
        {
            timer?.Stop();
            currentActiveRecipeListener?.Stop();

            if (connectionTryTimer == null)
            { 
                connectionTryTimer = new DispatcherTimer();
                connectionTryTimer.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond);
                connectionTryTimer.Tick += (object? sender, EventArgs e) =>
                {
                    if(TryConnectAsync == null || (TryConnectAsync.IsCompleted == true && TryConnectAsync.Result == false))
                    {
                        TryConnectAsync = Task.Delay(1000).ContinueWith((task) => TryConnect(), TaskScheduler.FromCurrentSynchronizationContext());
                    }
                };
            }
            connectionTryTimer.Start();
        }

        private static void CreateHandle()
        {
            hDeviceControlValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_CV");
            hDeviceCurrentValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_PV");

            hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
            hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");

            hMonitoring_PV = Ads.CreateVariableHandle("GVL_IO.aMonitoring_PV");
            hInputState = Ads.CreateVariableHandle("GVL_IO.aInputState");
            hInputState4 = Ads.CreateVariableHandle("GVL_IO.aInputState[4]");
            hDigitalOutput = Ads.CreateVariableHandle("GVL_IO.aDigitalOutputIO");
            hDigitalOutput2 = Ads.CreateVariableHandle("GVL_IO.aDigitalOutputIO[2]");
            hOutputCmd = Ads.CreateVariableHandle("GVL_IO.aOutputCmd");
            hOutputCmd1 = Ads.CreateVariableHandle("GVL_IO.aOutputCmd[1]");
            hOutputCmd2 = Ads.CreateVariableHandle("GVL_IO.aOutputCmd[2]");
            for(uint arrayIndex = 0; arrayIndex < NumAlarmWarningArraySize; arrayIndex++)
            {
                hInterlockEnable[arrayIndex] = Ads.CreateVariableHandle("GVL_IO.aInterlockEnable[" + (arrayIndex + 1) + "]");
            }
            for (uint arrayIndex = 0; arrayIndex < NumInterlockSet; arrayIndex++)
            {
                hInterlockset[arrayIndex] = Ads.CreateVariableHandle("GVL_IO.aInterlockSet[" + (arrayIndex + 1) + "]");
            }
            for (uint arrayIndex = 0; arrayIndex < NumInterlock; ++arrayIndex)
            {
                hInterlock[arrayIndex] = Ads.CreateVariableHandle("GVL_IO.aInterlock[" + (arrayIndex + 1) + "]");
            }

            hRcp = Ads.CreateVariableHandle("RCP.aRecipe");
            hRcpTotalStep = Ads.CreateVariableHandle("RCP.iRcpTotalStep");
            hCmd_RcpOperation = Ads.CreateVariableHandle("RCP.cmd_RcpOperation");
            hRcpStepN = Ads.CreateVariableHandle("RCP.iRcpStepN");
            hTemperaturePV = Ads.CreateVariableHandle("GVL_IO.aLineHeater_rTemperaturePV");
            hControlModeCmd = Ads.CreateVariableHandle("MAIN.controlModeCmd");
            hControlMode = Ads.CreateVariableHandle("MAIN.controlMode");
            hUserState = Ads.CreateVariableHandle("RCP.userState");
            hRecipeControlPauseTime = Ads.CreateVariableHandle("RCP.Pause_ET");
            hRecipeRunET = Ads.CreateVariableHandle("RCP.RecipeRunET");
            hE3508InputManAuto = Ads.CreateVariableHandle("GVL_IO.nE3508_nInputManAutoBytes");
            hOutputSetType = Ads.CreateVariableHandle("GVL_IO.nIQPLUS_SetType");
            hOutputMode = Ads.CreateVariableHandle("GVL_IO.nIQPLUS_Mode");
            for (uint analogDevice = 0; analogDevice < NumControllers; ++analogDevice)
            {
                hAControllerInput[analogDevice] = Ads.CreateVariableHandle("GVL_IO.aController[" + (analogDevice + 1)+ "].input");
            }
            hUIInterockCheckRecipeEnable = Ads.CreateVariableHandle("GVL_IO.nUIInterockCheckRecipeEnable");
            hUIInterockCheckOpenReactor = Ads.CreateVariableHandle("GVL_IO.nUIInterockCheckOpenReactor");
        }

        private static void IntializePubSub()
        {
            dCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.Publisher>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".CurrentValue"));
            }
            dControlValueIssuers = new Dictionary<string, ObservableManager<float>.Publisher>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".ControlValue"));
            }
            dControlCurrentValueIssuers = new Dictionary<string, ObservableManager<(float, float)>.Publisher>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlCurrentValueIssuers.Add(kv.Key, ObservableManager<(float, float)>.Get("FlowControl." + kv.Key + ".ControlTargetValue.CurrentPLCState"));
            }
            aMonitoringCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.Publisher>();
            foreach (KeyValuePair<string, int> kv in dMonitoringMeterIndex)
            {
                aMonitoringCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("MonitoringPresentValue." + kv.Key + ".CurrentValue"));
            }
            dValveStateIssuers = new Dictionary<string, ObservableManager<bool>.Publisher>();
            foreach ((string valveID, int valveIndex) in ValveIDtoOutputSolValveIdx1)
            {
                dValveStateIssuers.Add(valveID, ObservableManager<bool>.Get("Valve.OnOff." + valveID + ".CurrentPLCState"));
            }
            foreach ((string valveID, int valveIndex) in ValveIDtoOutputSolValveIdx2)
            {
                dValveStateIssuers.Add(valveID, ObservableManager<bool>.Get("Valve.OnOff." + valveID + ".CurrentPLCState"));
            }
            dCurrentActiveRecipeIssue = ObservableManager<short>.Get("RecipeRun.CurrentActiveRecipe");
            baHardWiringInterlockStateIssuers = ObservableManager<BitArray>.Get("HardWiringInterlockState");
            dIOStateList = ObservableManager<BitArray>.Get("DeviceIOList");
            dRecipeEndedPublisher = ObservableManager<bool>.Get("RecipeEnded");
            dLineHeaterTemperatureIssuers = ObservableManager<float[]>.Get("LineHeaterTemperature");
            dRecipeControlPauseTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Pause");
            dRecipeRunElapsedTimeIssuer = ObservableManager<(int, RecipeRunETMode)>.Get("RecipeRun.ElapsedTime");
            dDigitalOutput2 = ObservableManager<BitArray>.Get("DigitalOutput2");
            dDigitalOutput3 = ObservableManager<BitArray>.Get("DigitalOutput3");
            dOutputCmd1 = ObservableManager<BitArray>.Get("OutputCmd1");
            dInputManAuto = ObservableManager<BitArray>.Get("InputManAuto");
            dThrottleValveControlMode = ObservableManager<short>.Get("ThrottleValveControlMode");
            dPressureControlModeIssuer = ObservableManager<ushort>.Get("PressureControlMode");
            dThrottleValveStatusIssuer = ObservableManager<short>.Get("ThrottleValveStatus");
            dLogicalInterlockStateIssuer = ObservableManager<BitArray>.Get("LogicalInterlockState");
            dPLCConnectionPublisher = ObservableManager<PLCConnection>.Get("PLCService.Connected");
            dControlModeChangingPublisher = ObservableManager<ControlMode>.Get("ControlModeChanging");
            ObservableManager<bool>.Subscribe("Leak Test Mode", leakTestModeSubscriber = new LeakTestModeSubscriber());
            temperatureTVPublisher = ObservableManager<short>.Get("TemperatureTV");
            pressureTVPublisher = ObservableManager<short>.Get("PressureTV");
            rotationTVPublisher = ObservableManager<short>.Get("RotationTV");
            recipeEnableSubConditionPublisher = ObservableManager<BitArray>.Get("RecipeEnableSubCondition");
            reactorEnableSubConditionPublisher = ObservableManager<BitArray>.Get("ReactorEnableSubCondition");
        }

        private static (BitArray, int, uint) GetBuffer(string valveID)
        {
            int index = -1;
            if (ValveIDtoOutputSolValveIdx1.TryGetValue(valveID, out index) == true)
            {
                if (baReadValveStatePLC1 == null)
                {
                    throw new ReadValveStateException("PLC Service: BaReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (baReadValveStatePLC1, index, hReadValveStatePLC1);
                }
            }
            else
                if (ValveIDtoOutputSolValveIdx2.TryGetValue(valveID, out index) == true)
            {
                if (baReadValveStatePLC2 == null)
                {
                    throw new ReadValveStateException("PLC Service: baReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (baReadValveStatePLC2, index, hReadValveStatePLC2);
                }
            }
            else
            {
                throw new ReadValveStateException("PLC Service: non-exsiting valve ID entered to GetReadValveStateBuffer()");
            }
        }

        public static void AddPLCStateUpdateTask(Action task)
        {
            AddOnPLCStateUpdateTask.Add(task);
        }

        public static void RemovePLCStateUpdateTask(Action task)
        {
            AddOnPLCStateUpdateTask.Remove(task);
        }
    }
}
