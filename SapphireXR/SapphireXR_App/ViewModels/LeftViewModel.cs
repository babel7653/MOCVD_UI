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
using TwinCAT.Ads;
using System.Security.Policy;
using System.Reactive;

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

                leftViewModel.MaintenanceKeyLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.MaintenanceKey]);
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
                        leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentPLCStateViewModel();
                        break;

                    case 2:
                        leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentRecipeStepViewModel();
                        break;
                }
            }

            private LeftViewModel leftViewModel;
        }

        private class SignalTowerStateSubscriber : IObserver<BitArray>
        {
            internal SignalTowerStateSubscriber(LeftViewModel vm)
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

            void IObserver<BitArray>.OnNext(BitArray ioList)
            {
                setIfChange(ioList[(int)PLCService.IOListIndex.SingalTower_RED], (bool state) => { if (state == true) { leftViewModel.SignalTowerRed = ActiveSignalTowerRed;  } else { leftViewModel.SignalTowerRed = InActiveSignalTowerRed; } }, ref signalTowerRed);
                setIfChange(ioList[(int)PLCService.IOListIndex.SingalTower_YELLOW], (bool state) => { if (state == true) { leftViewModel.SignalTowerYellow = ActiveSignalTowerYellow; } else { leftViewModel.SignalTowerYellow = InActiveSignalTowerYellow; } }, ref signalTowerYellow);
                setIfChange(ioList[(int)PLCService.IOListIndex.SingalTower_GREEN], (bool state) => { if (state == true) { leftViewModel.SignalTowerGreen = ActiveSignalTowerGreen; } else { leftViewModel.SignalTowerGreen = InActiveSignalTowerGreen; } }, ref signalTowerGreen);
                setIfChange(ioList[(int)PLCService.IOListIndex.SingalTower_BLUE], (bool state) => { if (state == true) { leftViewModel.SignalTowerBlue = ActiveSignalTowerBlue; } else { leftViewModel.SignalTowerBlue = InActiveSignalTowerBlue; } }, ref signalTowerBlue);
                setIfChange(ioList[(int)PLCService.IOListIndex.SingalTower_WHITE], (bool state) => { if (state == true) { leftViewModel.SignalTowerWhite = ActiveSignalTowerWhite; } else { leftViewModel.SignalTowerWhite = InActiveSignalTowerWhite; } }, ref signalTowerWhite);
            }

            void setIfChange(bool ioState,  Action<bool> onChanged, ref bool? signalTowerState)
            {
                if (signalTowerState != ioState)
                {
                    onChanged(ioState);
                    signalTowerState = ioState;
                }
            }

            private LeftViewModel leftViewModel;
            private bool? signalTowerRed = null;
            private bool? signalTowerYellow = null;
            private bool? signalTowerGreen = null;
            private bool? signalTowerBlue = null;
            private bool? signalTowerWhite = null;
            private bool? signalTowerBuzzwer = null;
        }

        private class LineHeaterTemperatureSubscriber: IObserver<float[]>
        {
            internal LineHeaterTemperatureSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<float[]>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float[]>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void setIfChanged(float newValue, Action<float> onChanged, ref float? lineHeaterTemperature)
            {
                if(lineHeaterTemperature != newValue)
                {
                    onChanged(newValue);
                    lineHeaterTemperature = newValue;
                }
            }

            void IObserver<float[]>.OnNext(float[] currentLineHeaterTemperatures)
            {
                setIfChanged(currentLineHeaterTemperatures[0], (float newTemperature) => { leftViewModel.LineHeater1 = (int)newTemperature; }, ref lineHeater1Temperatures);
                setIfChanged(currentLineHeaterTemperatures[1], (float newTemperature) => { leftViewModel.LineHeater2 = (int)newTemperature; }, ref lineHeater2Temperatures);
                setIfChanged(currentLineHeaterTemperatures[2], (float newTemperature) => { leftViewModel.LineHeater3 = (int)newTemperature; }, ref lineHeater3Temperatures);
                setIfChanged(currentLineHeaterTemperatures[3], (float newTemperature) => { leftViewModel.LineHeater4 = (int)newTemperature; }, ref lineHeater4Temperatures);
                setIfChanged(currentLineHeaterTemperatures[4], (float newTemperature) => { leftViewModel.LineHeater5 = (int)newTemperature; }, ref lineHeater5Temperatures);
                setIfChanged(currentLineHeaterTemperatures[5], (float newTemperature) => { leftViewModel.LineHeater6 = (int)newTemperature; }, ref lineHeater6Temperatures);
                setIfChanged(currentLineHeaterTemperatures[6], (float newTemperature) => { leftViewModel.LineHeater7 = (int)newTemperature; }, ref lineHeater7Temperatures);
                setIfChanged(currentLineHeaterTemperatures[7], (float newTemperature) => { leftViewModel.LineHeater8 = (int)newTemperature; }, ref lineHeater8Temperatures);
            }

            LeftViewModel leftViewModel;
            float? lineHeater1Temperatures;
            float? lineHeater2Temperatures;
            float? lineHeater3Temperatures;
            float? lineHeater4Temperatures;
            float? lineHeater5Temperatures;
            float? lineHeater6Temperatures;
            float? lineHeater7Temperatures;
            float? lineHeater8Temperatures;
        }

        private class ResetCurrentRecipeSubscriber : IObserver<bool>
        {
            internal ResetCurrentRecipeSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
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
                if (leftViewModel.CurrentSourceStatusViewModel is SourceStatusFromCurrentRecipeStepViewModel)
                {
                    leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentRecipeStepViewModel();
                }
            }

            private LeftViewModel leftViewModel;
        }

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
            public SourceStatusFromCurrentPLCStateViewModel() : base("CurrentPLCState") { }
        }

        public class SourceStatusFromCurrentRecipeStepViewModel: SourceStatusViewModel
        {
            public SourceStatusFromCurrentRecipeStepViewModel() :base("CurrentRecipeStep") {  }
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
          
            CurrentSourceStatusViewModel = new SourceStatusFromCurrentPLCStateViewModel();
            PropertyChanging += (object? sender, PropertyChangingEventArgs args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(CurrentSourceStatusViewModel):
                        CurrentSourceStatusViewModel.dispose();
                        break;
                }
            };
        }

        private static Brush OnLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOnColor"] as Brush ?? Brushes.Lime;
        private static Brush OffLampColor = Application.Current.Resources.MergedDictionaries[0]["LampOffColor"] as Brush ?? Brushes.Red;
        private static Brush ReadyLampColor = Application.Current.Resources.MergedDictionaries[0]["LampReadyColor"] as Brush ?? Brushes.Yellow;

        private static Brush InActiveSignalTowerRed = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerRed"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xff, 0xa0, 0xa0));
        private static Brush InActiveSignalTowerYellow = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerYellow"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xC5));
        private static Brush InActiveSignalTowerGreen = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerGreen"] as Brush ?? new SolidColorBrush(Color.FromRgb(0xcd, 0xf5, 0xdd));
        private static Brush InActiveSignalTowerBlue = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerBlue"] as Brush ?? new SolidColorBrush(Color.FromRgb(0x86, 0xCE, 0xFA));
        private static Brush InActiveSignalTowerWhite = Application.Current.Resources.MergedDictionaries[0]["InActiveSignalTowerWhite"] as Brush ?? Brushes.LightGray;
        private static Brush ActiveSignalTowerRed = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerRed"] as Brush ?? Brushes.Red;
        private static Brush ActiveSignalTowerYellow = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerYellow"] as Brush ?? Brushes.Yellow;
        private static Brush ActiveSignalTowerGreen = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerGreen"] as Brush ?? Brushes.Green;
        private static Brush ActiveSignalTowerBlue = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerBlue"] as Brush ?? Brushes.Blue;
        private static Brush ActiveSignalTowerWhite = Application.Current.Resources.MergedDictionaries[0]["ActiveSignalTowerWhite"] as Brush ?? Brushes.White;

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
        private string _pLCAddressText = "PLC Address : " + AmsNetId.Local.ToString();
        [ObservableProperty]
        private string _pLCConnectionStatus = "PLC Connection: Connected";

        [ObservableProperty]
        private SourceStatusViewModel _currentSourceStatusViewModel;

        private CoolingWaterValueSubscriber showerHeaderTempSubscriber;
        private CoolingWaterValueSubscriber inductionCoilTempSubscriber;
        private HardWiringInterlockStateSubscriber hardWiringInterlockStateSubscriber;
        private MainViewTabIndexChagedSubscriber mainViewTabIndexChagedSubscriber;
        private SignalTowerStateSubscriber signalTowerStateSubscriber;
        private LineHeaterTemperatureSubscriber lineHeaterTemperatureSubscriber;
        private readonly ResetCurrentRecipeSubscriber resetCurrentRecipeSubscriber;
    }
}
