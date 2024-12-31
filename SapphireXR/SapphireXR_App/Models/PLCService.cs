using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Windows;
using System.Windows.Threading;
using TwinCAT.Ads;

namespace SapphireXR_App.Models
{
    static class PLCService
    {
        public class ReadValveStateException : Exception
        {
            public ReadValveStateException(string message) : base(message) { }
        }

        // Connect to PLC
        public static string AddressPLC { get; set; } = "PLC Address : ";
        public static string ModePLC { get; set; } = "System Mode : ";

        // Variable handles to be connected plc variables
        static uint hStatePLC = 0;
        public static bool TcStatePLC { get; set; }

        private static BitArray? baReadValveStatePLC1;
        private static BitArray? baReadValveStatePLC2;
        private static float[]? aDeviceMaxValue;
        private static float[]? aDeviceTargetValues;
        private static short[]? aDeviceCurrentValues;
        private static short[]? aDeviceControlValues;
        private static short[]? aDeviceRampTimes;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? dCurrentValueIssuers;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? dControlValueIssuers;
        private static Dictionary<string, ObservableManager<(int, int)>.DataIssuer>? dControlCurrentValueIssuers;


        //Create an instance of the TcAdsClient()
        public static AdsClient Ads { get; set; }
        static AmsNetId amsNetId = new("10.10.10.10.1.1");
        private static DispatcherTimer? timer;
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
                hDeviceControlValuePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aGasController_SV");
                //Read Present Value from Device of PLC
                hDeviceCurrentValuePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aGasController_PV");
                //Read and Write Max Value of PLC 
                hDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValueController");

                hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
                hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");
                hWriteDeviceTargetValuePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aGasController_TV");
                hWriteDeviceRampTimePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aGasController_RampTime");

                aDeviceRampTimes = new short[dIndexFlowController.Count];
                aDeviceTargetValues = new float[dIndexFlowController.Count];

