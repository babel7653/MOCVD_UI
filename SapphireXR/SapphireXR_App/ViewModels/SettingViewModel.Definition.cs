using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        internal class IOStateListSubscriber : IObserver<BitArray>
        {
            public IOStateListSubscriber(SettingViewModel vm)
            {
                settingViewModel = vm;
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
                settingViewModel.updateIOState(value);
            }

            private SettingViewModel settingViewModel;
        }

        private class AppClosingSubscriber : IObserver<bool>
        {
            public AppClosingSubscriber(SettingViewModel vm)
            {
                settingViewModel = vm;
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
                settingViewModel.AlarmSettingSave();
            }

            private SettingViewModel settingViewModel;
        }
       
        public partial class CheckAllColumnViewModel<T>: ObservableObject where T : WarningAlarmDevice
        {
            public CheckAllColumnViewModel(List<T>? list, PLCService.TriggerType alarmOrWarning)
            {
                ioList = list;
                doToggleCheck = (alarmOrWarning == PLCService.TriggerType.Alarm) ? setAllAlarmCheck : setAllWarningCheck;
                doAllChecked = (alarmOrWarning == PLCService.TriggerType.Alarm) ? allAlarmChecked : allWarningChecked;
                PropertyChanged += (sender, args) =>
                {
                    switch (args.PropertyName)
                    {
                        case nameof(ShowGuidePlaceHolderCheckBox):
                            ShowCheckBox = (ShowGuidePlaceHolderCheckBox == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden;
                            break;
                    }
                };
            }

            [RelayCommand]
            private void ToggleCheck()
            {
                doToggleCheck();
                HideGuidePlaceHolder();
            }

            private void setAllChecked(Action<T> forEach)
            {
                if (ioList != null)
                {
                    foreach (T io in ioList)
                    {
                        forEach(io);
                    }
                }
            }

            private void setAllWarningCheck()
            {
                setAllChecked((T io) => io.WarningSet = IsPlaceHolderCheck);
                IsPlaceHolderCheck = !IsPlaceHolderCheck;
            }

            private void setAllAlarmCheck()
            {
                setAllChecked((T io) => io.AlarmSet = IsPlaceHolderCheck);
                IsPlaceHolderCheck = !IsPlaceHolderCheck;
            }

            private bool allChecked(Func<T, bool> predicate)
            {
                if (ioList != null)
                {
                    return ioList.All(predicate);
                }
                {
                    return false;
                }
            }

            private bool allWarningChecked()
            {
               return allChecked(io => io.WarningSet == true);
            }

            private bool allAlarmChecked()
            {
                return allChecked(io => io.AlarmSet == true);
            }

            [RelayCommand]
            private void HideGuidePlaceHolder()
            {
                ShowGuidePlaceHolderCheckBox = Visibility.Hidden;
            }

            [RelayCommand]
            private void ShowGuidePlaceHolder()
            {
                IsPlaceHolderCheck = !doAllChecked();
                ShowGuidePlaceHolderCheckBox = Visibility.Visible;
            }

            [ObservableProperty]
            private Visibility _showGuidePlaceHolderCheckBox = Visibility.Hidden;
            [ObservableProperty]
            private Visibility _showCheckBox = Visibility.Visible;
            [ObservableProperty]
            private bool _isPlaceHolderCheck = false;

            private List<T>? ioList;
            private Action doToggleCheck;
            private Func<bool> doAllChecked;
        }

        public partial class IOSetting : ObservableObject
        {
            required public string Name { get; set; } = "";
            [ObservableProperty]
            private bool _onOff;
        }

        public static readonly string DevceIOSettingFilePath = Util.GetResourceAbsoluteFilePath("/Configurations/DeviceIO.json");
        public static readonly string PrecursorSourceMonitorLabelSettingFilePath = Util.GetResourceAbsoluteFilePath("/Configurations/PrecursorSourceMonitorLabel.json");

        public static Dictionary<string, AnalogDeviceIO> dAnalogDeviceIO = [];
        public static Dictionary<string, SwitchDI>? dSwitchDI = [];
        public static List<Device> GasIO { get; set; } = CreateDefaultGasIO();
        public static List<Device> ValveDeviceIO { get; set; } = [];
        public static Dictionary<string, string>? dPreSet { get; set; } = [];
        public static Dictionary<string, InterLockA>? dInterLockA { get; set; } = [];
        public static Dictionary<string, InterLockD>? dInterLockD { get; set; } = [];

        public float AlarmDeviation { get => AlarmDeviationValue; set => SetProperty(ref AlarmDeviationValue, value); }
        public float WarningDeviation { get => WarningDeviationValue; set => SetProperty(ref WarningDeviationValue, value); }
        public float AnalogDeviceDelayTime { get => AnalogDeviceDelayTimeValue; set => SetProperty(ref AnalogDeviceDelayTimeValue, value); }
        public float DigitalDeviceDelayTime { get => DigitalDeviceDelayTimeValue; set => SetProperty(ref DigitalDeviceDelayTimeValue, value); }

        public List<AnalogDeviceIO>? lAnalogDeviceIO { get; set; } = [];
        public List<SwitchDI>? lSwitchDI { get; set; } = [];

        [ObservableProperty]
        public IList<IOSetting> _iOList;

        private static float AlarmDeviationValue;
        private static float WarningDeviationValue;
        private static float AnalogDeviceDelayTimeValue;
        private static float DigitalDeviceDelayTimeValue;

        public CheckAllColumnViewModel<AnalogDeviceIO> AnalogWarningCheckAllColumnViewModel { get; set; }
        public CheckAllColumnViewModel<AnalogDeviceIO> AnalogAlarmCheckAllColumnViewModel { get; set; }
        public CheckAllColumnViewModel<SwitchDI> DigitalWarningCheckAllColumnViewModel { get; set; }
        public CheckAllColumnViewModel<SwitchDI> DigitalAlarmCheckAllColumnViewModel { get; set; }


        public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
        public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);

        [ObservableProperty]
        private string? _logIntervalInRecipeRun;

        [ObservableProperty]
        private bool? _inductionHeaterPowerOnOff = null;
        [ObservableProperty]
        private bool? _thermalBathPowerOnOff = null;
        [ObservableProperty]
        private bool? _vaccumPumpPowerOnOff = null;
        [ObservableProperty]
        private bool? _lineHeaterPowerOnOff = null;
        [ObservableProperty]
        private bool _isAnalogAlarmHighlight = false;

        private AppClosingSubscriber appClosingSubscriber;
        private IOStateListSubscriber iOStateListSubscriber;

        private static ObservableManager<(string, string)>.Publisher GasIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("GasIOLabelChanged");
        private static ObservableManager<(string, string)>.Publisher ValveIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("ValveIOLabelChanged");
        private static ObservableManager<(string, string)>.Publisher AnalogIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("AnalogIOLabelChanged");
        
        public static readonly Dictionary<string, string> AnalogDeviceIDShortNameMap = new Dictionary<string, string>
        {
            { "MFC01", "M01" }, { "MFC02", "M02" }, { "MFC03", "M03"  }, { "MFC04", "M04"  }, { "MFC05", "M05" },
            { "MFC06", "M06" }, { "MFC07", "M07" }, { "MFC08", "M08" }, { "MFC09", "M09" }, { "MFC10", "M10" },
            { "MFC11", "M11" }, { "MFC12", "M12" }, { "MFC13", "M13" }, {  "MFC14", "M14" }, { "MFC15", "M15" },
            { "MFC16", "M16" }, { "MFC17", "M17" }, { "MFC18", "M18" }, { "MFC19", "M19"  },
            { "EPC01", "E01" },  { "EPC02", "E02" }, { "EPC03", "E03" }, { "EPC04", "E04" }, { "EPC05", "E05" },
            { "EPC06", "E06" }, { "EPC07", "E07" }, { "Temperature", "R01"  }, { "Pressure", "R02"  }, { "Rotation", "R03"  }
        };
        public static readonly Dictionary<string, string> AnalogDeviceIDNameMap = new Dictionary<string, string>
        {
            { "M01", "MFC01" }, { "M02", "MFC02"  }, { "M03", "MFC03"   }, { "M04", "MFC04"  }, {  "M05", "MFC05" }, { "M06", "MFC06"  }, { "M07", "MFC07"  }, { "M08", "MFC08" },
            { "M09", "MFC09"  }, { "M10", "MFC10"  }, { "M11", "MFC11"  }, { "M12", "MFC12"  },  { "M13", "MFC13"  }, {  "M14", "MFC14"  }, { "M15", "MFC15" },
            { "M16", "MFC16" }, { "M17", "MFC17" }, { "M18", "MFC18" }, { "M19", "MFC19" }, { "E01", "EPC01" },  { "E02", "EPC02"  }, { "E03", "EPC03" }, { "E04", "EPC04" },
            { "E05", "EPC05" }, { "E06", "EPC06" }, { "E07", "EPC07"  }, { "R01", "Temperature"  }, { "R02", "Pressure"  }, { "R03", "Rotation"  }
        };
        private static readonly Dictionary<string, PLCService.InterlockEnableSetting> InterlockSettingNameToPLCInterlockEnableSettingEnum = new ()
        {
            { "CanOpenSusceptorTemperature", PLCService.InterlockEnableSetting.CanOpenSusceptorTemperature },
            { "CanOpenReactorPressure", PLCService.InterlockEnableSetting.CanOpenReactorPressure },
            { "PressureLimit", PLCService.InterlockEnableSetting.PressureLimit },
            { "ReTryCount", PLCService.InterlockEnableSetting.RetryCount }
        };
        private static readonly Dictionary<string, PLCService.InterlockValueSetting> InterlockSettingNameToPLCInterlockValueSettingEnum = new()
        {
            { "ProcessGasPressureAlarm", PLCService.InterlockValueSetting.ProcessGasPressureAlarm },
            { "ProcessGasPressureWarning", PLCService.InterlockValueSetting.ProcessGasPressureWarning },
            { "CWTempSHAlarm", PLCService.InterlockValueSetting.CWTempSHAlarm },
            { "CWTempSHWarning", PLCService.InterlockValueSetting.CWTempSHWarning },
            { "CWTempCoilAlarm", PLCService.InterlockValueSetting.CWTempCoilAlarm },
            { "CWTempCoilWarning", PLCService.InterlockValueSetting.CWTempCoilWarning },
             { "SusceptorOverTemperature", PLCService.InterlockValueSetting.SusceptorOverTemperature },
            { "ReactorOverPressure", PLCService.InterlockValueSetting.ReactorOverPressure },
            { "CanOpenSusceptorTemperature", PLCService.InterlockValueSetting.CanOpenSusceptorTemperature },
            { "CanOpenReactorPressure", PLCService.InterlockValueSetting.CanOpenReactorPressure },
            { "PressureLimit", PLCService.InterlockValueSetting.PressureLimit },
            { "ReTryCount", PLCService.InterlockValueSetting.RetryCount }
        };
    }
}
