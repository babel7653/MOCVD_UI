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
        private static float[] BaMaxValue = new float[29];

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

        public static void WriteDeviceMaxValue(List<GasAIO>? gasAIO)
        {
            // Device Max. Value Write
            try
            {
                if (gasAIO == null)
                {
                    throw new Exception("gasAIO is null in WriteDeviceMaxValue");
                }
                hWriteDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValue");

                foreach (GasAIO entry in gasAIO)
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
            {"R01",0 }, {"R02", 1}, {"R03", 2},{"M01", 3},  {"M02", 4},
            {"M03", 5}, {"M04", 6}, {"M05", 7},  {"M06", 8}, {"M07", 9},
            {"M08", 10},  {"M09", 11},  {"M10", 12},  {"M11", 13}, {"M12", 14},
            {"M13", 15},  {"M14", 16}, {"M15", 17}, {"M16", 18}, {"M17", 19},
            {"M18", 20}, {"M19", 21}, {"E01", 22}, {"E02", 23}, {"E03", 24},
            {"E04", 25}, {"E05", 26}, {"E06", 27}, {"E07", 28},
        };
    }
}
