using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireXE_App.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SapphireXE_App.ViewModels
{
   public partial class MainViewModel : ViewModelBase
    {
        public string fname = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\data\\parameters\\" + @"DeviceIO.json";

        public Dictionary<string, GasAIO> dGasAIO = new Dictionary<string, GasAIO>();
        public Dictionary<string, SwitchDI> dSwitchDI = new Dictionary<string, SwitchDI>();
        public Dictionary<string, GasDO> dGasDO = new Dictionary<string, GasDO>();
        public Dictionary<string, string> dPreSet { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, InterLockA> dInterLockA { get; set; } = new Dictionary<string, InterLockA>();
        public Dictionary<string, bool> dInterLockD { get; set; } = new Dictionary<string, bool>();
        public List<GasAIO> lGasAIO { get; set; } = new List<GasAIO>();
        public List<SwitchDI> lSwitchDI { get; set; } = new List<SwitchDI>();
        public List<GasDO> lGasDO { get; set; } = new List<GasDO>();
        public UserState userstate { get; set; } = new UserState();

        public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
        public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);

        private void AlarmSettingLoad()
        {
            //Json파일 읽기 및 Pars
            var fdevice = File.ReadAllText(fname);

            JToken jDeviceInit = JToken.Parse(fdevice);
            JToken jGasAIO = jDeviceInit["GasAIO"];
            JToken jSwitchDI = jDeviceInit["SwitchDI"];
            JToken jGasDO = jDeviceInit["GasDO"];
            JToken jPreSet = jDeviceInit["PreSet"];
            JToken jInterLockD = jDeviceInit["InterLockD"];
            JToken jInterLockA = jDeviceInit["InterLockA"];
            JToken jUserState = jDeviceInit["UserState"];

            dGasAIO = JsonConvert.DeserializeObject<Dictionary<string, GasAIO>>(jGasAIO.ToString());
            dSwitchDI = JsonConvert.DeserializeObject<Dictionary<string, SwitchDI>>(jSwitchDI.ToString());
            dGasDO = JsonConvert.DeserializeObject<Dictionary<string, GasDO>>(jGasDO.ToString());
            dPreSet = JsonConvert.DeserializeObject<Dictionary<string, string>>(jPreSet.ToString());
            dInterLockD = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jInterLockD.ToString());
            dInterLockA = JsonConvert.DeserializeObject<Dictionary<string, InterLockA>>(jInterLockA.ToString());
            userstate = JsonConvert.DeserializeObject<UserState>(jUserState.ToString());
           
            lGasAIO = dGasAIO.Values.ToList();
            lSwitchDI = dSwitchDI.Values.ToList();
            lGasDO = dGasDO.Values.ToList();

        }
        private void AlarmSettingSave()
        {
            JToken jsonGasAIO = JsonConvert.SerializeObject(dGasAIO);
            JToken jsonSwitchDI = JsonConvert.SerializeObject(dSwitchDI);
            JToken jsonGasDO = JsonConvert.SerializeObject(dGasDO);
            JToken jPreSet = JsonConvert.SerializeObject(dPreSet);
            JToken jInterLockD = JsonConvert.SerializeObject(dInterLockD);
            JToken jInterLockA = JsonConvert.SerializeObject(dInterLockA);
            JToken jUserState = JsonConvert.SerializeObject(userstate);

            JObject jDeviceIO = new JObject(
                new JProperty("GasAIO", jsonGasAIO),
                new JProperty("SwitchDI", jsonSwitchDI),
                new JProperty("GasDO", jsonGasDO),
                new JProperty("PreSet", jPreSet),
                new JProperty("InterLockD", jInterLockD),
                new JProperty("InterLockA", jInterLockA),
                new JProperty("UserState", jUserState)
                );

            if (File.Exists(fname))
            {
                File.Delete(fname);
            }
            File.WriteAllText(fname, jDeviceIO.ToString());
        }
    }
}
