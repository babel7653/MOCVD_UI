using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using SapphireXR_App.Common;
using System.ComponentModel;
using System.Collections;


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

        public partial class IOSetting: ObservableObject
        {
            required public string Name { get; set; } = "";
            [ObservableProperty]
            private bool _onOff;
        }

        static SettingViewModel()
        {
            var fdevice = File.ReadAllText(DevceIOSettingFilePath);
            JToken? jDeviceInit = JToken.Parse(fdevice);
            if(jDeviceInit == null)
            {
                return;
            }

            var publishDeviceNameChanged = (object? sender, string? propertyName, ObservableManager<(string, string)>.DataIssuer publisher) =>
            {
                if (propertyName == nameof(Device.Name))
                {
                    Device? device = sender as Device;
                    if (device != null && device.ID != null && device.Name != null)
                    {
                        publisher.Issue((device.ID, device.Name));
                    }
                }
            };

            JToken? jAnalogDeviceIO = jDeviceInit["AnalogDeviceIO"];
            if (jAnalogDeviceIO != null)
            {
                var deserialized = JsonConvert.DeserializeObject<Dictionary<string, AnalogDeviceIO>>(jAnalogDeviceIO.ToString());
                if (deserialized != null)
                {
                    dAnalogDeviceIO = deserialized;
                }
            }
            JToken? jSwitchDI = jDeviceInit["SwitchDI"];
            if (jSwitchDI != null)
            {
                dSwitchDI = JsonConvert.DeserializeObject<Dictionary<string, SwitchDI>>(jSwitchDI.ToString());
            }
            JToken? jGasDO = jDeviceInit["GasDO"];
            if (jGasDO != null)
            {
                dGasDO = JsonConvert.DeserializeObject<Dictionary<string, GasDO>>(jGasDO.ToString());
            }
            JToken? jPreSet = jDeviceInit["PreSet"];
            if (jPreSet != null)
            {
                dPreSet = JsonConvert.DeserializeObject<Dictionary<string, string>>(jPreSet.ToString());
            }
            JToken? jInterLockD = jDeviceInit["InterLockD"];
            if (jInterLockD != null)
            {
                dInterLockD = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jInterLockD.ToString());
            }
            JToken? jInterLockA = jDeviceInit["InterLockA"];
            if (jInterLockA != null)
            {
                dInterLockA = JsonConvert.DeserializeObject<Dictionary<string, InterLockA>>(jInterLockA.ToString());
            }
            JToken? jValveDeviceIO = jDeviceInit["ValveDeviceIO"];
            if (jValveDeviceIO != null)
            {
                var deserialized = JsonConvert.DeserializeObject<List<ValveDeviceIO>>(jValveDeviceIO.ToString());
                if(deserialized != null)
                {
                    ValveDeviceIO = deserialized;
                }
                else
                {
                    ValveDeviceIO = CreateDefaultValveDeviceIO();
                }
            }
            else
            {
                ValveDeviceIO = CreateDefaultValveDeviceIO();
            }
            foreach(var io in ValveDeviceIO)
            {
                io.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                {
                    PublishDeviceNameChanged(sender, args.PropertyName, ValveIOLabelChangedPublisher);
                };
            }
            JToken? jGasIO = jDeviceInit["GasIO"];
            if (jGasIO != null)
            {
                var deserialized = JsonConvert.DeserializeObject<List<Device>>(jGasIO.ToString());
                if (deserialized != null)
                {
                    GasIO = deserialized;
                }
            }
            foreach (var io in GasIO)
            {
                io.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                {
                    PublishDeviceNameChanged(sender, args.PropertyName, GasIOLabelChangedPublisher);
                };
            }
        }

        public SettingViewModel()
        {
            AlarmSettingLoad();
            IOList = new List<IOSetting> {
                new() { Name= "Power Reset Switch", OnOff = true },new() { Name= "Cover - Upper Limit", OnOff = true }, new() { Name= "Cover - Lower Limit", OnOff = true },
                new() { Name= "SMPS - 24V 480", OnOff = true }, new() { Name= "SMPS - 24V 72", OnOff = true },  new() { Name= "SMPS - 15V Plus", OnOff = true },
                new() { Name= "SMPS - 15V Minus", OnOff = true },  new() { Name= "CP - Induction Heater", OnOff = true }, new() { Name= "CP - Thermal Bath", OnOff = true },
                new() { Name= "CP - Vacuum Pump", OnOff = true },  new() { Name= "CP - Line Heater", OnOff = true }, new() { Name= "CP - Rotation Motor", OnOff = true },
                new() { Name= "CP - Cover Motor", OnOff = true },  new() { Name= "CP - Throttle Valve", OnOff = true }, new() { Name= "CP - Lamp", OnOff = true },
                new() { Name= "CP - SMPS15CP", OnOff = true },  new() { Name= "Line Heater 1", OnOff = true }, new() { Name= "Line Heater 2", OnOff = true },
                new() { Name= "Line Heater 3", OnOff = true }, new() { Name= "Line Heater 4", OnOff = true },  new() { Name= "Line Heater 5", OnOff = true },
                new() { Name= "Line Heater 6", OnOff = true },  new() { Name= "Line Heater 7", OnOff = true },  new() { Name= "Line Heater 8", OnOff = true },
                new() { Name= "Thermal Bath - Deviation Alaram #1", OnOff = true }, new() { Name= "Thermal Bath - Deviation Alaram #2", OnOff = true },
                new() { Name= "Thermal Bath - Deviation Alaram #3", OnOff = true }, new() { Name= "Thermal Bath - Deviation Alaram #4", OnOff = true },
                new() { Name= "Thermal Bath - Deviation Alaram #5", OnOff = true }, new() { Name= "Thermal Bath - Deviation Alaram #6", OnOff = true },
                new() { Name= "Singal Tower - RED", OnOff = true }, new() { Name= "Singal Tower - YELLOW", OnOff = true }, new() { Name= "Singal Tower - GREEN", OnOff = true },
                new() { Name= "Singal Tower - BLUE", OnOff = true },  new() { Name= "Singal Tower - WHITE", OnOff = true }, new() { Name= "Singal Tower - BUZZWER", OnOff = true }
            };
            ObservableManager<BitArray>.Subscribe("DeviceIOList", iOStateListSubscriber = new IOStateListSubscriber(this));
            ObservableManager<BitArray>.Subscribe("OutputCmd1", modulePowerStateSubscriber = new ModulePowerStateSubscriber(this));
            ObservableManager<bool>.Subscribe("App.Closing", appClosingSubscriber = new AppClosingSubscriber(this));
        }

        private static void PublishDeviceNameChanged(object? sender, string? propertyName, ObservableManager<(string, string)>.DataIssuer publisher)
        {
            if (propertyName == nameof(Device.Name))
            {
                Device? device = sender as Device;
                if (device != null && device.ID != null && device.Name != null)
                {
                    publisher.Issue((device.ID, device.Name));
                }
            }
        }

        public static int? ReadMaxValue(string id)
        {
            if (AnalogDeviceIDShortNameMap.TryGetValue(id, out var shortName) == true)
            {
                var found = dAnalogDeviceIO.Where((KeyValuePair<string, AnalogDeviceIO> analogDeviceIO) => shortName == analogDeviceIO.Key).Select((KeyValuePair<string, AnalogDeviceIO> analogDeviceIO) => analogDeviceIO.Value.MaxValue);
                if(0 < found.Count())
                {
                    return found.First();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void AlarmSettingLoad()
        {
            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(LogIntervalInRecipeRun):
                        if (LogIntervalInRecipeRun != null)
                        {
                            AppSetting.LogIntervalInRecipeRunInMS = int.Parse(LogIntervalInRecipeRun);
                        }
                        break;
                }
            };
            //Json파일 읽기 및 Pars
         
          
            lAnalogDeviceIO = dAnalogDeviceIO?.Values.ToList();
            if (lAnalogDeviceIO != null)
            {
                foreach (var io in lAnalogDeviceIO)
                {
                    io.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                    {
                        PublishDeviceNameChanged(sender, args.PropertyName, AnalogIOLabelChangedPublisher);
                    };
                }
            }
            lSwitchDI = dSwitchDI?.Values.ToList();
            lGasDO = dGasDO?.Values.ToList();
         
            PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
        }

        public void AlarmSettingSave()
        {
            JToken jsonAnalogDeviceIO = JsonConvert.SerializeObject(dAnalogDeviceIO);
            JToken jValveDeviceIO = JsonConvert.SerializeObject(ValveDeviceIO);
            JToken jGasIO = JsonConvert.SerializeObject(GasIO);
            JToken jsonSwitchDI = JsonConvert.SerializeObject(dSwitchDI);
            JToken jsonGasDO = JsonConvert.SerializeObject(dGasDO);
            JToken jPreSet = JsonConvert.SerializeObject(dPreSet);
            JToken jInterLockD = JsonConvert.SerializeObject(dInterLockD);
            JToken jInterLockA = JsonConvert.SerializeObject(dInterLockA);
           

            JObject jDeviceIO = new(
                new JProperty("AnalogDeviceIO", jsonAnalogDeviceIO),
                new JProperty("ValveDeviceIO", jValveDeviceIO),
                new JProperty("GasIO", jGasIO),
                new JProperty("SwitchDI", jsonSwitchDI),
                new JProperty("GasDO", jsonGasDO),
                new JProperty("PreSet", jPreSet),
                new JProperty("InterLockD", jInterLockD),
                new JProperty("InterLockA", jInterLockA)
            );

            if (File.Exists(DevceIOSettingFilePath)) File.Delete(DevceIOSettingFilePath);
            File.WriteAllText(DevceIOSettingFilePath, jDeviceIO.ToString());

            PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
        }

        private void updateIOState(BitArray ioStateList)
        {
            int io = 0;
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.PowerResetSwitch];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Cover_UpperLimit];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Cover_LowerLimit];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SMPS_24V480];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SMPS_24V72];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SMPS_15VPlus];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SMPS_15VMinus];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_InudctionHeater];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_ThermalBath];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_VaccumPump];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_LineHeater];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_RotationMotor];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_CoverMotor];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_ThrottleValve];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_Lamp];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.CP_SM515CP];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader1];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader2];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader3];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader4];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader5];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader6];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader7];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.LineHeader8];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram1];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram2];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram3];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram4];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram5];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.Bath_DeviationAlaram6];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_RED];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_YELLOW];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_GREEN];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_BLUE];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_WHITE];
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_BUZZWER];
        }

        private static List<ValveDeviceIO> CreateDefaultValveDeviceIO()
        {
            List<ValveDeviceIO> valveDeviceIO = new List<ValveDeviceIO>();

            foreach(string valveID in PLCService.ValveIDtoOutputSolValveIdx1.Keys)
            {
                valveDeviceIO.Add(new () { ID = valveID, Name = valveID, SolValveID = valveID });
            }
            foreach (string valveID in PLCService.ValveIDtoOutputSolValveIdx2.Keys)
            {
                valveDeviceIO.Add(new() { ID = valveID, Name = valveID, SolValveID = valveID });
            }

            return valveDeviceIO;
        }

        private static List<Device> CreateDefaultGasIO()
        {
            return [new() { ID = "Gas1", Name = "H2"}, new() { ID = "Gas2", Name = "N2" }, new() { ID = "Gas3", Name = "NH3" }, new() { ID = "Gas4", Name = "SiH4" },
                new() { ID = "Source1", Name = "TEB" }, new() { ID = "Source2", Name = "TMAI"}, new() { ID = "Source3", Name = "TMIn"}, new() { ID = "Source4", Name = "TMGa"},
                new() { ID = "Source5", Name = "DTMGa"}, new() { ID = "Source6", Name = "Cp2Mg"}];
        }


        [RelayCommand]
        private void ToggleInductionHeaterPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(InductionHeaterPowerOn, PLCService.OutputCmd1Index.InductionHeaterPower, "Induction Heater Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand]
        private void ToggleThermalBathPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(ThermalBathPowerOn, PLCService.OutputCmd1Index.ThermalBathPower, "Thermal Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand]
        private void ToggleVaccumPumpPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(VaccumPumpPowerOn, PLCService.OutputCmd1Index.VaccumPumpPower, "Vaccum Pump Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand]
        private void ToggleLineHeaterPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(LineHeaterPowerOn, PLCService.OutputCmd1Index.LineHeaterPower, "Line Heater Power On/Off") == true)
            {
                
            }
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

        private IOStateListSubscriber iOStateListSubscriber;

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
        private static ObservableManager<(string, string)>.DataIssuer GasIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("GasIOLabelChanged");
        private static ObservableManager<(string, string)>.DataIssuer ValveIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("ValveIOLabelChanged");
        private static ObservableManager<(string, string)>.DataIssuer AnalogIOLabelChangedPublisher = ObservableManager<(string, string)>.Get("AnalogIOLabelChanged");
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
