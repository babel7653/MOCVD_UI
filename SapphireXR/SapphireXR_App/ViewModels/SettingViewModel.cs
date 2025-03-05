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
                    => { settingViewModel.InductionHeaterPowerOn = (value == true ? "ON" : "OFF"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.ThermalBathMC], ref prevThermalBatchPowerOn, (bool value) 
                    => { settingViewModel.ThermalBathPowerOn = (value == true ? "ON" : "OFF"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.VaccumPumpMC], ref prevVaccumPumpPowerOn, (bool value) 
                    => { settingViewModel.VaccumPumpPowerOn = (value == true ? "ON" : "OFF"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.LineHeaterMC], ref prevLineHeaterPowerOn, (bool value) 
                    => { settingViewModel.LineHeaterPowerOn = (value == true ? "ON" : "OFF"); });
            }

            private SettingViewModel settingViewModel;
            private bool? prevInpudctionHeaterPowerOn = null;
            private bool? prevThermalBatchPowerOn = null;
            private bool? prevVaccumPumpPowerOn = null;
            private bool? prevLineHeaterPowerOn = null;
        }
        public partial class IOSetting: ObservableObject
        {
            required public string Name { get; set; } = "";
            [ObservableProperty]
            private bool _onOff;
            
        }

        public string fname = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\Data\\Configuration\\" + @"DeviceIO.json";
        public Dictionary<string, AnalogDeviceIO>? dAnalogDeviceIO = [];
        public Dictionary<string, SwitchDI>? dSwitchDI = [];
        public Dictionary<string, GasDO>? dGasDO = [];
        public Dictionary<string, string>? dPreSet { get; set; } = [];
        public Dictionary<string, InterLockA>? dInterLockA { get; set; } = [];
        public Dictionary<string, bool>? dInterLockD { get; set; } = [];
        public List<AnalogDeviceIO>? lAnalogDeviceIO { get; set; } = [];
        public List<SwitchDI>? lSwitchDI { get; set; } = [];
        public List<GasDO>? lGasDO { get; set; } = [];
        public UserState? userstate { get; set; } = new();

        [ObservableProperty]
        public IList<IOSetting> _iOList;

        public bool WithoutConnection { get; set; }

        public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
        public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);

        [ObservableProperty]
        private string? _logIntervalInRecipeRun;

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
            ObservableManager<BitArray>.Subscribe("DigitalOutput3", modulePowerStateSubscriber = new ModulePowerStateSubscriber(this));
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
                            GlobalSetting.LogIntervalInRecipeRunInMS = int.Parse(LogIntervalInRecipeRun);
                        }
                        break;
                }
            };
            //Json파일 읽기 및 Pars
            var fdevice = File.ReadAllText(fname);

            JToken? jDeviceInit = JToken.Parse(fdevice);
            JToken? jAnalogDeviceIO = jDeviceInit["AnalogDeviceIO"];
            JToken? jSwitchDI = jDeviceInit["SwitchDI"];
            JToken? jGasDO = jDeviceInit["GasDO"];
            JToken? jPreSet = jDeviceInit["PreSet"];
            JToken? jInterLockD = jDeviceInit["InterLockD"];
            JToken? jInterLockA = jDeviceInit["InterLockA"];
            JToken? jUserState = jDeviceInit["UserState"];
            JToken? jWithoutConnection = jDeviceInit["WithoutConnection"];
            JToken? jLogIntervalInRecipeRun = jDeviceInit["LogIntervalInRecipeRun"];

            dAnalogDeviceIO = JsonConvert.DeserializeObject<Dictionary<string, AnalogDeviceIO>>(jAnalogDeviceIO.ToString());
            dSwitchDI = JsonConvert.DeserializeObject<Dictionary<string, SwitchDI>>(jSwitchDI.ToString());
            dGasDO = JsonConvert.DeserializeObject<Dictionary<string, GasDO>>(jGasDO.ToString());
            dPreSet = JsonConvert.DeserializeObject<Dictionary<string, string>>(jPreSet.ToString());
            dInterLockD = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jInterLockD.ToString());
            dInterLockA = JsonConvert.DeserializeObject<Dictionary<string, InterLockA>>(jInterLockA.ToString());
            userstate = JsonConvert.DeserializeObject<UserState>(jUserState.ToString());
            if (jLogIntervalInRecipeRun != null)
            {
                LogIntervalInRecipeRun = JsonConvert.DeserializeObject<string>(jLogIntervalInRecipeRun.ToString()) ?? GlobalSetting.DefaultLogIntervalInRecipeRunInMS.ToString();
            }

            lAnalogDeviceIO = dAnalogDeviceIO.Values.ToList();
            lSwitchDI = dSwitchDI.Values.ToList();
            lGasDO = dGasDO.Values.ToList();

            PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
            PLCService.ReadMaxValueFromPLC();
        }

        public void AlarmSettingSave()
        {
            JToken jsonAnalogDeviceIO = JsonConvert.SerializeObject(dAnalogDeviceIO);
            JToken jsonSwitchDI = JsonConvert.SerializeObject(dSwitchDI);
            JToken jsonGasDO = JsonConvert.SerializeObject(dGasDO);
            JToken jPreSet = JsonConvert.SerializeObject(dPreSet);
            JToken jInterLockD = JsonConvert.SerializeObject(dInterLockD);
            JToken jInterLockA = JsonConvert.SerializeObject(dInterLockA);
            JToken jUserState = JsonConvert.SerializeObject(userstate);
            JToken? jLogIntervalInRecipeRun = (LogIntervalInRecipeRun != null ? JsonConvert.SerializeObject(int.Parse(LogIntervalInRecipeRun)) : JsonConvert.SerializeObject(GlobalSetting.DefaultLogIntervalInRecipeRunInMS));
           

            JObject jDeviceIO = new(
                new JProperty("AnalogDeviceIO", jsonAnalogDeviceIO),
                new JProperty("SwitchDI", jsonSwitchDI),
                new JProperty("GasDO", jsonGasDO),
                new JProperty("PreSet", jPreSet),
                new JProperty("InterLockD", jInterLockD),
                new JProperty("InterLockA", jInterLockA),
                new JProperty("UserState", jUserState),
                new JProperty("LogIntervalInRecipeRun", jLogIntervalInRecipeRun)
                );

            if (File.Exists(fname)) File.Delete(fname);
            File.WriteAllText(fname, jDeviceIO.ToString());

            PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
            PLCService.ReadMaxValueFromPLC();
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

        private void showConfirmWindow(string onOff, PLCService.DigitalOutput3Index index)
        {
            string nextState = (onOff == "On" ? "Off" : "On");
            if(ValveOperationEx.Show("Moduel Power 상태 변경", nextState + " 상태로 변경하시겠습니까?") == Enums.ValveOperationExResult.Ok)
            {
                PLCService.WriteModulePowerState(index, (nextState == "On") ? true : false);
            }
        }

        [RelayCommand]
        private void ToggleInductionHeaterPower()
        {
            showConfirmWindow(InductionHeaterPowerOn, PLCService.DigitalOutput3Index.InductionHeaterMC);
        }
        [RelayCommand]
        private void ToggleThermalBathPower()
        {
            showConfirmWindow(ThermalBathPowerOn, PLCService.DigitalOutput3Index.ThermalBathMC);
        }
        [RelayCommand]
        private void ToggleVaccumPumpPower()
        {
            showConfirmWindow(VaccumPumpPowerOn, PLCService.DigitalOutput3Index.VaccumPumpMC);
        }
        [RelayCommand]
        private void ToggleLineHeaterPower()
        {
            showConfirmWindow(LineHeaterPowerOn, PLCService.DigitalOutput3Index.LineHeaterMC);
        }

        private IOStateListSubscriber iOStateListSubscriber;

        [ObservableProperty]
        private string _inductionHeaterPowerOn = "";
        [ObservableProperty]
        private string _thermalBathPowerOn = "";
        [ObservableProperty]
        private string _vaccumPumpPowerOn = "";
        [ObservableProperty]
        private string _lineHeaterPowerOn = "";

        ModulePowerStateSubscriber modulePowerStateSubscriber;
    }
}
