using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Configuration;

namespace SapphireXR_App.ViewModels
{
    public partial class LeftViewModel : ObservableObject
    {
        internal class ValveStateSubscriber : IObserver<bool>
        {
            internal ValveStateSubscriber(string associatedStateIDStr, LeftViewModel vm)
            {
                associatedStateID = associatedStateIDStr;
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
                
            }

            private string associatedStateID;
            private LeftViewModel leftViewModel;
        }

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

        public LeftViewModel()
        {
            ObservableManager<float>.Subscribe("MonitoringPresentValue.ShowerHeadTemp.CurrentValue", showerHeaderTempSubscriber = new CoolingWaterValueSubscriber("ShowerHeadTemp", this));
            ObservableManager<float>.Subscribe("MonitoringPresentValue.InductionCoilTemp.CurrentValue", inductionCoilTempSubscriber = new CoolingWaterValueSubscriber("InductionCoilTemp", this));
            ObservableManager<BitArray>.Subscribe("HardWiringInterlockState", hardWiringInterlockStateSubscriber = new HardWiringInterlockStateSubscriber(this));

            valveStateSubscribers =
            [
                new ("NH3_1Carrier", this), new ("NH3_1Source", this), new ("NH3_1Vent", this), new ("NH3_2Carrier", this), new ("NH3_2Source", this), new ("NH3_2Vent", this),
                new ("SiH4Carrier}", this), new ("SiH4Source}", this), new ("SiH4Vent}", this), new ("TEBCarrier}", this), new ("TEBSource}", this), new ("TEBVent}", this),
                new ("TMAlCarrier}", this), new ("TMAlSource}", this), new ("TMAlVent}", this), new ("TMInCarrier}", this), new ("TMInSource}", this), new ("TMInVent}", this),
                new ("TMGaCarrier}", this), new ("TMGaSource}", this), new ("TMGaVent}", this), new ("DTMGaCarrier}", this), new ("DTMGaSource}", this), new ("DTMGaVent}", this)
            ];
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
        private string _nH3_1Carrier= "";
        [ObservableProperty]
        private string _nH3_1Source= "";
        [ObservableProperty]
        private string _nH3_1Vent= "";
        [ObservableProperty]
        private string _nH3_2Carrier= "";
        [ObservableProperty]
        private string _nH3_2Source= "";
        [ObservableProperty]
        private string _nH3_2Vent= "";
        [ObservableProperty]
        private string _siH4Carrier= "";
        [ObservableProperty]
        private string _siH4Source= "";
        [ObservableProperty]
        private string _siH4Vent= "";
        [ObservableProperty]
        private string _tEBCarrier= "";
        [ObservableProperty]
        private string _tEBSource= "";
        [ObservableProperty]
        private string _tEBVent= "";
        [ObservableProperty]
        private string _tMAlCarrier= "";
        [ObservableProperty]
        private string _tMAlSource= "";
        [ObservableProperty]
        private string _tMAlVent= "";
        [ObservableProperty]
        private string _tMInCarrier= "";
        [ObservableProperty]
        private string _tMInSource= "";
        [ObservableProperty]
        private string _tMInVent= "";
        [ObservableProperty]
        private string _tMGaCarrier= "";
        [ObservableProperty]
        private string _tMGaSource= "";
        [ObservableProperty]
        private string _tMGaVent= "";
        [ObservableProperty]
        private string _dTMGaCarrier= "";
        [ObservableProperty]
        private string _dTMGaSource= "";
        [ObservableProperty]
        private string _dTMGaVent= "";
                  
        private CoolingWaterValueSubscriber showerHeaderTempSubscriber;
        private CoolingWaterValueSubscriber inductionCoilTempSubscriber;
        private HardWiringInterlockStateSubscriber hardWiringInterlockStateSubscriber;

        private ValveStateSubscriber[] valveStateSubscribers;
    }
}
