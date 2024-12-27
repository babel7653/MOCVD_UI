﻿using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using System.Collections;
using System.Reactive.Linq;
using System.Security.Permissions;
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

        private static BitArray? BaReadValveStatePLC1;
        private static BitArray? BaReadValveStatePLC2;
        private static float[]? BaMaxValue;
        private static float[]? BaTargetValue;
        private static int[]? CurrentValues;
        private static int[]? ControlValues;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? CurrentValueIssuers;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? ControlValueIssuers;

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

                hReadFlowControllerControlValuePLC = Ads.CreateVariableHandle("GVL_IO.aAnalogOutputIO");
                hReadFlowControllerCurrentValuePLC = Ads.CreateVariableHandle("GVL_IO.aAnalogInputIO");
                hWriteDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValueController");
                hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
                hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");
                hWriteDeviceTargetValuePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aMFC_TV");

                ConnectedNotifier.Issue(PLCConnection.Connecrted);
            }
            else
            {
                throw new TwinCAT.ClientNotConnectedException(null);
            }
        }

        // Read from PLC State
        public static uint hReadValveStatePLC1 { get; set; }
        public static uint hReadValveStatePLC2 { get; set; }
        public static uint hWriteDeviceMaxValuePLC { get; set; }
        public static uint hReadFlowControllerControlValuePLC { get; set; }
        public static uint hReadFlowControllerCurrentValuePLC { get; set; }
        public static uint hWriteDeviceTargetValuePLC { get; set; }

        public static void WriteDeviceMaxValue(List<AnalogDeviceIO>? analogDeviceIOs)
        {
            // Device Max. Value Write
            try
            {
                if (analogDeviceIOs == null)
                {
                    throw new Exception("AnalogDeviceIO is null in WriteDeviceMaxValue");
                }
               

                foreach (AnalogDeviceIO entry in analogDeviceIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for AnalogDeviceIO");
                    }
                    BaMaxValue = new float[29];
                    BaMaxValue[FlowControllerIDtoIdx[entry.ID]] = entry.MaxValue;
                    Ads.WriteAny(hWriteDeviceMaxValuePLC, BaMaxValue);
                }
                // List Analog Device Input / Output
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void WriteDeviceTargetValue(List<AnalogDeviceIO>? analogDeviceIOs)
        {
            // Device Target Value Write
            try
            {
                List<AnalogDeviceIO> deviceIOs = new();
                if (analogDeviceIOs == null)
                {
                    throw new Exception("AnalogDeviceIO is null in WriteDeviceMaxValue in PLCService");
                }
                if(BaTargetValue == null)
                {
                    throw new Exception("BaTargetValue is null in WriteDeviceTargetValue in PLCService");
                }
                for (int i = 3; i < 22; i++)
                {
                    deviceIOs.Add(analogDeviceIOs[i]);
                }

                foreach (AnalogDeviceIO entry in deviceIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for AnalogDeviceIO");
                    }
                    BaTargetValue[MFCIDtoIdx[entry.ID]] = entry.TargetValue;
                }
                Ads.WriteAny(hWriteDeviceTargetValuePLC, BaTargetValue);
                // List Analog Device Input / Output
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ReadValveStateFromPLC()
        {
            // Solenoid Valve State Read(Update)
            try
            {
                uint aReadValveStatePLC1 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array
                uint aReadValveStatePLC2 = (uint)Ads.ReadAny(hReadValveStatePLC2, typeof(uint)); // Convert to Array
                
                BaReadValveStatePLC1 = new BitArray(new int[] { (int)aReadValveStatePLC1 });
                BaReadValveStatePLC2 = new BitArray(new int[] { (int)aReadValveStatePLC2 });
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
            ReadFlowControlStateFromPLC();

            CurrentValueIssuers = new Dictionary<string, ObservableManager<int>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in FCCurrentValuetoIdx)
            {
                CurrentValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".CurrentValue"));
            }
            ControlValueIssuers = new Dictionary<string, ObservableManager<int>.DataIssuer>();
            foreach (KeyValuePair<string, int> kv in FCControlValuetoIdx)
            {
                ControlValueIssuers.Add(kv.Key, ObservableManager<int>.Get("FlowControl." + kv.Key + ".ControlValue"));
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(5000000);
            timer.Tick += OnTick;
            timer.Start();
        }

        public static void ReadMaxValueFromPLC()
        {
            BaMaxValue = Ads.ReadAny<float[]>(hWriteDeviceMaxValuePLC, [29]);
        }

        public static float ReadMaxValue(string flowControlID)
        {
            if(BaMaxValue == null)
            {
                throw new Exception("BaMaxValue is null in ReadMaxValue(), WriteDeviceMaxValue() must be called before");
            }
            return BaMaxValue[FlowControllerIDtoIdx[flowControlID]];
        }

        private static void OnTick(object? sender, EventArgs e)
        {
            ReadFlowControlStateFromPLC();
            if (ControlValues != null)
            {
                foreach (KeyValuePair<string, int> kv in FCControlValuetoIdx)
                {
                    ControlValueIssuers?[kv.Key].Issue(ControlValues[FCControlValuetoIdx[kv.Key]]);
                }
            }
            if (CurrentValues != null)
            {
                foreach (KeyValuePair<string, int> kv in FCCurrentValuetoIdx)
                {
                    CurrentValueIssuers?[kv.Key].Issue(CurrentValues[FCCurrentValuetoIdx[kv.Key]]);
                }
            }

            string expcetionStr = string.Empty;
            if(ControlValues == null)
            {
                expcetionStr += "ControlValues is null in OnTick PLCService";
            }
            if(CurrentValues == null)
            {
                if(expcetionStr != string.Empty)
                {
                    expcetionStr += "\r\n";
                }
                expcetionStr += "CurrentValues is null in OnTick PLCService";
            }
            if(expcetionStr != string.Empty)
            {
                throw new Exception(expcetionStr);
            }
        }

        private static void ReadFlowControlStateFromPLC()
        {
            CurrentValues = Ads.ReadAny<int[]>(hReadFlowControllerCurrentValuePLC, [40]);
            ControlValues = Ads.ReadAny<int[]>(hReadFlowControllerControlValuePLC, [28]);
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
                if (BaReadValveStatePLC1 == null)
                {
                    throw new ReadValveStateException("PLC Service: BaReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (BaReadValveStatePLC1, index, hReadValveStatePLC1);
                }
            }
            else
                if (ValveIDtoOutputSolValveIdx2.TryGetValue(valveID, out index) == true)
            {
                if (BaReadValveStatePLC2 == null)
                {
                    throw new ReadValveStateException("PLC Service: BaReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (BaReadValveStatePLC2, index, hReadValveStatePLC2);
                }
            }
            else
            {
                throw new ReadValveStateException("PLC Service: non-exsiting valve ID entered to GetReadValveStateBuffer()");
            }
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

        public static Dictionary<string, int> FlowControllerIDtoIdx = new Dictionary<string, int>
        {
            {"M01", 0}, {"M02", 1}, {"M03", 2}, {"M04", 3}, {"M05", 4},
            {"M06", 5}, {"M07", 6}, {"M08", 7},  {"M09", 8},  {"M10", 9},
            {"M11", 10}, {"M12", 11}, {"M13", 12}, {"M14", 13}, {"M15", 14},
            {"M16", 15}, {"M17", 16}, {"M18", 17}, {"M19", 18}, {"E01", 19},
            {"E02", 20}, {"E03", 21}, {"E04", 22}, {"E05", 23}, {"E06", 24},
            {"E07", 25}, {"R01", 26}, {"R02", 27}, {"R03", 28},

        };
        public static Dictionary<string, int> MFCIDtoIdx = new Dictionary<string, int>
        {
             {"M01", 0}, {"M02", 1}, {"M03", 2}, {"M04", 3}, {"M05", 4},
            {"M06", 5}, {"M07", 6}, {"M08", 7},  {"M09", 8},  {"M10", 9},
            {"M11", 10}, {"M12", 11}, {"M13", 12}, {"M14", 13}, {"M15", 14},
            {"M16", 15}, {"M17", 16}, {"M18", 17}, {"M19", 18},

        };

        public static Dictionary<string, int> FCControlValuetoIdx = new Dictionary<string, int> {
            { "MFC01", 1 },  { "MFC02", 2 },  { "MFC03", 3 },  { "MFC04", 4 },  { "MFC05", 5 },  { "MFC06", 6 },  { "MFC07", 7 },  { "MFC08", 8 },  { "MFC9", 9 },  { "MFC10", 10 },
            { "MFC11", 11 },  { "MFC12", 12 },  { "MFC13", 13 },  { "MFC14", 14 },  { "MFC15", 15 },  { "MFC16", 16 },  { "MFC17", 17 },  { "MFC18", 18 },  { "MFC19", 19 },  { "EPC01", 1 },
            { "EPC02", 2 },  { "EPC03", 3 },  { "EPC04", 4 },  { "EPC05", 5 },  { "EPC06", 6 },  { "EPC07", 7 }
        };

        public static Dictionary<string, int> FCCurrentValuetoIdx = new Dictionary<string, int>
        {
            { "MFC01", 1 },  { "MFC02", 2 },  { "MFC03", 3 },  { "MFC04", 4 },  { "MFC05", 5 },  { "MFC06", 6 },  { "MFC07", 7 },  { "MFC08", 8 },  { "MFC9", 9 },  { "MFC10", 10 },
            { "MFC11", 11 },  { "MFC12", 12 },  { "MFC13", 13 },  { "MFC14", 14 },  { "MFC15", 15 },  { "MFC16", 16 },  { "MFC17", 17 },  { "MFC18", 18 },  { "MFC19", 19 },  { "EPC01", 1 },
             { "EPC02", 2 },  { "EPC03", 3 },  { "EPC04", 4 },  { "EPC05", 5 },  { "EPC06", 6 },  { "EPC07", 7 }
        };
    }
}
