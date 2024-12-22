using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using System.Collections;
using System.Windows;
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

        private static BitArray? BaReadValveStatePLC1 = new BitArray(32);
        private static BitArray? BaReadValveStatePLC2 = new BitArray(32);
        private static float[] BaMaxValue = new float[40];
        private static float[] BaTargetValue = new float[19];

        //Create an instance of the TcAdsClient()
        public static AdsClient Ads { get; set; }
        static AmsNetId amsNetId = new("10.10.10.10.1.1");

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
        public static uint hWriteDeviceTargetValuePLC { get; set; }

        public static void WriteDeviceMaxValue(List<GasAIO>? gasAIOs)
        {
            // Device Max. Value Write
            try
            {
                if (gasAIOs == null)
                {
                    throw new Exception("gasAIO is null in WriteDeviceMaxValue");
                }
                hWriteDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValue");

                foreach (GasAIO entry in gasAIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for gasAIO");
                    }
                    BaMaxValue[FlowControllerIDtoIdx[entry.ID]] = entry.MaxValue;
                }
                Ads.WriteAny(hWriteDeviceMaxValuePLC, BaMaxValue);
                //lGasAIO
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void WriteDeviceTargetValue(List<GasAIO>? gasAIOs)
        {
            // Device Target Value Write
            try
            {
                List<GasAIO> gass = new();
                if (gasAIOs == null)
                {
                    throw new Exception("gasAIO is null in WriteDeviceMaxValue");
                }
                for (int i = 3; i < 22; i++)
                {
                    gass.Add(gasAIOs[i]);
                }
                hWriteDeviceTargetValuePLC = Ads.CreateVariableHandle("P30_GasFlowControl.aMFC_TV");

                foreach (GasAIO entry in gass)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for gasAIO");
                    }
                    BaTargetValue[MFCIDtoIdx[entry.ID]] = entry.TargetValue;
                }
                Ads.WriteAny(hWriteDeviceTargetValuePLC, BaTargetValue);
                //lGasAIO
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
                hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
                uint aReadValveStatePLC1 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array

                hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");
                uint aReadValveStatePLC2 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array

                BaReadValveStatePLC1 = new BitArray(new int[] { (int)aReadValveStatePLC1 });
                BaReadValveStatePLC2 = new BitArray(new int[] { (int)aReadValveStatePLC2 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private static ObservableManager<PLCConnection>.DataIssuerBase ConnectedNotifier;


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
    }
}
