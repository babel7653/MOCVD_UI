using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using TwinCAT.Ads;

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
            Ads.Connect(AmsNetId.Local, 851);
            if (Ads.IsConnected == true)
            {
                hStatePLC = Ads.CreateVariableHandle("MAIN.StatePLC");
                TcStatePLC = (bool)Ads.ReadAny(hStatePLC, typeof(bool));
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
                
                hRcp = Ads.CreateVariableHandle("RCP.aRecipe");
                hRcpTotalStep = Ads.CreateVariableHandle("RCP.iRcpTotalStep");
                hRcpStart = Ads.CreateVariableHandle("RCP.bRcpStart");
                hRcpState = Ads.CreateVariableHandle("RCP.iRcpOperationState");
                hRcpStepN =Ads.CreateVariableHandle("RCP.iRcpStepN");
                

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

            dCurrentValueIssuers = new Dictionary<string, ObservableManager<int>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dCurrentValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".CurrentValue"));
            }
            dControlValueIssuers = new Dictionary<string, ObservableManager<int>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".ControlValue"));
            }
            dControlCurrentValueIssuers = new Dictionary<string, ObservableManager<(int, int)>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexController)
            {
                dControlCurrentValueIssuers.Add(kv.Key, ObservableManager<(int, int)>.Get("FlowControl." + kv.Key + ".ControlTargetValue.CurrentPLCState"));
            }
            dCurrentActiveRecipeIssue = ObservableManager<short>.Get("RecipeRun.CurrentActiveRecipe");

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(2000000);
            timer.Tick += OnTick;
            timer.Start();

            currentActiveRecipeListener = new DispatcherTimer();
            currentActiveRecipeListener.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 500);
            currentActiveRecipeListener.Tick += (object? sender, EventArgs e) => { dCurrentActiveRecipeIssue.Issue(Ads.ReadAny<short>(hRcpStepN)); };
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
            if(aDeviceControlValues != null && aDeviceTargetValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dControlCurrentValueIssuers?[kv.Key].Issue((aDeviceControlValues[dIndexController[kv.Key]], (int)aDeviceTargetValues[dIndexController[kv.Key]]));
                }
            }

            string expcetionStr = string.Empty;
            if(aDeviceControlValues == null)
            {
                expcetionStr += "aDeviceControlValues is null in OnTick PLCService";
            }
            if(aDeviceCurrentValues == null)
            {
                if(expcetionStr != string.Empty)
                {
                    expcetionStr += "\r\n";
                }
                expcetionStr += "aDeviceCurrentValues is null in OnTick PLCService";
            }
            if(aDeviceTargetValues == null)
            {
                if (expcetionStr != string.Empty)
                {
                    expcetionStr += "\r\n";
                }
                expcetionStr += "aDeviceTargetValues is null in OnTick PLCService";
            }
            if(expcetionStr != string.Empty)
            {
                throw new Exception(expcetionStr);
            }
        }

        private static void ReadCurrentValueFromPLC()
        {
            aDeviceCurrentValues = Ads.ReadAny<short[]>(hDeviceCurrentValuePLC, [NumControllers]);
            aDeviceControlValues = Ads.ReadAny<short[]>(hDeviceControlValuePLC, [NumControllers]);
            aDeviceTargetValues = Ads.ReadAny<float[]>(hWriteDeviceTargetValuePLC, [NumControllers]);
            aMonitoring_PVs = Ads.ReadAny<float[]>(hMonitoring_PV, [11]);
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

        public static void WriteValveState(string valveID, bool onOff)
        {
            (BitArray buffer, int index, uint variableHandle) = GetBuffer(valveID);
            buffer[index] = onOff;

            uint[] sentBuffer = new uint[1];
            buffer.CopyTo(sentBuffer, 0);
            Ads.WriteAny(variableHandle, sentBuffer);
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
            Ads.WriteAny(hRcp, recipe);
        }

        public static void WriteTotalStep(short totalStep)
        {
           Ads.WriteAny(hRcpTotalStep, totalStep);
        }

        public static void WriteStart(bool start)
        {
            Ads.WriteAny(hRcpStart, start);
        }

        public static void WriteOperationState(short operationState)
        {
            Ads.WriteAny(hRcpState, operationState);
        }
    }
}
