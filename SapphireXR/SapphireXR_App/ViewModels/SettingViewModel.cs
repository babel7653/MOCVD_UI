﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;


namespace SapphireXR_App.ViewModels
{
    public class SettingViewModel : ObservableObject
    {
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

        public bool WithoutConnection { get; set; }

        public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
        public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);


        public SettingViewModel()
        {
            AlarmSettingLoad();
        }
        public void AlarmSettingLoad()
        {
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

            dAnalogDeviceIO = JsonConvert.DeserializeObject<Dictionary<string, AnalogDeviceIO>>(jAnalogDeviceIO.ToString());
            dSwitchDI = JsonConvert.DeserializeObject<Dictionary<string, SwitchDI>>(jSwitchDI.ToString());
            dGasDO = JsonConvert.DeserializeObject<Dictionary<string, GasDO>>(jGasDO.ToString());
            dPreSet = JsonConvert.DeserializeObject<Dictionary<string, string>>(jPreSet.ToString());
            dInterLockD = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jInterLockD.ToString());
            dInterLockA = JsonConvert.DeserializeObject<Dictionary<string, InterLockA>>(jInterLockA.ToString());
            userstate = JsonConvert.DeserializeObject<UserState>(jUserState.ToString());

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

            JObject jDeviceIO = new(
                new JProperty("AnalogDeviceIO", jsonAnalogDeviceIO),
                new JProperty("SwitchDI", jsonSwitchDI),
                new JProperty("GasDO", jsonGasDO),
                new JProperty("PreSet", jPreSet),
                new JProperty("InterLockD", jInterLockD),
                new JProperty("InterLockA", jInterLockA),
                new JProperty("UserState", jUserState)
                );

            if (File.Exists(fname)) File.Delete(fname);
            File.WriteAllText(fname, jDeviceIO.ToString());

            PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
            PLCService.ReadMaxValueFromPLC();

        }
    }
}
