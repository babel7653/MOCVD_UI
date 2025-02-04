using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Configuration;
using static SapphireXR_App.ViewModels.LeftViewModel;
using System.ComponentModel;
using SapphireXR_App.Controls;
using static SapphireXR_App.ViewModels.RecipeEditViewModel.RecipeStateUpader;

namespace SapphireXR_App.ViewModels
{
    public partial class LeftViewModel : ObservableObject
    {
        internal class CoolingWaterValueSubscriber : IObserver<float>
        {
            internal CoolingWaterValueSubscriber(string coolingWaterIDStr, LeftViewModel vm)
            {
                coolingWaterID = coolingWaterIDStr;
                leftViewModel = vm;
            }

            void IObserver<float>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnNext(float value)
            {
                switch(coolingWaterID)
                {
                    case "ShowerHeadTemp":
                        leftViewModel.ShowerHeadTemp = ((int)value).ToString();
                    break;

                    case "InductionCoilTemp":
                        leftViewModel.InductionCoilTemp = ((int)value).ToString();
                        break;
                }
            }

            private string coolingWaterID;
            private LeftViewModel leftViewModel;
        }

        internal class HardWiringInterlockStateSubscriber : IObserver<BitArray>
        {
            public HardWiringInterlockStateSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<BitArray>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnNext(BitArray value)
            {
                var convertOnOffStateColor = (bool bit) => bit == true ? OnLampColor: OffLampColor;
                var convertThreeStateColor = (BitArray value, int startIndex) =>
                {
                    if (value[startIndex] == true)
                    {
                        return ReadyLampColor;
                    }

                    if (value[startIndex + 1] == true)
                    {
                        return OnLampColor;
                    }

                    if (value[startIndex + 2] == true)
                    {
                        return OffLampColor;
                    }

                    return null;
                };

                leftViewModel.MaintenanceKeyLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.MaintainenceKey]);
                leftViewModel.DoorReactorCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorReactorCabinet]);
                leftViewModel.DoorGasDeliveryCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorGasDeliveryCabinet]);
                leftViewModel.DoorPowerDistributeCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorPowerDistributeCabinet]);
                leftViewModel.CleanDryAirLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.CleanDryAir]);
                leftViewModel.CoolingWaterLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.CoolingWater]);

                leftViewModel.InductionHeaterLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.InductionHeaterReady) ?? leftViewModel.InductionHeaterLampColor;
                leftViewModel.SusceptorMotorLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.SusceptorMotorStop) ?? leftViewModel.SusceptorMotorLampColor;
                leftViewModel.VacuumPumpLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.VacuumPumpWarning) ?? leftViewModel.VacuumPumpLampColor;
            }

            LeftViewModel leftViewModel;
        }

        internal class MainViewTabIndexChagedSubscriber : IObserver<int>
        {
            internal MainViewTabIndexChagedSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }
            void IObserver<int>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnNext(int tabIndex)
            {
                switch(tabIndex)
                {
                    case 0:
                    case 1:
                        leftViewModel.CurrentSourceStatusViewModel = leftViewModel.sourceStatusFromCurrentPLCStateViewModel;
                        break;

                    case 2:
                        leftViewModel.CurrentSourceStatusViewModel = leftViewModel.sourceStatusFromCurrentRecipeStepViewModel;
                        break;
                }
            }

            private LeftViewModel leftViewModel;
        }

        public abstract partial class SourceStatusViewModel : ObservableObject
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

            public SourceStatusViewModel(string valveStateSubscsribePostfixStr)
            {
                valveStateSubscsribePostfix = valveStateSubscsribePostfixStr;
                valveStateSubscrbers = [
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { SiH4Carrier = NH3_2Carrier = NH3_1Carrier = "H2"; SiH4CarrierColor = NH3_2CarrierColor = NH3_1CarrierColor = H2Color;  } 
                        else { SiH4Carrier = NH3_2Carrier = NH3_1Carrier = "N2"; SiH4CarrierColor = NH3_2CarrierColor = NH3_1CarrierColor = DefaultColor; }  }, "V01"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TEBCarrier = "H2"; TEBCarrierColor = H2Color; } else { TEBCarrier = "N2"; TEBCarrierColor = DefaultColor; }  }, "V05"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMAlCarrier = "H2"; TMAlCarrierColor = H2Color; } else { TMAlCarrier = "N2"; TMAlCarrierColor = DefaultColor; } }, "V08"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMInCarrier = "H2";  TMInCarrierColor = H2Color;} else { TMInCarrier = "N2";  TMInCarrierColor = DefaultColor;} }, "V11"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMGaCarrier = "H2";  TMGaCarrierColor = H2Color;} else { TMGaCarrier = "N2";  TMGaCarrierColor =DefaultColor;} }, "V14"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { DTMGaCarrier = "H2";  DTMGaCarrierColor = H2Color;} else { DTMGaCarrier = "N2";  DTMGaCarrierColor = DefaultColor;}  }, "V17"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { Cp2MgCarrier = "H2";  Cp2MgCarrierColor = H2Color;} else { Cp2MgCarrier = "N2";  Cp2MgCarrierColor = DefaultColor;} }, "V20"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { NH3_1Source = NH3_2Source = "On";  NH3_1SourceColor = NH3_2SourceColor = OnColor; } else { NH3_1Source = NH3_2Source = "Off";  NH3_1SourceColor = NH3_2SourceColor = DefaultColor;} }, "V04"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { SiH4Source = "On";  SiH4SourceColor = OnColor;} else { SiH4Source = "Off";  SiH4SourceColor =DefaultColor;} }, "V03"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TEBSource = "On";  TEBSourceColor =OnColor;} else { TEBSource = "Off";  TEBSourceColor = DefaultColor;}  }, "V07"),
                    new ValveStateSubscriber(this, (bool nextValveState) => { if (nextValveState == true) { TMAlSource = TMInSource = "On";  TMAlSourceColor = TMInSourceColor = OnColor;} else { TMAlSource = TMInSource = "Off";  TMAlSourceColor = TMInSourceColor = DefaultColor;} }, "V10"),
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
            }

            public void active()
            {
                foreach(ValveStateSubscriber valveStateSubscriber in valveStateSubscrbers)
                {
                    unsubscribers.Add(ObservableManager<bool>.Subscribe("Valve.OnOff." + valveStateSubscriber.valveID + "." + valveStateSubscsribePostfix, valveStateSubscriber));
                }
            }
            public void inactive()
            {
                foreach(IDisposable unsubscriber in unsubscribers)
                {
                    unsubscriber.Dispose();
                }
                unsubscribers.Clear();
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
        }

        public class SourceStatusFromCurrentPLCStateViewModel : SourceStatusViewModel
        {
            public SourceStatusFromCurrentPLCStateViewModel() : base("CurrentPLCState") { }
        }

        public class SourceStatusFromCurrentRecipeStepViewModel: SourceStatusViewModel, IObserver<bool>
        {
            public SourceStatusFromCurrentRecipeStepViewModel() :base("CurrentRecipeStep") 
            {
                ObservableManager<bool>.Subscribe("Reset.CurrentRecipeStep", this);
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
                NH3_1Carrier = NH3_2Carrier = SiH4Carrier = TEBCarrier = TMAlCarrier = TMInCarrier = TMGaCarrier = DTMGaCarrier = Cp2MgCarrier = "";
                NH3_1Source = NH3_2Source = SiH4Source = TEBSource = TMAlSource = TMInSource = TMGaSource = DTMGaSource = Cp2MgSource = "";
                NH3_1Vent = NH3_2Vent = SiH4Vent = TEBVent = TMAlVent = TMInVent = TMGaVent = DTMGaVent = Cp2MgVent = "";
            }
        }

        public LeftViewModel()
        {
            ObservableManager<float>.Subscribe("MonitoringPresentValue.ShowerHeadTemp.CurrentValue", showerHeaderTempSubscriber = new CoolingWaterValueSubscriber("ShowerHeadTemp", this));
            ObservableManager<float>.Subscribe("MonitoringPresentValue.InductionCoilTemp.CurrentValue", inductionCoilTempSubscriber = new CoolingWaterValueSubscriber("InductionCoilTemp", this));
            ObservableManager<BitArray>.Subscribe("HardWiringInterlockState", hardWiringInterlockStateSubscriber = new HardWiringInterlockStateSubscriber(this));
            ObservableManager<int>.Subscribe("MainView.SelectedTabIndex", mainViewTabIndexChagedSubscriber = new MainViewTabIndexChagedSubscriber(this));

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(CurrentSourceStatusViewModel):
                        if(CurrentSourceStatusViewModel == sourceStatusFromCurrentRecipeStepViewModel)
                        {
                            sourceStatusFromCurrentPLCStateViewModel.inactive();
                        }
                        else
                            if(CurrentSourceStatusViewModel == sourceStatusFromCurrentPLCStateViewModel)
                            {
                                sourceStatusFromCurrentRecipeStepViewModel.inactive();
                            }
                        CurrentSourceStatusViewModel.active();
                        break;
                }
            };
            CurrentSourceStatusViewModel = sourceStatusFromCurrentPLCStateViewModel;
        }

        private static Brush OnLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOnColor"] as Brush ?? Brushes.Lime;
        private static Brush OffLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOffColor"] as Brush ?? Brushes.Red;
        private static Brush ReadyLampColor = Application.Current.Resources.MergedDictionaries[0]["LampReadyColor"] as Brush ?? Brushes.Yellow;

        [ObservableProperty]
        private string _showerHeadTemp = "";
        [ObservableProperty]
        private string _inductionCoilTemp = "";

        [ObservableProperty]
        private Brush _maintenanceKeyLampColor = OnLampColor;
        [ObservableProperty]
        private Brush _inductionHeaterLampColor = OnLampColor;
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
        private Brush _vacuumPumpLampColor = OnLampColor;

        [ObservableProperty]
        private SourceStatusViewModel _currentSourceStatusViewModel;

        private SourceStatusFromCurrentPLCStateViewModel sourceStatusFromCurrentPLCStateViewModel = new SourceStatusFromCurrentPLCStateViewModel();
        private SourceStatusFromCurrentRecipeStepViewModel sourceStatusFromCurrentRecipeStepViewModel = new SourceStatusFromCurrentRecipeStepViewModel();

        private CoolingWaterValueSubscriber showerHeaderTempSubscriber;
        private CoolingWaterValueSubscriber inductionCoilTempSubscriber;
        private HardWiringInterlockStateSubscriber hardWiringInterlockStateSubscriber;
        private MainViewTabIndexChagedSubscriber mainViewTabIndexChagedSubscriber;
    }
}
