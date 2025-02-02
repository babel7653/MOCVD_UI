using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Windows.Threading;
using TwinCAT.Ads;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        public class ReadValveStateException : Exception
        {
            public ReadValveStateException(string message) : base(message) { }
        }

        public enum HardWiringInterlockStateIndex
        {
            MaintainenceKey = 0, DoorReactorCabinet = 1, DoorGasDeliveryCabinet = 2, DoorPowerDistributeCabinet = 3, CleanDryAir = 4, CoolingWater = 5,
            InductionHeaterReady = 6, inductionHeaterRun = 7, inductionHeaterFault = 8, SusceptorMotorStop = 9, SusceptorMotorRun = 10, SusceptorMotorFault = 11,
            VacuumPumpWarning = 12, VacuumPumpRun = 13, VacuumPumpFault = 14,
        };

        public enum IOListIndex
        {
            PowerResetSwitch = 0, Cover1, Cover2, SMPS1, SMPS2, SMPS3, SMPS4, CP1, CP2, CP3, CP4, CP5, CP6, CP7, CP8, CP9, LineHeader1, LineHeader2, LineHeader3, LineHeader4,
            LineHeader5, LineHeader6, LineHeader7, LineHeader8, ThermalBath1, ThermalBath2, ThermalBath3, ThermalBath4, ThermalBath5, ThermalBath6, SingalTower1, SingalTower2,
            SingalTower3, SingalTower4, SingalTower5, SingalTower6
        };

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
        private static float[]? aMonitoring_PVs;
        private static short[]? aInputState;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? dCurrentValueIssuers;
        private static Dictionary<string, ObservableManager<int>.DataIssuer>? dControlValueIssuers;
        private static Dictionary<string, ObservableManager<(int, int)>.DataIssuer>? dControlCurrentValueIssuers;
        private static Dictionary<string, ObservableManager<float>.DataIssuer>? aMonitoringCurrentValueIssuers;
        private static ObservableManager<BitArray>.DataIssuer? dHardWiringInterlockStateIssuers;

        private static ObservableManager<short>.DataIssuer? dCurrentActiveRecipeIssue;

        //Create an instance of the TcAdsClient()
        public static AdsClient Ads { get; set; }
        private static AmsNetId amsNetId = new("10.10.10.10.1.1");
        private static DispatcherTimer? timer;
        private static DispatcherTimer? currentActiveRecipeListener;

        // Read from PLC State
        private static uint hReadValveStatePLC1;
        private static uint hReadValveStatePLC2;
        private static uint hDeviceMaxValuePLC;
        private static uint hDeviceControlValuePLC;
        private static uint hDeviceCurrentValuePLC;
        private static uint hWriteDeviceTargetValuePLC;
        private static uint hWriteDeviceRampTimePLC;
        private static uint hRcp;
        private static uint hRcpTotalStep;
        private static uint hRcpStart;
        private static uint hRcpState;
        private static uint hRcpStepN;
        private static uint hMonitoring_PV;
        private static uint hInputState;

        private static ObservableManager<PLCConnection>.DataIssuer ConnectedNotifier;

        public static readonly Dictionary<string, int> ValveIDtoOutputSolValveIdx1 = new Dictionary<string, int>
        {
            { "V01", 0 }, { "V02", 1 }, { "V03", 2 }, { "V04", 3 }, { "V05", 4 },
            { "V06", 5 }, { "V07", 6 }, { "V08", 7 }, { "V09", 8 }, { "V10", 9 },
            { "V11", 10 }, { "V12", 11 }, { "V13", 12 }, { "V14", 13 }, { "V15", 14 },
            { "V16", 15 }, { "V37", 16 }, { "V38", 17 }, { "V39", 18 }, { "V40", 19 },
            { "V41", 20 }, { "V42", 21 }, { "V43", 22 }, { "V44", 23 },  { "V45", 24 },
            { "V46", 25 }
        };

        public static readonly Dictionary<string, int> ValveIDtoOutputSolValveIdx2 = new Dictionary<string, int>
        {
            { "V17", 0 }, { "V18", 1 }, { "V19", 2 }, { "V20", 3 }, { "V21", 4 },
            { "V22", 5 }, { "V23", 6 }, { "V24", 7 }, { "V25", 8 }, { "V26", 9 },
            { "V27", 10 }, { "V28", 11 }, { "V29", 12 }, { "V30", 13 }, { "V31", 14 },
            { "V32", 15 }, { "V33", 16 }, { "V34", 17 }, { "V35", 18 }, { "V36", 19 },
            { "V47", 20 }, { "V48", 21 }, { "V49", 22 }, { "V50", 23 },  { "V51", 24 },
            { "V52", 25 }, { "V53", 26 }
        };

        public static readonly Dictionary<string, int> dIndexController = new Dictionary<string, int>
        {
            { "MFC01", 0 }, { "MFC02", 1 }, { "MFC03", 2 }, { "MFC04", 3 }, { "MFC05", 4 },
            { "MFC06", 5 }, { "MFC07", 6 }, { "MFC08", 7 }, { "MFC09", 8 }, { "MFC10", 9 },
            { "MFC11", 10 }, { "MFC12", 11 }, { "MFC13", 12 }, { "MFC14", 13 }, { "MFC15", 14 },
            { "MFC16", 15 }, { "MFC17", 16 }, { "MFC18", 17 }, { "MFC19", 18 },
            { "EPC01", 19 },  { "EPC02", 20 }, { "EPC03", 21 }, { "EPC04", 22 }, { "EPC05", 23 },
            { "EPC06", 24 }, { "EPC07", 25 }, {"Temperature", 26}, {"Pressure", 27}, {"Rotation", 28}
        };
        public static readonly int NumControllers = dIndexController.Count;

        public static readonly Dictionary<string, int> dMonitoringMeterIndex = new Dictionary<string, int>
        {
            { "UltPressure", 0 },  { "ExtPressure", 1},  { "DorPressure", 2}, { "H2", 3}, { "N2", 4}, { "NH3", 5},
            { "SiH4", 6}, { "ShowerHeadTemp", 7}, { "InductionCoilTemp", 8}, { "HeaterPowerRate", 9 }, { "ValvePosition", 10 }, { "TEB", 11},
             { "TMAl", 12},  { "TMIn", 13},  { "TMGa", 14},  { "DTMGa", 15},  { "Cp2Mg", 16}
        };
    }
}