                ConnectedNotifier.Issue(PLCConnection.Connecrted);
            }
            else
            {
                throw new TwinCAT.ClientNotConnectedException(null);
            }
        }

        // Read from PLC State
        private static uint hReadValveStatePLC1;
        private static uint hReadValveStatePLC2;
        private static uint hDeviceMaxValuePLC;
        private static uint hDeviceControlValuePLC;
        private static uint hDeviceCurrentValuePLC;
        private static uint hWriteDeviceTargetValuePLC;
        private static uint hWriteDeviceRampTimePLC;
        
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
            foreach (KeyValuePair<string, int> kv in dIndexFlowController)
            {
                dCurrentValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".CurrentValue"));
            }
            dControlValueIssuers = new Dictionary<string, ObservableManager<int>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexFlowController)
            {
                dControlValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".ControlValue"));
            }
            dControlCurrentValueIssuers = new Dictionary<string, ObservableManager<(int, int)>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in dIndexFlowController)
            {
                dControlCurrentValueIssuers.Add(kv.Key, ObservableManager<(int, int)>.Get("FlowControl." + kv.Key + ".ControlTargetValue"));
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(2000000);
            timer.Tick += OnTick;
            timer.Start();
        }

        public static void ReadMaxValueFromPLC()
        {
            aDeviceMaxValue = Ads.ReadAny<float[]>(hDeviceMaxValuePLC, [dIndexFlowController.Count]);
        }

        public static float ReadMaxValue(string flowControlID)
        {
            if(aDeviceMaxValue == null)
            {
                throw new Exception("aDeviceMaxValue is null in ReadMaxValue(), WriteDeviceMaxValue() must be called before");
            }
            return aDeviceMaxValue[dIndexFlowController[flowControlID]];
        }

        private static void OnTick(object? sender, EventArgs e)
        {
            ReadCurrentValueFromPLC();
            if (aDeviceControlValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexFlowController)
                {
                    dControlValueIssuers?[kv.Key].Issue(aDeviceControlValues[dIndexFlowController[kv.Key]]);
                }
            }
            if (aDeviceCurrentValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexFlowController)
                {
                    dCurrentValueIssuers?[kv.Key].Issue(aDeviceCurrentValues[dIndexFlowController[kv.Key]]);
                }
            }
            if(aDeviceControlValues != null && aDeviceTargetValues != null)
            {
                foreach (KeyValuePair<string, int> kv in dIndexFlowController)
                {
                    dControlCurrentValueIssuers?[kv.Key].Issue((aDeviceControlValues[dIndexFlowController[kv.Key]], (int)aDeviceTargetValues[dIndexFlowController[kv.Key]]));
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
            aDeviceCurrentValues = Ads.ReadAny<short[]>(hDeviceCurrentValuePLC, [NumFlowControllers]);
            aDeviceControlValues = Ads.ReadAny<short[]>(hDeviceControlValuePLC, [NumFlowControllers]);
            aDeviceTargetValues = Ads.ReadAny<float[]>(hWriteDeviceTargetValuePLC, [NumFlowControllers]);
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

                float[] maxValue = new float[26];
                uint index = 0;
                uint count = 1;
                foreach (AnalogDeviceIO entry in analogDeviceIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for AnalogDeviceIO");
                    }
                    if (count > 3)
                    {
                        maxValue[index++] = entry.MaxValue;
                    }
                    count++;
                }
                Ads.WriteAny(hDeviceMaxValuePLC, maxValue, [dIndexFlowController.Count]);
                // List Analog Device Input / Output
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void WriteTargetValue(string flowControllerID, int targetValue)
        {
            aDeviceTargetValues![dIndexFlowController[flowControllerID]] = (float)targetValue;
            Ads.WriteAny(hWriteDeviceTargetValuePLC, aDeviceTargetValues!, [aDeviceTargetValues!.Length]);
        }

        public static void WriteRampTime(string flowControllerID, Int16 currentValue)
        {
            aDeviceRampTimes![dIndexFlowController[flowControllerID]] = currentValue;
            Ads.WriteAny(hWriteDeviceRampTimePLC, aDeviceRampTimes!, [aDeviceRampTimes!.Length]);
        }

        private static ObservableManager<PLCConnection>.DataIssuer ConnectedNotifier;


        public static Dictionary<string, int> ValveIDtoOutputSolValveIdx1 = new Dictionary<string, int>
        {
            { "V01", 0 }, { "V02", 1 }, { "V03", 2 }, { "V04", 3 }, { "V05", 4 },
            { "V06", 5 }, { "V07", 6 }, { "V08", 7 }, { "V09", 8 }, { "V10", 9 },
            { "V11", 10 }, { "V12", 11 }, { "V13", 12 }, { "V14", 13 }, { "V15", 14 },
            { "V16", 15 }, { "V37", 16 }, { "V38", 17 }, { "V39", 18 }, { "V40", 19 },
            { "V41", 20 }, { "V42", 21 }, { "V43", 22 }, { "V44", 23 },  { "V45", 24 },
            { "V46", 25 }
        };

        public static Dictionary<string, int> ValveIDtoOutputSolValveIdx2 = new Dictionary<string, int>
        {
            { "V17", 0 }, { "V18", 1 }, { "V19", 2 }, { "V20", 3 }, { "V21", 4 },
            { "V22", 5 }, { "V23", 6 }, { "V24", 7 }, { "V25", 8 }, { "V26", 9 },
            { "V27", 10 }, { "V28", 11 }, { "V29", 12 }, { "V30", 13 }, { "V31", 14 },
            { "V32", 15 }, { "V33", 16 }, { "V34", 17 }, { "V35", 18 }, { "V36", 19 },
            { "V47", 20 }, { "V48", 21 }, { "V49", 22 }, { "V50", 23 },  { "V51", 24 },
            { "V52", 25 }, { "V53", 26 }
        };

        public static Dictionary<string, int> dIndexFlowController = new Dictionary<string, int>
        {
            { "MFC01", 0 }, { "MFC02", 1 }, { "MFC03", 2 }, { "MFC04", 3 }, { "MFC05", 4 },
            { "MFC06", 5 }, { "MFC07", 6 }, { "MFC08", 7 }, { "MFC09", 8 }, { "MFC10", 9 }, 
            { "MFC11", 10 }, { "MFC12", 11 }, { "MFC13", 12 }, { "MFC14", 13 }, { "MFC15", 14 },
            { "MFC16", 15 }, { "MFC17", 16 }, { "MFC18", 17 }, { "MFC19", 18 },
            { "EPC01", 19 },  { "EPC02", 20 }, { "EPC03", 21 }, { "EPC04", 22 }, { "EPC05", 23 },
            { "EPC06", 24 }, { "EPC07", 25 }
        };
        public static int NumFlowControllers = dIndexFlowController.Count;
    }
}
