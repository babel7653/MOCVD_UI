using SapphireXR_App.Common;
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
            ConnectedNotifier = ObservableManager<PLCConnection>.Get("PLCService.Connected");
            Ads = new AdsClient();
            try
            {
                Connect();
            }
            catch (Exception)
            {
                MessageBox.Show("TwinCAT이 연결되지 않았습니다.");
                AddressPLC = "PLC Address : ";
                ModePLC = "System Mode : Not Connected";

                throw;
            }
        }

        public static void Connect()
        {
            Ads.Connect(new AmsAddress("5.62.179.198.1.1:851"));
            if (Ads.IsConnected == true)
            {
                AddressPLC = $"PLC Address : {Ads.Address}";
                ModePLC = "System Mode : Ready";
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
                hInterlock1 = Ads.CreateVariableHandle("GVL_IO.aInterlock[1]");

                hRcp = Ads.CreateVariableHandle("RCP.aRecipe");
                hRcpTotalStep = Ads.CreateVariableHandle("RCP.iRcpTotalStep");
                hCmd_RcpOperation = Ads.CreateVariableHandle("RCP.cmd_RcpOperation");
                hState_RcpOperation = Ads.CreateVariableHandle("RCP.state_RcpOperation");
                hRcpStepN =Ads.CreateVariableHandle("P50_RecipeControl.nRcpIndex");
                hTemperaturePV = Ads.CreateVariableHandle("P13_LineHeater.rTemperaturePV");
                hOperationMode = Ads.CreateVariableHandle("MAIN.bOperationMode");
                hUserState = Ads.CreateVariableHandle("RCP.userState");
                hRecipeControlHoldTime = Ads.CreateVariableHandle("P50_RecipeControl.Hold_ET");
                hRecipeControlRampTime = Ads.CreateVariableHandle("P50_RecipeControl.Ramp_ET");
                hRecipeControlPauseTime = Ads.CreateVariableHandle("P50_RecipeControl.Pause_ET");
                hE3508InputManAuto = Ads.CreateVariableHandle("P11_E3508.nInputManAutoBytes");
                hOutputSetType = Ads.CreateVariableHandle("P12_IQ_PLUS.nOutputSetType");
                hOutputMode = Ads.CreateVariableHandle("P12_IQ_PLUS.nOutputMode"); 

                aDeviceRampTimes = new short[dIndexController.Count];
                aDeviceTargetValues = new float[dIndexController.Count];

                ConnectedNotifier.Issue(PLCConnection.Connecrted);
            }
            else
            {
                throw new TwinCAT.ClientNotConnectedException(null);
            }
        }

        public static void ReadValveStateFromPLC()
        {
            // Solenoid Valve State Read(Update)
            try
            {
                uint aReadValveStatePLC1 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array
                uint aReadValveStatePLC2 = (uint)Ads.ReadAny(hReadValveStatePLC2, typeof(uint)); // Convert to Array
                
                baReadValveStatePLC1 = new BitArray(new int[] { (int)aReadValveStatePLC1 });
                baReadValveStatePLC2 = new BitArray(new int[] { (int)aReadValveStatePLC2 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ReadInitialStateValueFromPLC()
        {
            ReadValveStateFromPLC();
            ReadMaxValueFromPLC();
            ReadCurrentValueFromPLC();

            dCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".CurrentValue"));
            }
            dControlValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".ControlValue"));
            }
            dTargetValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dTargetValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".TargetValue"));
            }
            dControlCurrentValueIssuers = new Dictionary<string, ObservableManager<(float, float)>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlCurrentValueIssuers.Add(kv.Key, ObservableManager<(float, float)>.Get("FlowControl." + kv.Key + ".ControlTargetValue.CurrentPLCState"));
            }
            aMonitoringCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
            foreach(KeyValuePair<string, int> kv in dMonitoringMeterIndex)
            {
                aMonitoringCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("MonitoringPresentValue." + kv.Key + ".CurrentValue"));
            }
            dValveStateIssuers = new Dictionary<string, ObservableManager<bool>.DataIssuer>();
            foreach((string valveID, int valveIndex) in ValveIDtoOutputSolValveIdx1)
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
            dRecipeControlHoldTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Hold");
            dRecipeControlPauseTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Pause");
            dRecipeControlRampTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Ramp");
            dDigitalOutput2 = ObservableManager<BitArray>.Get("DigitalOutput2");
            dDigitalOutput3 = ObservableManager<BitArray>.Get("DigitalOutput3");
            dOutputCmd1 = ObservableManager<BitArray>.Get("OutputCmd1");
            dInputManAuto = ObservableManager<BitArray>.Get("InputManAuto");
            dThrottleValveControlMode = ObservableManager<short>.Get("ThrottleValveControlMode");
            dPressureControlModeIssuer = ObservableManager<ushort>.Get("PressureControlMode");
            dThrottleValveStatusIssuer = ObservableManager<short>.Get("ThrottleValveStatus");
            dLogicalInterlockStateIssuer = ObservableManager<BitArray>.Get("LogicalInterlockState");

            ObservableManager<bool>.Subscribe("Leak Test Mode", leakTestModeSubscriber = new LeakTestModeSubscriber());

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(2000000);
            timer.Tick += OnTick;
            timer.Start();

            currentActiveRecipeListener = new DispatcherTimer();
            currentActiveRecipeListener.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 500);
            currentActiveRecipeListener.Tick += (object? sender, EventArgs e) => {
                dCurrentActiveRecipeIssue.Issue(Ads.ReadAny<short>(hRcpStepN));
                if(RecipeRunEndNotified == false && Ads.ReadAny<short>(hCmd_RcpOperation) == 50)
                {
                    dRecipeEndedPublisher.Issue(true);
                    RecipeRunEndNotified = true;
                }
                else
                    if(RecipeRunEndNotified == true && Ads.ReadAny<short>(hCmd_RcpOperation) == 0)
                    {
                        RecipeRunEndNotified = false;
                    }
            };
            currentActiveRecipeListener.Start();
        }

        public static void ReadMaxValueFromPLC()
        {
            aDeviceMaxValue = Ads.ReadAny<float[]>(hDeviceMaxValuePLC, [dIndexController.Count]);
        }

        public static float ReadMaxValue(string flowControlID)
        {
            if(aDeviceMaxValue == null)
            {
                throw new Exception("aDeviceMaxValue is null in ReadMaxValue(), WriteDeviceMaxValue() must be called before");
            }
            return aDeviceMaxValue[dIndexController[flowControlID]];
        }

        private static void OnTick(object? sender, EventArgs e)
        {
            ReadCurrentValueFromPLC();
            if (aDeviceControlValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dControlValueIssuers?[kv.Key].Issue(aDeviceControlValues[dIndexController[kv.Key]]);
                }
            }
            if (aDeviceCurrentValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dCurrentValueIssuers?[kv.Key].Issue(aDeviceCurrentValues[dIndexController[kv.Key]]);
                }
            }
            if(aDeviceTargetValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dTargetValueIssuers?[kv.Key].Issue(aDeviceTargetValues[dIndexController[kv.Key]]);
                }
            }
            if(aDeviceControlValues != null && aDeviceCurrentValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dControlCurrentValueIssuers?[kv.Key].Issue((aDeviceCurrentValues[dIndexController[kv.Key]],aDeviceControlValues[dIndexController[kv.Key]]));
                }
            }

            if(aMonitoring_PVs != null)
            {
                foreach(KeyValuePair<string, int> kv in dMonitoringMeterIndex)
                {
                    aMonitoringCurrentValueIssuers?[kv.Key].Issue(aMonitoring_PVs[kv.Value]);
                }
            }

            if(aInputState != null)
            {
                short value = aInputState[0];
                baHardWiringInterlockStateIssuers?.Issue(new BitArray(BitConverter.IsLittleEndian == true? BitConverter.GetBytes(value) :BitConverter.GetBytes(value).Reverse().ToArray()));
                dThrottleValveStatusIssuer?.Issue(aInputState[4]);

                bool[] ioList = new bool[64];
                for(int inputState = 1; inputState < aInputState.Length; ++inputState)
                {
                    new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(aInputState[inputState]) : BitConverter.GetBytes(aInputState[inputState]).Reverse().ToArray()).CopyTo(ioList, (inputState - 1) * sizeof(short) * 8);
                }
                dIOStateList?.Issue(new BitArray(ioList));
            }

            if(baReadValveStatePLC1 != null)
            {
                foreach((string valveID, int index) in ValveIDtoOutputSolValveIdx1)
                {
                    dValveStateIssuers?[valveID].Issue(baReadValveStatePLC1[index]);
                }
            }
            if (baReadValveStatePLC2 != null)
            {
                foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx2)
                {
                    dValveStateIssuers?[valveID].Issue(baReadValveStatePLC2[index]);
                }
            }
            dLineHeaterTemperatureIssuers?.Issue(Ads.ReadAny<float[]>(hTemperaturePV, [(int)LineHeaterTemperature]));

            byte[] digitalOutput = Ads.ReadAny<byte[]>(hDigitalOutput, [4]);
            dDigitalOutput2?.Issue(new BitArray(new byte[1] { digitalOutput[1] }));
            dDigitalOutput3?.Issue(new BitArray(new byte[1] { digitalOutput[2] }));
            short[] outputCmd = Ads.ReadAny<short[]>(hOutputCmd, [3]);
            dOutputCmd1?.Issue(bOutputCmd1 = new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd[0]) : BitConverter.GetBytes(outputCmd[0]).Reverse().ToArray()));
            dThrottleValveControlMode?.Issue(outputCmd[1]);
            ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
            dInputManAuto?.Issue(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray()));
            dPressureControlModeIssuer?.Issue(Ads.ReadAny<ushort>(hOutputSetType));
            dRecipeControlHoldTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlHoldTime).Time.Seconds);
            dRecipeControlRampTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlRampTime).Time.Seconds);
            dRecipeControlPauseTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlPauseTime).Time.Seconds);
            short iterlock1 = Ads.ReadAny<short>(hInterlock1);
            dLogicalInterlockStateIssuer?.Issue(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(iterlock1) : BitConverter.GetBytes(iterlock1).Reverse().ToArray()));

            string exceptionStr = string.Empty;
            if(aDeviceControlValues == null)
            {
                exceptionStr += "aDeviceControlValues is null in OnTick PLCService";
            }
            if(aDeviceCurrentValues == null)
            {
                if(exceptionStr != string.Empty)
                {
                    exceptionStr += "\r\n";
                }
                exceptionStr += "aDeviceCurrentValues is null in OnTick PLCService";
            }
            if(aDeviceTargetValues == null)
            {
                if (exceptionStr != string.Empty)
                {
                    exceptionStr += "\r\n";
                }
                exceptionStr += "aDeviceTargetValues is null in OnTick PLCService";
            }
            if(aMonitoring_PVs == null)
            {
                if (exceptionStr != string.Empty)
                {
                    exceptionStr += "\r\n";
                }
                exceptionStr += "aMonitoring_PVs is null in OnTick PLCService";
            }
            if(baReadValveStatePLC1 == null)
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
                throw new Exception(exceptionStr);
            }
        }

        private static void ReadCurrentValueFromPLC()
        {
            aDeviceCurrentValues = Ads.ReadAny<float[]>(hDeviceCurrentValuePLC, [NumControllers]);
            aDeviceControlValues = Ads.ReadAny<float[]>(hDeviceControlValuePLC, [NumControllers]);
            aDeviceTargetValues = Ads.ReadAny<float[]>(hWriteDeviceTargetValuePLC, [NumControllers]);
            aMonitoring_PVs = Ads.ReadAny<float[]>(hMonitoring_PV, [18]);
            aInputState = Ads.ReadAny<short[]>(hInputState, [5]);
            ReadValveStateFromPLC();
        }

        public static float ReadCurrentValue(string controllerID)
        {
            if (aDeviceCurrentValues != null)
            {
                return aDeviceCurrentValues[dIndexController[controllerID]];
            }
            else
            {
                return float.NaN;
            }
        }

        public static bool ReadValveState(string valveID)
        {
            (BitArray buffer, int index, uint variableHandle) = GetBuffer(valveID);
            return buffer[index];
        }

        private static void DoWriteValveState(string valveID, bool onOff)
        {
            (BitArray buffer, int index, uint variableHandle) = GetBuffer(valveID);
            buffer[index] = onOff;

            uint[] sentBuffer = new uint[1];
            buffer.CopyTo(sentBuffer, 0);
            Ads.WriteAny(variableHandle, sentBuffer);
        }

        public static void AddCoupledValves(string leftValveID, string rightValveID)
        {
            LeftCoupled[rightValveID] = leftValveID;
            RightCoupled[leftValveID] = rightValveID;
        }

        public static void WriteValveState(string valveID, bool onOff)
        {
            if (LeakTestMode == false)
            {
                string? coupled = null;
                if (RightCoupled.TryGetValue(valveID, out coupled) == true)
                {
                    DoWriteValveState(coupled, onOff);
                }
            }
            else
            {
                string? coupled = null;
                if (LeftCoupled.TryGetValue(valveID, out coupled) == true)
                {
                    if(onOff == true)
                    {
                        DoWriteValveState(coupled, onOff);
                    }
                }
                else
                    if (RightCoupled.TryGetValue(valveID, out coupled) == true)
                    {
                        if(onOff == false)
                        {
                            DoWriteValveState(coupled, onOff);
                        }
                    }
            }
            DoWriteValveState(valveID, onOff);
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

        public static void WriteDeviceMaxValue(List<AnalogDeviceIO>? analogDeviceIOs)
        {
            // Device Max. Value Write
            try
            {
                if (analogDeviceIOs == null)
                {
                    throw new Exception("AnalogDeviceIO is null in WriteDeviceMaxValue");
                }

                float[] maxValue = new float[29];
                int index = 0;
                foreach (AnalogDeviceIO entry in analogDeviceIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for AnalogDeviceIO");
                    }
                    if (index < 3)
                    {
                        maxValue[index + 26] = entry.MaxValue;
                    }
                    else
                    {
                        maxValue[index - 3] = entry.MaxValue;
                    }
                        index++;
                }
                Ads.WriteAny(hDeviceMaxValuePLC, maxValue, [dIndexController.Count]);
                // List Analog Device Input / Output
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void WriteTargetValue(string flowControllerID, int targetValue)
        {
            aDeviceTargetValues![dIndexController[flowControllerID]] = (float)targetValue;
            Ads.WriteAny(hWriteDeviceTargetValuePLC, aDeviceTargetValues!, [aDeviceTargetValues!.Length]);
        }

        public static void WriteRampTime(string flowControllerID, short currentValue)
        {
            aDeviceRampTimes![dIndexController[flowControllerID]] = currentValue;
            Ads.WriteAny(hWriteDeviceRampTimePLC, aDeviceRampTimes!, [aDeviceRampTimes!.Length]);
        }

        public static void WriteRecipe(PlcRecipe[] recipe)
        {
            Ads.WriteAny(hRcp, recipe, [recipe.Length]);
        }

        public static void RefreshRecipe(PlcRecipe[] updates)
        {
            foreach(PlcRecipe recipe in updates)
            {
                 Ads.WriteAny(Ads.CreateVariableHandle("RCP.aRecipe[" + recipe.aRecipeShort[0] + "]"), recipe);
            }
        }

        public static void WriteTotalStep(short totalStep)
        {
           Ads.WriteAny(hRcpTotalStep, totalStep);
        }

        public static void WriteRCPOperationCommand(short operationState)
        {
            Ads.WriteAny(hCmd_RcpOperation, operationState);
        }

        public static short ReadRCPOperationState()
        {
            return Ads.ReadAny<short>(hState_RcpOperation);
        }

        public static void WriteOperationMode(bool operatonMode)
        {
            Ads.WriteAny(hOperationMode, operatonMode);
        }

        public static short ReadUserState()
        {
            return Ads.ReadAny<short>(hUserState);
        }
     
        public static void WriteOutputCmd1(OutputCmd1Index index, bool powerOn)
        {
            if(bOutputCmd1 != null)
            {
                bOutputCmd1[(int)index] = powerOn;              
                int[] array = new int[1];
                bOutputCmd1.CopyTo(array, 0);
                Ads.WriteAny(hOutputCmd1, (short)array[0]);
            }
        }

        public static BitArray ReadOutputCmd1()
        {
            short outputCmd1 = Ads.ReadAny<short>(hOutputCmd1);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd1) : BitConverter.GetBytes(outputCmd1).Reverse().ToArray());
            

        }

        public static ushort ReadPressureControlMode()
        {
            return Ads.ReadAny<ushort>(hOutputSetType);
        }

        public static void WriteThrottleValveMode(short value)
        {
            Ads.WriteAny(hOutputCmd2, value);
        }

        public static ushort ReadThrottleValveMode()
        {
            return Ads.ReadAny<ushort>(hOutputMode);
        }

        public static bool ReadInputManAuto(uint index)
        {
            ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray())[7];
        }

        public static bool ReadDigitalOutputIO2(int bitIndex)
        {
            return new BitArray(new byte[1] { Ads.ReadAny<byte>(hDigitalOutput2) })[bitIndex];
        }

        public static bool ReadInputState4(int bitIndex)
        {
            short inputState = Ads.ReadAny<short>(hInputState4);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputState) : BitConverter.GetBytes(inputState).Reverse().ToArray())[bitIndex];
        }
    }
}
