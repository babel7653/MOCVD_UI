using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using TwinCAT.Ads;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.WindowServices;

namespace SapphireXR_App.ViewModels
{
    public partial class LeftViewModel : ObservableObject
    {
        public abstract partial class SourceStatusViewModel : ObservableObject, IDisposable
        {
            private class ValveStateSubscriber : IObserver<bool>
            {
                internal ValveStateSubscriber(SourceStatusViewModel vm, Action<bool> onNextValveStateAC, string valveIDStr)
                {
                    onNextValveState = onNextValveStateAC;
                    sourceStatusViewModel = vm;
                    valveID = valveIDStr;
                }

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
                    if (currentValveState == null || currentValveState != value)
                    {
                        onNextValveState(value);
                        currentValveState = value;
                    }
                }

                protected readonly SourceStatusViewModel sourceStatusViewModel;
                private readonly Action<bool> onNextValveState;
                public readonly string valveID;
                private bool? currentValveState = null; 
            }

            public SourceStatusViewModel(LeftViewModel vm, string valveStateSubscsribePostfixStr)
            {
                valveStateSubscsribePostfix = valveStateSubscsribePostfixStr;
                valveStateSubscrbers = [
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { SiH4Carrier = NH3_2Carrier = NH3_1Carrier = vm.Gas1; SiH4CarrierColor = NH3_2CarrierColor = NH3_1CarrierColor = H2Color;  } 
                        else { SiH4Carrier = NH3_2Carrier = NH3_1Carrier = vm.Gas2; SiH4CarrierColor = NH3_2CarrierColor = NH3_1CarrierColor = DefaultColor; }  }, "V01"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TEBCarrier = vm.Gas1; TEBCarrierColor = H2Color; } else { TEBCarrier = vm.Gas2; TEBCarrierColor = DefaultColor; }  }, "V05"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMAlCarrier = vm.Gas1; TMAlCarrierColor = H2Color; } else { TMAlCarrier = vm.Gas2; TMAlCarrierColor = DefaultColor; } }, "V08"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMInCarrier = vm.Gas1;  TMInCarrierColor = H2Color;} else { TMInCarrier = vm.Gas2;  TMInCarrierColor = DefaultColor;} }, "V11"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMGaCarrier = vm.Gas1;  TMGaCarrierColor = H2Color;} else { TMGaCarrier = vm.Gas2;  TMGaCarrierColor =DefaultColor;} }, "V14"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { DTMGaCarrier = vm.Gas1;  DTMGaCarrierColor = H2Color;} else { DTMGaCarrier = vm.Gas2;  DTMGaCarrierColor = DefaultColor;}  }, "V17"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { Cp2MgCarrier = vm.Gas1;  Cp2MgCarrierColor = H2Color;} else { Cp2MgCarrier = vm.Gas2;  Cp2MgCarrierColor = DefaultColor;} }, "V20"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { NH3_1Source = NH3_2Source = "On";  NH3_1SourceColor = NH3_2SourceColor = OnColor; } else { NH3_1Source = NH3_2Source = "Off";  NH3_1SourceColor = NH3_2SourceColor = DefaultColor;} }, "V04"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { SiH4Source = "On";  SiH4SourceColor = OnColor;} else { SiH4Source = "Off";  SiH4SourceColor =DefaultColor;} }, "V03"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TEBSource = "On";  TEBSourceColor =OnColor;} else { TEBSource = "Off";  TEBSourceColor = DefaultColor;}  }, "V07"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMAlSource = "On";  TMAlSourceColor = OnColor;} else { TMAlSource = "Off";  TMAlSourceColor = DefaultColor;} }, "V10"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMInSource = "On";  TMInSourceColor = OnColor;} else { TMInSource = "Off";  TMInSourceColor = DefaultColor;} }, "V13"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMGaSource = "On"; TMGaSourceColor = OnColor; } else { TMGaSource = "Off"; TMGaSourceColor = DefaultColor; } }, "V16"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { DTMGaSource = "On"; DTMGaSourceColor = OnColor; } else { DTMGaSource = "Off"; DTMGaSourceColor =DefaultColor; } }, "V19"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { Cp2MgSource = "On"; Cp2MgSourceColor = OnColor; } else { Cp2MgSource = "Off"; Cp2MgSourceColor = DefaultColor; }  }, "V22"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { NH3_1Vent = "Run"; NH3_1VentColor = RunColor; } else { NH3_1Vent = "Vent"; NH3_1VentColor = DefaultColor; } }, "V29"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { NH3_2Vent = "Run"; NH3_2VentColor = RunColor; } else { NH3_2Vent = "Vent"; NH3_2VentColor = DefaultColor; } }, "V30"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { SiH4Vent = "Run"; SiH4VentColor = RunColor; } else { SiH4Vent = "Vent"; SiH4VentColor = DefaultColor; } }, "V31"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TEBVent = "Run"; TEBVentColor = RunColor; } else { TEBVent = "Vent"; TEBVentColor = DefaultColor; }  }, "V23"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMAlVent = "Run"; TMAlVentColor = RunColor; } else { TMAlVent = "Vent";  TMAlVentColor = DefaultColor;} }, "V24"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMInVent = "Run"; TMInVentColor = RunColor; } else { TMInVent = "Vent"; TMInVentColor = DefaultColor; } }, "V25"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMGaVent = "Run"; TMGaVentColor = RunColor; } else { TMGaVent = "Vent"; TMGaVentColor = DefaultColor; } }, "V26"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { DTMGaVent = "Run"; DTMGaVentColor =RunColor; } else { DTMGaVent = "Vent"; DTMGaVentColor = DefaultColor; } }, "V27"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { Cp2MgVent = "Run"; Cp2MgVentColor =RunColor; } else { Cp2MgVent = "Vent"; Cp2MgVentColor = DefaultColor; } }, "V28")
                ];
                foreach (ValveStateSubscriber valveStateSubscriber in valveStateSubscrbers)
                {
                    unsubscribers.Add(ObservableManager<bool>.Subscribe("Valve.OnOff." + valveStateSubscriber.valveID + "." + valveStateSubscsribePostfix, valveStateSubscriber));
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        foreach (IDisposable unsubscriber in unsubscribers)
                        {
                            unsubscriber.Dispose();
                        }
                        unsubscribers.Clear();
                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public void dispose()
            {
                Dispose(disposing: true);
            }

            [ObservableProperty]
            private string _nH3_1Carrier = "";
            [ObservableProperty]
            private string _nH3_1Source = "";
            [ObservableProperty]
            private string _nH3_1Vent = "";
            [ObservableProperty]
            private string _nH3_2Carrier = "";
            [ObservableProperty]
            private string _nH3_2Source = "";
            [ObservableProperty]
            private string _nH3_2Vent = "";
            [ObservableProperty]
            private string _siH4Carrier = "";
            [ObservableProperty]
            private string _siH4Source = "";
            [ObservableProperty]
            private string _siH4Vent = "";
            [ObservableProperty]
            private string _tEBCarrier = "";
            [ObservableProperty]
            private string _tEBSource = "";
            [ObservableProperty]
            private string _tEBVent = "";
            [ObservableProperty]
            private string _tMAlCarrier = "";
            [ObservableProperty]
            private string _tMAlSource = "";
            [ObservableProperty]
            private string _tMAlVent = "";
            [ObservableProperty]
            private string _tMInCarrier = "";
            [ObservableProperty]
            private string _tMInSource = "";
            [ObservableProperty]
            private string _tMInVent = "";
            [ObservableProperty]
            private string _tMGaCarrier = "";
            [ObservableProperty]
            private string _tMGaSource = "";
            [ObservableProperty]
            private string _tMGaVent = "";
            [ObservableProperty]
            private string _dTMGaCarrier = "";
            [ObservableProperty]
            private string _dTMGaSource = "";
            [ObservableProperty]
            private string _dTMGaVent = "";
            [ObservableProperty]
            private string _cp2MgCarrier = "";
            [ObservableProperty]
            private string _cp2MgSource = "";
            [ObservableProperty]
            private string _cp2MgVent = "";

            [ObservableProperty]
            private Brush _nH3_1CarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _nH3_1SourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _nH3_1VentColor = DefaultColor;
            [ObservableProperty]
            private Brush _nH3_2CarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _nH3_2SourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _nH3_2VentColor = DefaultColor;
            [ObservableProperty]
            private Brush _siH4CarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _siH4SourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _siH4VentColor = DefaultColor;
            [ObservableProperty]
            private Brush _tEBCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _tEBSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _tEBVentColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMAlCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMAlSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMAlVentColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMInCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMInSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMInVentColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMGaCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMGaSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _tMGaVentColor = DefaultColor;
            [ObservableProperty]
            private Brush _dTMGaCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _dTMGaSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _dTMGaVentColor = DefaultColor;
            [ObservableProperty]
            private Brush _cp2MgCarrierColor = DefaultColor;
            [ObservableProperty]
            private Brush _cp2MgSourceColor = DefaultColor;
            [ObservableProperty]
            private Brush _cp2MgVentColor = DefaultColor;

            private readonly ValveStateSubscriber[] valveStateSubscrbers;
            private IList<IDisposable> unsubscribers = new List<IDisposable>();
            private readonly string valveStateSubscsribePostfix;

            private static Brush DefaultColor = Application.Current.Resources.MergedDictionaries[0]["SourceStatusDefault"] as Brush ?? Brushes.Black;
            private static Brush H2Color = Application.Current.Resources.MergedDictionaries[0]["SourceStatusH2"] as Brush ?? Brushes.Black;
            private static Brush OnColor = Application.Current.Resources.MergedDictionaries[0]["SourceStatusOn"] as Brush ?? Brushes.Black;
            private static Brush RunColor = Application.Current.Resources.MergedDictionaries[0]["SourceStatusRun"] as Brush ?? Brushes.Black;

            private bool disposedValue = false;
        }

        public class SourceStatusFromCurrentPLCStateViewModel : SourceStatusViewModel
        {
            public SourceStatusFromCurrentPLCStateViewModel(LeftViewModel vm) : base(vm, "CurrentPLCState") { }
        }

        public class SourceStatusFromCurrentRecipeStepViewModel: SourceStatusViewModel
        {
            public SourceStatusFromCurrentRecipeStepViewModel(LeftViewModel vm) :base(vm, "CurrentRecipeStep") {  }
        }

        public LeftViewModel()
        {
            ObservableManager<float>.Subscribe("MonitoringPresentValue.ShowerHeadTemp.CurrentValue", showerHeaderTempSubscriber = new CoolingWaterValueSubscriber("ShowerHeadTemp", this));
            ObservableManager<float>.Subscribe("MonitoringPresentValue.InductionCoilTemp.CurrentValue", inductionCoilTempSubscriber = new CoolingWaterValueSubscriber("InductionCoilTemp", this));
            ObservableManager<BitArray>.Subscribe("HardWiringInterlockState", hardWiringInterlockStateSubscriber = new HardWiringInterlockStateSubscriber(this));
            ObservableManager<int>.Subscribe("MainView.SelectedTabIndex", mainViewTabIndexChagedSubscriber = new MainViewTabIndexChagedSubscriber(this));
            ObservableManager<BitArray>.Subscribe("DeviceIOList", signalTowerStateSubscriber = new SignalTowerStateSubscriber(this));
            ObservableManager<float[]>.Subscribe("LineHeaterTemperature", lineHeaterTemperatureSubscriber = new LineHeaterTemperatureSubscriber(this));
            ObservableManager<bool>.Subscribe("Reset.CurrentRecipeStep", resetCurrentRecipeSubscriber = new ResetCurrentRecipeSubscriber(this));
            ObservableManager<BitArray>.Subscribe("LogicalInterlockState", logicalInterlockSubscriber = new LogicalInterlockSubscriber(this));
            ObservableManager<(string, string)>.Subscribe("GasIOLabelChanged", gasIOLabelSubscriber = new GasIOLabelSubscriber(this));
            ObservableManager<PLCConnection>.Subscribe("PLCService.Connected", plcConnectionStateSubscriber = new PLCConnectionStateSubscriber(this)); 
          
            CurrentSourceStatusViewModel = new SourceStatusFromCurrentPLCStateViewModel(this);
            PropertyChanging += (object? sender, PropertyChangingEventArgs args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(CurrentSourceStatusViewModel):
                        CurrentSourceStatusViewModel.dispose();
                        break;
                }
            };
            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(PLCConnectionStatus):
                        switch(PLCConnectionStatus)
                        {
                            case "Connected":
                                PLCConnectionStatusColor = PLCConnectedFontColor;
                                break;

                            case "Disconnected":
                                PLCConnectionStatusColor = PLCDisconnectedFontColor;
                                break;
                        }
                        break;
                }
            };
            setConnectionStatusText(PLCService.Connected);
        }

        public static string GetGas3Label(string? gas3Name, int index)
        {
            if (gas3Name != default)
            {
                return gas3Name + "#" + index;
            }
            else
            {
                return "";
            }
        }

        public static string GetIogicalInterlockLabel(string? gasName)
        {
            if (gasName != default)
            {
                return "Gas Pressure " + gasName;
            }
            else
            {
                return "";
            }
        }

        private void setConnectionStatusText(PLCConnection connectionStatus)
        {
            switch (connectionStatus)
            {
                case PLCConnection.Connected:
                    PLCConnectionStatus = "Connected";
                    break;

                case PLCConnection.Disconnected:
                    PLCConnectionStatus = "Disconnected";
                    break;
            }
        }

        [RelayCommand]
        public void ToggleBuzzerOnOff()
        {
            bool onOff = BuzzerImage == LeftViewModel.BuzzerOffPath;
            if(ConfirmMessage.Show("Buzzer 상태 변경", "Buzzer" + (onOff == true ? " On" : " Off") + " 상태로 변경하시겠습니까?", WindowStartupLocation.Manual) == ValveOperationExResult.Ok)
            {
                PLCService.WriteBuzzerOnOff(onOff);
            }
            
        }

        [ObservableProperty]
        private static string _gas3_1 = GetGas3Label(Util.GetGasDeviceName("Gas3"), 1);
        [ObservableProperty]
        private static string _gas3_2 = GetGas3Label(Util.GetGasDeviceName("Gas3"), 2);
        [ObservableProperty]
        private static string _gas4 = Util.GetGasDeviceName("Gas4") ?? "";
        [ObservableProperty]
        private static string _source1 = Util.GetGasDeviceName("Source1") ?? "";
        [ObservableProperty]
        private static string _source2 = Util.GetGasDeviceName("Source2") ?? "";
        [ObservableProperty]
        private static string _source3 = Util.GetGasDeviceName("Source3") ?? "";
        [ObservableProperty]
        private static string _source4 = Util.GetGasDeviceName("Source4") ?? "";
        [ObservableProperty]
        private static string _source5 = Util.GetGasDeviceName("Source5") ?? "";
        [ObservableProperty]
        private static string _source6 = Util.GetGasDeviceName("Source6") ?? "";
        [ObservableProperty]
        private static string _logicalInterlockGas1 = GetIogicalInterlockLabel(Util.GetGasDeviceName("Gas1"));
        [ObservableProperty]
        private static string _logicalInterlockGas2 = GetIogicalInterlockLabel(Util.GetGasDeviceName("Gas2"));
        [ObservableProperty]
        private static string _logicalInterlockGas3 = GetIogicalInterlockLabel(Util.GetGasDeviceName("Gas3"));
        [ObservableProperty]
        private static string _logicalInterlockGas4 = GetIogicalInterlockLabel(Util.GetGasDeviceName("Gas4"));

        private static readonly Brush OnLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOnColor"] as Brush ?? Brushes.Lime;
        private static readonly Brush OffLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOffColor"] as Brush ?? Brushes.DarkGray;
        private static readonly Brush ReadyLampColor = Application.Current.Resources.MergedDictionaries[0]["LampReadyColor"] as Brush ?? Brushes.Yellow;
        private static readonly Brush RunLampColor = Application.Current.Resources.MergedDictionaries[0]["LampRunolor"] as Brush ?? Brushes.Lime;
        private static readonly Brush FaultLampColor = Application.Current.Resources.MergedDictionaries[0]["LampFaultColor"] as Brush ?? Brushes.Red;

        private static readonly Brush InActiveSignalTowerRed = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerRed"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xff, 0xa0, 0xa0));
        private static readonly Brush InActiveSignalTowerYellow = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerYellow"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xC5));
        private static readonly Brush InActiveSignalTowerGreen = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerGreen"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xcd, 0xf5, 0xdd));
        private static readonly Brush InActiveSignalTowerBlue = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerBlue"] as Brush ?? new SolidColorBrush(Color.FromRgb(0x86, 0xCE, 0xFA));
        private static readonly Brush InActiveSignalTowerWhite = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerWhite"] as Brush ?? Brushes.LightGray;
        private static readonly Brush ActiveSignalTowerRed = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerRed"] as Brush ?? Brushes.Red;
        private static readonly Brush ActiveSignalTowerYellow = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerYellow"] as Brush ?? Brushes.Yellow;
        private static readonly Brush ActiveSignalTowerGreen = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerGreen"] as Brush ?? Brushes.Green;
        private static readonly Brush ActiveSignalTowerBlue = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerBlue"] as Brush ?? Brushes.Blue;
        private static readonly Brush ActiveSignalTowerWhite = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerWhite"] as Brush ?? Brushes.White;

        private static readonly Brush PLCConnectedFontColor = Application.Current.Resources.MergedDictionaries[0]["Sapphire_Blue"] as Brush ?? new SolidColorBrush(Color.FromRgb(0x60, 0xCD, 0xFF));
        private static readonly Brush PLCDisconnectedFontColor = Application.Current.Resources.MergedDictionaries[0]["Alert_Red_02"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xEC, 0x3D, 0x3F));

        private static readonly string BuzzerOnPath = "/Resources/icons/icon=buzzeron.png";
        private static readonly string BuzzerOffPath = "/Resources/icons/icon=buzzeroff.png";

        private string Gas1 = Util.GetGasDeviceName("Gas1") ?? "";
        private string Gas2 = Util.GetGasDeviceName("Gas2") ?? "";

        [ObservableProperty]
        private string _showerHeadTemp = "";
        [ObservableProperty]
        private string _inductionCoilTemp = "";

        [ObservableProperty]
        private Brush _maintenanceKeyLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _inductionHeaterLampColor = ReadyLampColor;
        [ObservableProperty]
        private Brush _cleanDryAirLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _doorReactorCabinetLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _doorPowerDistributeCabinetLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _doorGasDeliveryCabinetLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _coolingWaterLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _susceptorMotorLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _vacuumPumpLampColor = ReadyLampColor;
        [ObservableProperty]
        private Brush _dorVacuumStateLampColor = ReadyLampColor;
        [ObservableProperty]
        private Brush _tempControllerAlarmLampColor = OnLampColor;

        [ObservableProperty]
        private Brush _signalTowerRed = Brushes.Transparent;
        [ObservableProperty]
        private Brush _signalTowerYellow = Brushes.Transparent;
        [ObservableProperty]
        private Brush _signalTowerGreen = Brushes.Transparent;
        [ObservableProperty]
        private Brush _signalTowerBlue = Brushes.Transparent;
        [ObservableProperty]
        private Brush _signalTowerWhite = Brushes.Transparent;

        [ObservableProperty]
        private Brush _gasPressureGas2StateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _gasPressureGas1StateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _gasPressureGas3StateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _gasPressureGas4StateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _recipeStartStateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _reactorOpenStateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _heaterTurnOnStateColor = Brushes.Transparent;
        [ObservableProperty]
        private Brush _pumpTurnOnStateColor = Brushes.Transparent;

        [ObservableProperty]
        private int _lineHeater1;
        [ObservableProperty]
        private int _lineHeater2;
        [ObservableProperty]
        private int _lineHeater3;
        [ObservableProperty]
        private int _lineHeater4;
        [ObservableProperty]
        private int _lineHeater5;
        [ObservableProperty]
        private int _lineHeater6;
        [ObservableProperty]
        private int _lineHeater7;
        [ObservableProperty]
        private int _lineHeater8;

        [ObservableProperty]
        private string _pLCAddressText = AmsNetId.Local.ToString();
        [ObservableProperty]
        private string _pLCConnectionStatus = "Diconnected";
        [ObservableProperty]
        private Brush _pLCConnectionStatusColor = PLCDisconnectedFontColor;
        [ObservableProperty]
        private string _buzzerImage = BuzzerOffPath;

        [ObservableProperty]
        private SourceStatusViewModel _currentSourceStatusViewModel;

        private readonly CoolingWaterValueSubscriber showerHeaderTempSubscriber;
        private readonly CoolingWaterValueSubscriber inductionCoilTempSubscriber;
        private readonly HardWiringInterlockStateSubscriber hardWiringInterlockStateSubscriber;
        private readonly MainViewTabIndexChagedSubscriber mainViewTabIndexChagedSubscriber;
        private readonly SignalTowerStateSubscriber signalTowerStateSubscriber;
        private readonly LineHeaterTemperatureSubscriber lineHeaterTemperatureSubscriber;
        private readonly ResetCurrentRecipeSubscriber resetCurrentRecipeSubscriber;
        private readonly LogicalInterlockSubscriber logicalInterlockSubscriber;
        private readonly GasIOLabelSubscriber gasIOLabelSubscriber;
        private readonly PLCConnectionStateSubscriber plcConnectionStateSubscriber;
    }
}
