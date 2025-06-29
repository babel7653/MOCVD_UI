using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Collections;
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
        private class ModulePowerStateSubscriber : IObserver<BitArray>
        {
            internal ModulePowerStateSubscriber(SettingViewModel vm)
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
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.InductionHeaterMC], ref prevInpudctionHeaterPowerOn, (bool value)
                    => { settingViewModel.InductionHeaterPowerOn = (value == true ? "On" : "Off"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.ThermalBathMC], ref prevThermalBatchPowerOn, (bool value)
                    => { settingViewModel.ThermalBathPowerOn = (value == true ? "On" : "Off"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.VaccumPumpMC], ref prevVaccumPumpPowerOn, (bool value)
                    => { settingViewModel.VaccumPumpPowerOn = (value == true ? "On" : "Off"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.LineHeaterMC], ref prevLineHeaterPowerOn, (bool value)
                    => { settingViewModel.LineHeaterPowerOn = (value == true ? "On" : "Off"); });
            }

            private SettingViewModel settingViewModel;
            private bool? prevInpudctionHeaterPowerOn = null;
            private bool? prevThermalBatchPowerOn = null;
            private bool? prevVaccumPumpPowerOn = null;
            private bool? prevLineHeaterPowerOn = null;
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

        private class PLCConnectionStateSubscriber : IObserver<PLCConnection>
        {
            public PLCConnectionStateSubscriber(SettingViewModel vm)
            {
                settingViewModel = vm;
            }

            void IObserver<PLCConnection>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnNext(PLCConnection value)
            {
                if (value == PLCConnection.Connected)
                {
                    if (deviceMaxValueWrittenToPLC == false)
                    {
                        PLCService.WriteDeviceMaxValue(settingViewModel.lAnalogDeviceIO);
                        deviceMaxValueWrittenToPLC = true;
                    }
                }
                settingViewModel.ToggleInductionHeaterPowerCommand.NotifyCanExecuteChanged();
                settingViewModel.ToggleThermalBathPowerCommand.NotifyCanExecuteChanged();
                settingViewModel.ToggleVaccumPumpPowerCommand.NotifyCanExecuteChanged();
                settingViewModel.ToggleLineHeaterPowerCommand.NotifyCanExecuteChanged();
            }

            private SettingViewModel settingViewModel;
            private bool deviceMaxValueWrittenToPLC = false;
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
        public static Dictionary<string, GasDO>? dGasDO = [];
        public static List<Device> GasIO { get; set; } = CreateDefaultGasIO();
        public static List<ValveDeviceIO> ValveDeviceIO { get; set; } = [];
        public static Dictionary<string, string>? dPreSet { get; set; } = [];
        public static Dictionary<string, InterLockA>? dInterLockA { get; set; } = [];
        public static Dictionary<string, bool>? dInterLockD { get; set; } = [];
        public List<AnalogDeviceIO>? lAnalogDeviceIO { get; set; } = [];
        public List<SwitchDI>? lSwitchDI { get; set; } = [];
        public List<GasDO>? lGasDO { get; set; } = [];

        [ObservableProperty]
        public IList<IOSetting> _iOList;

        [ObservableProperty]
        private bool _online = false;

        public bool WithoutConnection { get; set; }

        public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
        public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);

        [ObservableProperty]
        private string? _logIntervalInRecipeRun;

        [ObservableProperty]
        private string _inductionHeaterPowerOn = "";
        [ObservableProperty]
        private string _thermalBathPowerOn = "";
        [ObservableProperty]
        private string _vaccumPumpPowerOn = "";
        [ObservableProperty]
        private string _lineHeaterPowerOn = "";

        private ModulePowerStateSubscriber modulePowerStateSubscriber;
        private AppClosingSubscriber appClosingSubscriber;
        private IOStateListSubscriber iOStateListSubscriber;
        private PLCConnectionStateSubscriber plcConnectionStateSubscriber;

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
    }
}
