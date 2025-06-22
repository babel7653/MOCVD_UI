using SapphireXR_App.Common;
using System.Collections;
using System.Windows.Threading;
using TwinCAT.Ads;
using System.Windows;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        internal class ReadValveStateException : Exception
        {
            public ReadValveStateException(string message) : base(message) { }
        }

        internal class LeakTestModeSubscriber : IObserver<bool>
        {
            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                LeakTestMode = value;
                if(value == false)
                {
                    foreach((string valveID, string coupled) in LeftCoupled)
                    {
                        try
                        {
                            DoWriteValveState(valveID, false);
                            DoWriteValveState(coupled, false);
                        }
                        catch (Exception exception)
                        {
                            if(ShowMessageOnLeakTestModeSubscriberWriteValveState == true)
                            {
                                ShowMessageOnLeakTestModeSubscriberWriteValveState = MessageBox.Show("PLC로부터 Valve 상태를 읽어오는데 실패했습니다. 이 메시지를 다시 표시하지 않으려면 Yes를 클릭하세요. 원인은 다음과 같습니다: " + exception.Message, "",
                                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes ? false : true;
                            }
                        }
                    }
                }
            }
        }

        internal enum HardWiringInterlockStateIndex
        {
            MaintenanceKey = 0, DoorReactorCabinet = 1, DoorGasDeliveryCabinet = 2, DoorPowerDistributeCabinet = 3, CleanDryAir = 4, CoolingWater = 5,
            InductionHeaterReady = 6, InductionHeaterRun = 7, InductionHeaterFault = 8, SusceptorMotorStop = 9, SusceptorMotorRun = 10, SusceptorMotorFault = 11,
            VacuumPumpWarning = 12, VacuumPumpRun = 13, VacuumPumpFault = 14
        };

        const int NumShortBits = sizeof(short) * 8;
        internal enum IOListIndex
        {
            PowerResetSwitch = 2, Cover_UpperLimit = 3, Cover_LowerLimit = 4, SMPS_24V480 = 5, SMPS_24V72 = 6, SMPS_15VPlus= 7, SMPS_15VMinus = 8, CP_InudctionHeater = 9, 
            CP_ThermalBath = 10,  CP_VaccumPump = 11, CP_LineHeater = 12, CP_RotationMotor = 13, CP_CoverMotor = 14, CP_ThrottleValve = 15, CP_Lamp = NumShortBits * 1, 
            CP_SM515CP = NumShortBits * 1 + 1, LineHeader1 = NumShortBits * 1 + 2, LineHeader2 = NumShortBits * 1 + 3, LineHeader3 = NumShortBits * 1 + 4,  LineHeader4 = NumShortBits * 1 + 5, 
            LineHeader5 = NumShortBits * 1 + 6, LineHeader6 = NumShortBits * 1 + 7, LineHeader7 = NumShortBits * 1 + 8, LineHeader8 = NumShortBits * 1 + 9, 
            Bath_DeviationAlaram1 = NumShortBits * 1 + 10, Bath_DeviationAlaram2 = NumShortBits * 1 + 11, Bath_DeviationAlaram3 = NumShortBits * 1 + 12, 
            Bath_DeviationAlaram4 = NumShortBits * 1 + 13,  Bath_DeviationAlaram5 = NumShortBits * 1 + 14, Bath_DeviationAlaram6 = NumShortBits * 1 + 15, 
            SingalTower_RED = NumShortBits * 2, SingalTower_YELLOW = NumShortBits * 2 + 1, SingalTower_GREEN = NumShortBits * 2 + 2, SingalTower_BLUE = NumShortBits * 2 + 3, 
            SingalTower_WHITE = NumShortBits * 2 + 4, SingalTower_BUZZWER = NumShortBits * 2 + 5, DOR_Vaccum_State = NumShortBits * 2 + 6, Temp_Controller_Alarm = NumShortBits * 2 + 7
        };

        internal enum DigitalOutput2Index
        {
            InductionHeaterOn = 0, InductionHeaterReset, VaccumPumpOn, VaccumPumpReset, 
        }

        public enum DigitalOutput3Index
        {
            InductionHeaterMC = 0, ThermalBathMC, VaccumPumpMC, LineHeaterMC, RotationAlaramReset = 6
        }

        public enum OutputCmd1Index
        {
            InductionHeaterPower = 0, ThermalBathPower, VaccumPumpPower, LineHeaterPower, InductionHeaterControl, InductionHeaterReset, VaccumPumpControl, VaccumPumpReset, TempControllerManAuto = 11, PressureControlMode = 12
        }

        public enum OutputSetType: ushort
        {
            Pressure = 1, Position = 2
        }

        // Variable handles to be connected plc variables
        private static BitArray? baReadValveStatePLC1 = null;
        private static BitArray? baReadValveStatePLC2 = null;
        private static float[]? aDeviceTargetValues = null;
        private static float[]? aDeviceCurrentValues = null;
        private static float[]? aDeviceControlValues = null;
        private static short[]? aDeviceRampTimes = null;
        private static float[]? aMonitoring_PVs = null;
        private static short[]? aInputState = null;
        private static BitArray? bOutputCmd1 = null;

        private static Dictionary<string, ObservableManager<float>.DataIssuer>? dCurrentValueIssuers;
        private static Dictionary<string, ObservableManager<float>.DataIssuer>? dControlValueIssuers;
        private static Dictionary<string, ObservableManager<float>.DataIssuer>? dTargetValueIssuers;
        private static Dictionary<string, ObservableManager<(float, float)>.DataIssuer>? dControlCurrentValueIssuers;
        private static Dictionary<string, ObservableManager<float>.DataIssuer>? aMonitoringCurrentValueIssuers;
        private static ObservableManager<BitArray>.DataIssuer? baHardWiringInterlockStateIssuers;
        private static ObservableManager<BitArray>.DataIssuer? dIOStateList;
        private static Dictionary<string, ObservableManager<bool>.DataIssuer>? dValveStateIssuers;
        private static ObservableManager<bool>.DataIssuer? dRecipeEndedPublisher;
        private static ObservableManager<short>.DataIssuer? dCurrentActiveRecipeIssue;
        private static ObservableManager<float[]>.DataIssuer? dLineHeaterTemperatureIssuers;
        private static ObservableManager<int>.DataIssuer? dRecipeControlHoldTimeIssuer;
        private static ObservableManager<int>.DataIssuer? dRecipeControlRampTimeIssuer;
        private static ObservableManager<int>.DataIssuer? dRecipeControlPauseTimeIssuer;
        private static ObservableManager<BitArray>.DataIssuer? dDigitalOutput2;
        private static ObservableManager<BitArray>.DataIssuer? dDigitalOutput3;
        private static ObservableManager<BitArray>.DataIssuer? dOutputCmd1;
        private static ObservableManager<BitArray>.DataIssuer? dInputManAuto;
        private static ObservableManager<short>.DataIssuer? dThrottleValveControlMode;
        private static ObservableManager<ushort>.DataIssuer? dPressureControlModeIssuer;
        private static ObservableManager<short>.DataIssuer? dThrottleValveStatusIssuer;
        private static ObservableManager<BitArray>.DataIssuer? dLogicalInterlockStateIssuer;

        private static LeakTestModeSubscriber? leakTestModeSubscriber = null;

        //Create an instance of the TcAdsClient()
        public static AdsClient Ads { get; set; }
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
        private static uint hCmd_RcpOperation;
        private static uint hRcpStepN;
        private static uint hMonitoring_PV;
        private static uint hInputState;
        private static uint hInputState4;
        private static uint hState_RcpOperation;
        private static uint hTemperaturePV;
        private static uint hOperationMode;
        private static uint hUserState;
        private static uint hRecipeControlHoldTime;
        private static uint hRecipeControlRampTime;
        private static uint hRecipeControlPauseTime;
        private static uint hDigitalOutput;
        private static uint hDigitalOutput2;
        private static uint hOutputCmd;
        private static uint hE3508InputManAuto;
        private static uint hOutputCmd1;
        private static uint hOutputCmd2;
        private static uint hOutputSetType;
        private static uint hOutputMode;
        private static uint hInterlock1;

        private static bool RecipeRunEndNotified = false;
        private static bool LeakTestMode = true;

        private static bool ShowMessageOnLeakTestModeSubscriberWriteValveState = true;
        private static bool ShowMessageOnOnTick = true;

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
            { "UltimatePressure", 0 },  { "ExtPressure", 1},  { "DorPressure", 2}, { Util.GetGasDeviceName("Gas1") ?? "Gas 1", 3}, {  Util.GetGasDeviceName("Gas2") ?? "Gas 2", 4}, { Util.GetGasDeviceName("Gas3") ?? "Gas3", 5},
            { Util.GetGasDeviceName("Gas4") ?? "Gas4", 6}, { "ShowerHeadTemp", 7}, { "InductionCoilTemp", 8}, { "HeaterPowerRate", 9 }, { "ValvePosition", 10 }, { Util.GetGasDeviceName("Source1") ?? "Source1", 11},
             { Util.GetGasDeviceName("Source2") ?? "Source2", 12},  { Util.GetGasDeviceName("Source3") ?? "Source3", 13},  { Util.GetGasDeviceName("Source4") ?? "Source4", 14},  { Util.GetGasDeviceName("Source5") ?? "Source5", 15},  { Util.GetGasDeviceName("Source6") ?? "Source6", 16}
        };

        public static readonly uint LineHeaterTemperature = 8;

        private static Dictionary<string, string> LeftCoupled = new Dictionary<string, string>();
        private static Dictionary<string, string> RightCoupled = new Dictionary<string, string>();
    }
}
