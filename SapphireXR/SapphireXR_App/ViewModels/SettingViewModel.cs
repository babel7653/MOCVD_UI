using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using SapphireXR_App.Common;
using System.ComponentModel;


namespace SapphireXR_App.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
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
                new() { Name= "Power Reset Switch", OnOff = true },  new() { Name= "Cover", OnOff = true }, new() { Name= "Cover", OnOff = true },  
                new() { Name= "SMPS", OnOff = true }, new() { Name= "SMPS", OnOff = true },  new() { Name= "SMPS", OnOff = true },
                new() { Name= "SMPS", OnOff = true },  new() { Name= "CP", OnOff = true }, new() { Name= "CP", OnOff = true },
                new() { Name= "CP", OnOff = true },  new() { Name= "CP", OnOff = true }, new() { Name= "CP", OnOff = true },
                new() { Name= "CP", OnOff = true },  new() { Name= "CP", OnOff = true }, new() { Name= "CP", OnOff = true },
                new() { Name= "CP", OnOff = true },  new() { Name= "Line Heater 1", OnOff = true }, new() { Name= "Line Heater 2", OnOff = true },
                new() { Name= "Line Heater 3", OnOff = true },  new() { Name= "Line Heater 4", OnOff = true }, new() { Name= "Line Heater 5", OnOff = true },
                new() { Name= "Line Heater 6", OnOff = true },  new() { Name= "Line Heater 7", OnOff = true }, new() { Name= "Thermal Bath", OnOff = true },
                new() { Name= "Thermal Bath", OnOff = true },  new() { Name= "Thermal Bath", OnOff = true }, new() { Name= "Thermal Bath", OnOff = true },
                new() { Name= "Thermal Bath", OnOff = true },  new() { Name= "Thermal Bath", OnOff = true }, new() { Name= "Singal Tower", OnOff = true },
                new() { Name= "Singal Tower", OnOff = true },  new() { Name= "Singal Tower", OnOff = true }, new() { Name= "Singal Tower", OnOff = true },
                new() { Name= "Singal Tower", OnOff = true },  new() { Name= "Singal Tower", OnOff = true }
            };
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
    }
}
