﻿using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Windows;
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
            //Read Set Value from PLC 
            hDeviceControlValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_CV");
            //Read Present Value from Device of PLC
            hDeviceCurrentValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_PV");
            //Read and Write Max Value of PLC 
            hDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValueController");

            hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
            hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");
            hWriteDeviceTargetValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_TV");
            hWriteDeviceRampTimePLC = Ads.CreateVariableHandle("GVL_IO.aController_RampTime");

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
            hOperationMode = Ads.CreateVariableHandle("MAIN.bOperationMode");
            hUserState = Ads.CreateVariableHandle("RCP.userState");
            hRecipeControlPauseTime = Ads.CreateVariableHandle("RCP.Pause_ET");
            hRecipeRunET = Ads.CreateVariableHandle("RCP.RecipeRunET");
            hE3508InputManAuto = Ads.CreateVariableHandle("GVL_IO.nE3508_nInputManAutoBytes");
            hOutputSetType = Ads.CreateVariableHandle("GVL_IO.nIQPLUS_SetType");
            hOutputMode = Ads.CreateVariableHandle("GVL_IO.nIQPLUS_Mode");
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
            dTargetValueIssuers = new Dictionary<string, ObservableManager<float>.Publisher>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dTargetValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".TargetValue"));
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
            dOperationModeChangingPublisher = ObservableManager<bool>.Get("OperationModeChanging");

            ObservableManager<bool>.Subscribe("Leak Test Mode", leakTestModeSubscriber = new LeakTestModeSubscriber());
        }

        private static void ReadStateFromPLC(object? sender, EventArgs e)
        {
            try
            {
                ReadCurrentValueFromPLC();
                if (aDeviceControlValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlValueIssuers?[kv.Key].Publish(aDeviceControlValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dCurrentValueIssuers?[kv.Key].Publish(aDeviceCurrentValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceTargetValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dTargetValueIssuers?[kv.Key].Publish(aDeviceTargetValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceControlValues != null && aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlCurrentValueIssuers?[kv.Key].Publish((aDeviceCurrentValues[dIndexController[kv.Key]], aDeviceControlValues[dIndexController[kv.Key]]));
                    }
                }

                if (aMonitoring_PVs != null)
                {
                    foreach (KeyValuePair<string, int> kv in dMonitoringMeterIndex)
                    {
                        aMonitoringCurrentValueIssuers?[kv.Key].Publish(aMonitoring_PVs[kv.Value]);
                    }
                }

                if (aInputState != null)
                {
                    short value = aInputState[0];
                    baHardWiringInterlockStateIssuers?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray()));
                    dThrottleValveStatusIssuer?.Publish(aInputState[4]);

                    bool[] ioList = new bool[64];
                    for (int inputState = 1; inputState < aInputState.Length; ++inputState)
                    {
                        new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(aInputState[inputState]) : BitConverter.GetBytes(aInputState[inputState]).Reverse().ToArray()).CopyTo(ioList, (inputState - 1) * sizeof(short) * 8);
                    }
                    dIOStateList?.Publish(new BitArray(ioList));
                }

                if (baReadValveStatePLC1 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx1)
                    {
                        dValveStateIssuers?[valveID].Publish(baReadValveStatePLC1[index]);
                    }
                }
                if (baReadValveStatePLC2 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx2)
                    {
                        dValveStateIssuers?[valveID].Publish(baReadValveStatePLC2[index]);
                    }
                }
                dLineHeaterTemperatureIssuers?.Publish(Ads.ReadAny<float[]>(hTemperaturePV, [(int)LineHeaterTemperature]));

                byte[] digitalOutput = Ads.ReadAny<byte[]>(hDigitalOutput, [4]);
                dDigitalOutput2?.Publish(new BitArray(new byte[1] { digitalOutput[1] }));
                dDigitalOutput3?.Publish(new BitArray(new byte[1] { digitalOutput[2] }));
                short[] outputCmd = Ads.ReadAny<short[]>(hOutputCmd, [3]);
                dOutputCmd1?.Publish(bOutputCmd1 = new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd[0]) : BitConverter.GetBytes(outputCmd[0]).Reverse().ToArray()));
                dThrottleValveControlMode?.Publish(outputCmd[1]);
                ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
                dInputManAuto?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray()));
                dPressureControlModeIssuer?.Publish(Ads.ReadAny<ushort>(hOutputSetType));

                int iterlock1 = Ads.ReadAny<int>(hInterlock[0]);
                dLogicalInterlockStateIssuer?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(iterlock1) : BitConverter.GetBytes(iterlock1).Reverse().ToArray()));
            
                string exceptionStr = string.Empty;
                if (aDeviceControlValues == null)
                {
                    exceptionStr += "aDeviceControlValues is null in OnTick PLCService";
                }
                if (aDeviceCurrentValues == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aDeviceCurrentValues is null in OnTick PLCService";
                }
                if (aDeviceTargetValues == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aDeviceTargetValues is null in OnTick PLCService";
                }
                if (aMonitoring_PVs == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aMonitoring_PVs is null in OnTick PLCService";
                }
                if (baReadValveStatePLC1 == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "baReadValveStatePLC1 is null in OnTick PLCService";
                }
                if (baReadValveStatePLC2 == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "baReadValveStatePLC2 is null in OnTick PLCService";
                }
                if (exceptionStr != string.Empty)
                {
                    throw new ReadBufferException(exceptionStr);
                }
            }
            catch(ReadBufferException exception)
            {
                if (ShowMessageOnOnTick == true)
                {
                    ShowMessageOnOnTick = MessageBox.Show("PLC로부터 상태 (Analog Device Control/Valve 상태)를 읽어오는데 실패했습니다. 이 메시지를 다시 표시하지 않으려면 Yes를 클릭하세요. 원인은 다음과 같습니다: " + exception.Message, "",
                        MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes ? false : true;
                }
            }
            catch (Exception)
            {
                Connected = PLCConnection.Disconnected;
            }
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
    }
}
