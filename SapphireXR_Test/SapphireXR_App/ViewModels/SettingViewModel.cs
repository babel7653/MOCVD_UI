using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SapphireXR_App.Models;
using System.Windows.Input;
using System.IO;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
  public class SettingViewModel : ViewModelBase
  {
    public string fname = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\data\\parameters\\" + @"DeviceIO.json";
    public Dictionary<string, GasAIO> dGasAIO = [];
    public Dictionary<string, SwitchDI> dSwitchDI = [];
    public Dictionary<string, GasDO> dGasDO = [];
    public Dictionary<string, string> dPreSet { get; set; } = [];
    public Dictionary<string, InterLockA> dInterLockA { get; set; } = [];
    public Dictionary<string, bool> dInterLockD { get; set; } = [];
    public List<GasAIO> lGasAIO { get; set; } = [];
    public static List<GasAIO> sGasAIO = new();

    public List<SwitchDI> lSwitchDI { get; set; } = [];
    public List<GasDO> lGasDO { get; set; } = [];
    public UserState userstate { get; set; } = new();

    public ICommand AlarmSettingLoadCommand => new RelayCommand(AlarmSettingLoad);
    public ICommand AlarmSettingSaveCommand => new RelayCommand(AlarmSettingSave);

    public SettingViewModel()
    {
      AlarmSettingLoad();
      sGasAIO = lGasAIO;
    }

    public void AlarmSettingLoad()
    {
      //Json파일 읽기 및 Pars
      var fdevice = File.ReadAllText(fname);

      JToken? jDeviceInit = JToken.Parse(fdevice);
      JToken? jGasAIO = jDeviceInit["GasAIO"];
      JToken? jSwitchDI = jDeviceInit["SwitchDI"];
      JToken? jGasDO = jDeviceInit["GasDO"];
      JToken? jPreSet = jDeviceInit["PreSet"];
      JToken? jInterLockD = jDeviceInit["InterLockD"];
      JToken? jInterLockA = jDeviceInit["InterLockA"];
      JToken? jUserState = jDeviceInit["UserState"];

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
    public void AlarmSettingSave()
    {
      JToken jsonGasAIO = JsonConvert.SerializeObject(dGasAIO);
      JToken jsonSwitchDI = JsonConvert.SerializeObject(dSwitchDI);
      JToken jsonGasDO = JsonConvert.SerializeObject(dGasDO);
      JToken jPreSet = JsonConvert.SerializeObject(dPreSet);
      JToken jInterLockD = JsonConvert.SerializeObject(dInterLockD);
      JToken jInterLockA = JsonConvert.SerializeObject(dInterLockA);
      JToken jUserState = JsonConvert.SerializeObject(userstate);

      JObject jDeviceIO = new(
          new JProperty("GasAIO", jsonGasAIO),
          new JProperty("SwitchDI", jsonSwitchDI),
          new JProperty("GasDO", jsonGasDO),
          new JProperty("PreSet", jPreSet),
          new JProperty("InterLockD", jInterLockD),
          new JProperty("InterLockA", jInterLockA),
          new JProperty("UserState", jUserState)
          );

      if (File.Exists(fname)) File.Delete(fname);
      File.WriteAllText(fname, jDeviceIO.ToString());
    }


  }

}
