using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using SapphireXR_App.Common;
using System.ComponentModel;
using System.Collections;
using SapphireXR_App.Enums;
using SapphireXR_App.WindowServices;


namespace SapphireXR_App.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        static SettingViewModel()
        {
            var fdevice = File.ReadAllText(DevceIOSettingFilePath);
            JToken? jDeviceInit = JToken.Parse(fdevice);
            if(jDeviceInit == null)
            {
                return;
            }

            var publishDeviceNameChanged = (object? sender, string? propertyName, ObservableManager<(string, string)>.Publisher publisher) =>
            {
                if (propertyName == nameof(Device.Name))
                {
                    Device? device = sender as Device;
                    if (device != null && device.ID != null && device.Name != null)
                    {
                        publisher.Publish((device.ID, device.Name));
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
            JToken? jAlarmDeviation = jDeviceInit["AlarmDeviation"];
            if (jAlarmDeviation != null)
            {
                AlarmDeviationValue = JsonConvert.DeserializeObject<float>(jAlarmDeviation.ToString());
            }
            JToken? jWarningDeviation = jDeviceInit["WarningDeviation"];
            if (jWarningDeviation != null)
            {
                WarningDeviationValue = JsonConvert.DeserializeObject<float>(jWarningDeviation.ToString());
            }
            JToken? jAnalogDelayTime = jDeviceInit["AlarmDelayTimeA"];
            if (jAnalogDelayTime != null)
            {
                AnalogDeviceDelayTimeValue = JsonConvert.DeserializeObject<float>(jAnalogDelayTime.ToString());
            }
            JToken? jDigitalDelayTime = jDeviceInit["AlarmDelayTimeD"];
            if (jDigitalDelayTime != null)
            {
                DigitalDeviceDelayTimeValue = JsonConvert.DeserializeObject<float>(jDigitalDelayTime.ToString());
            }
            JToken? jInterLockD = jDeviceInit["InterLockD"];
            if (jInterLockD != null)
            {
                dInterLockD = JsonConvert.DeserializeObject<Dictionary<string, InterLockD>>(jInterLockD.ToString());
                if (dInterLockD != null)
                {
                    foreach ((string name, InterLockD interlockD) in dInterLockD)
                    {
                        PLCService.InterlockEnableSetting? plcArg = null;
                        switch (name)
                        {
                            case "InductionPowerSupply":
                                plcArg = PLCService.InterlockEnableSetting.InductionPowerSupply;
                                break;

                            case "SusceptorRotationMotor":
                                plcArg = PLCService.InterlockEnableSetting.SusceptorRotationMotor;
                                break;
                        }
                        if (plcArg != null)
                        {
                            interlockD.PropertyChanged += (sender, args) =>
                            {
                                if (PLCService.Connected == PLCConnection.Connected)
                                {
                                    InterLockD? interlockD = sender as InterLockD;
                                    if (interlockD != null)
                                    {
                                        switch(args.PropertyName)
                                        {
                                            case nameof(InterLockD.IsEnable):
                                                PLCService.WriteInterlockEnableState(interlockD.IsEnable, plcArg.Value);
                                                break;
                                        }
                                    }
                                }
                            };
                        }
                    }
                }
            }
            JToken? jInterLockA = jDeviceInit["InterLockA"];
            if (jInterLockA != null)
            {
                dInterLockA = JsonConvert.DeserializeObject<Dictionary<string, InterLockA>>(jInterLockA.ToString());
                if (dInterLockA != null)
                {
                    foreach (KeyValuePair<string, InterLockA> analogLogInterlock in dInterLockA)
                    {
                        (PLCService.InterlockEnableSetting, PLCService.InterlockValueSetting) plcArgs;
                        if(InterlockSettingNameToPLCServiceArgs.TryGetValue(analogLogInterlock.Key, out plcArgs) == true)
                        { 
                            analogLogInterlock.Value.PropertyChanged += (sender, args) =>
                            {
                                if (PLCService.Connected == PLCConnection.Connected)
                                {
                                    InterLockA? interlockA = sender as InterLockA;
                                    if (interlockA != null)
                                    {
                                        switch (args.PropertyName)
                                        {
                                            case nameof(InterLockA.IsEnable):
                                                PLCService.WriteInterlockEnableState(interlockA.IsEnable, plcArgs.Item1);
                                            break;

                                            case nameof(InterLockA.Treshold):
                                                try
                                                {
                                                    PLCService.WriteInterlockValueState(float.Parse(interlockA.Treshold), plcArgs.Item2);
                                                }
                                                catch(ArgumentException)
                                                { 
                                                }
                                                catch(FormatException)
                                                {
                                                }
                                                catch(OverflowException)
                                                {
                                                }
                                                break;
                                        }
                                    }
                                }
                            };
                        }
                    }
                }
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
            Online = PLCService.Connected == PLCConnection.Connected ? true : false;

            ObservableManager<BitArray>.Subscribe("DeviceIOList", iOStateListSubscriber = new IOStateListSubscriber(this));
            ObservableManager<BitArray>.Subscribe("OutputCmd1", modulePowerStateSubscriber = new ModulePowerStateSubscriber(this));
            ObservableManager<bool>.Subscribe("App.Closing", appClosingSubscriber = new AppClosingSubscriber(this));
            ObservableManager<PLCConnection>.Subscribe("PLCService.Connected", plcConnectionStateSubscriber = new PLCConnectionStateSubscriber(this));

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                if (PLCService.Connected == PLCConnection.Connected)
                {
                    switch (args.PropertyName)
                    {
                        case nameof(AlarmDeviation):
                            PLCService.WriteAlarmDeviationState(AlarmDeviation);
                            break;

                        case nameof(WarningDeviation):
                            PLCService.WriteWarningDeviationState(WarningDeviation);
                            break;

                        case nameof(AnalogDeviceDelayTime):
                            PLCService.WriteAnalogDeviceDelayTime(AnalogDeviceDelayTime);
                            break;

                        case nameof(DigitalDeviceDelayTime):
                            PLCService.WriteDigitalDeviceDelayTime(DigitalDeviceDelayTime);
                            break;
                    }
                }
            };
        }

        private static void PublishDeviceNameChanged(object? sender, string? propertyName, ObservableManager<(string, string)>.Publisher publisher)
        {
            if (propertyName == nameof(Device.Name))
            {
                Device? device = sender as Device;
                if (device != null && device.ID != null && device.Name != null)
                {
                    publisher.Publish((device.ID, device.Name));
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
          
            lAnalogDeviceIO = dAnalogDeviceIO?.Values.ToList();
            if (lAnalogDeviceIO != null)
            {
                foreach (var io in lAnalogDeviceIO)
                {
                    io.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                    {
                        PublishDeviceNameChanged(sender, args.PropertyName, AnalogIOLabelChangedPublisher);
                        AnalogDeviceIO? analogDevice = sender as AnalogDeviceIO;
                        if (analogDevice != null && analogDevice.ID != null)
                        {
                            switch (args.PropertyName)
                            {
                                case nameof(AnalogDeviceIO.AlarmSet):
                                    PLCService.WriteAnalogDeviceAlarmState(analogDevice.ID, analogDevice.AlarmSet);
                                    break;

                                case nameof(AnalogDeviceIO.WarningSet):
                                    PLCService.WriteAnalogDeviceWarningState(analogDevice.ID, analogDevice.WarningSet);
                                    break;
                            }
                        }
                    };
                }
            }
            lSwitchDI = dSwitchDI?.Values.ToList();
            if(lSwitchDI != null)
            {
                foreach(var io in lSwitchDI)
                {
                    io.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                    {
                        SwitchDI? switchID = sender as SwitchDI;
                        if (switchID != null && switchID.ID != null)
                        {
                            switch (args.PropertyName)
                            {
                                case nameof(SwitchDI.AlarmSet):
                                    PLCService.WriteDigitalDeviceAlarmState(switchID.ID, switchID.AlarmSet);
                                    break;

                                case nameof(SwitchDI.WarningSet):
                                    PLCService.WriteDigitalDeviceWarningState(switchID.ID, switchID.WarningSet);
                                    break;
                            }
                        }
                    };
                }
            }
            lGasDO = dGasDO?.Values.ToList();

            if (PLCService.Connected == PLCConnection.Connected)
            {
                initializeSettingToPLC();
            }
        }

        public void AlarmSettingSave()
        {
            JToken jsonAnalogDeviceIO = JsonConvert.SerializeObject(dAnalogDeviceIO);
            JToken jValveDeviceIO = JsonConvert.SerializeObject(ValveDeviceIO);
            JToken jGasIO = JsonConvert.SerializeObject(GasIO);
            JToken jsonSwitchDI = JsonConvert.SerializeObject(dSwitchDI);
            JToken jsonGasDO = JsonConvert.SerializeObject(dGasDO);
            JToken jInterLockD = JsonConvert.SerializeObject(dInterLockD);
            JToken jInterLockA = JsonConvert.SerializeObject(dInterLockA);
            JToken jAlarmDeviation = JsonConvert.SerializeObject(AlarmDeviationValue);
            JToken jWarningDeviation = JsonConvert.SerializeObject(WarningDeviationValue);
            JToken jAnalogDelayTime = JsonConvert.SerializeObject(AnalogDeviceDelayTimeValue);
            JToken jDigitalDelayTime = JsonConvert.SerializeObject(DigitalDeviceDelayTimeValue);


            JObject jDeviceIO = new(
                new JProperty("AlarmDeviation", jAlarmDeviation),
                new JProperty("WarningDeviation", jWarningDeviation),
                new JProperty("AlarmDelayTimeA", jAnalogDelayTime),
                new JProperty("AlarmDelayTimeD", jDigitalDelayTime),
                new JProperty("AnalogDeviceIO", jsonAnalogDeviceIO),
                new JProperty("ValveDeviceIO", jValveDeviceIO),
                new JProperty("GasIO", jGasIO),
                new JProperty("SwitchDI", jsonSwitchDI),
                new JProperty("GasDO", jsonGasDO),
                new JProperty("InterLockD", jInterLockD),
                new JProperty("InterLockA", jInterLockA)
            );

            if (File.Exists(DevceIOSettingFilePath)) File.Delete(DevceIOSettingFilePath);
            File.WriteAllText(DevceIOSettingFilePath, jDeviceIO.ToString());
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
            IOList[io++].OnOff = ioStateList[(int)PLCService.IOListIndex.SingalTower_BUZZER];
        }

        public void initializeSettingToPLC()
        {
            if (settingToPLCInitialized == false)
            {
                PLCService.WriteDeviceMaxValue(lAnalogDeviceIO);
                PLCService.WriteAlarmWarningSetting(lAnalogDeviceIO ?? [], lSwitchDI ?? []);
                
                PLCService.WriteAlarmDeviationState(AlarmDeviation);
                PLCService.WriteWarningDeviationState(WarningDeviation);
                PLCService.WriteAnalogDeviceDelayTime(AnalogDeviceDelayTime);
                PLCService.CommitAnalogDeviceInterlockSettingToPLC();
                PLCService.WriteDigitalDeviceDelayTime(DigitalDeviceDelayTime);
                PLCService.CommitDigitalDeviceInterlockSettingToPLC();

                if (dInterLockA != null)
                {
                    foreach ((string name, InterLockA interlockA) in dInterLockA)
                    {
                        (PLCService.InterlockEnableSetting, PLCService.InterlockValueSetting) plcArgs;
                        if (InterlockSettingNameToPLCServiceArgs.TryGetValue(name, out plcArgs) == true)
                        {
                            PLCService.WriteInterlockEnableState(interlockA.IsEnable, plcArgs.Item1);
                            try
                            {
                                PLCService.WriteInterlockValueState(float.Parse(interlockA.Treshold), plcArgs.Item2);
                            }
                            catch (ArgumentNullException) { }
                            catch (FormatException) { }
                            catch (OverflowException) { }
                        }
                    }
                }
                if(dInterLockD != null)
                {
                    foreach((string name, InterLockD interlockD) in dInterLockD)
                    {
                        PLCService.InterlockEnableSetting? plcArg = null;
                        switch (name)
                        {
                            case "InductionPowerSupply":
                                plcArg = PLCService.InterlockEnableSetting.InductionPowerSupply;
                                break;

                            case "SusceptorRotationMotor":
                                plcArg = PLCService.InterlockEnableSetting.SusceptorRotationMotor;
                                break;
                        }
                        if(plcArg != null)
                        {
                            PLCService.WriteInterlockEnableState(interlockD.IsEnable, plcArg.Value);
                        }
                    }
                }
                PLCService.CommitInterlockEnableToPLC();
                PLCService.CommitInterlockValueToPLC();
                settingToPLCInitialized = true;
            }
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

        private bool canCommandExecuteBase()
        {
            return PLCService.Connected == PLCConnection.Connected;
        }

        [RelayCommand(CanExecute = "canCommandExecuteBase")]
        private void ToggleInductionHeaterPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(InductionHeaterPowerOn, PLCService.OutputCmd1Index.InductionHeaterPower, "Induction Heater Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand(CanExecute = "canCommandExecuteBase")]
        private void ToggleThermalBathPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(ThermalBathPowerOn, PLCService.OutputCmd1Index.ThermalBathPower, "Thermal Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand(CanExecute = "canCommandExecuteBase")]
        private void ToggleVaccumPumpPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(VaccumPumpPowerOn, PLCService.OutputCmd1Index.VaccumPumpPower, "Vaccum Pump Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand(CanExecute = "canCommandExecuteBase")]
        private void ToggleLineHeaterPower()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(LineHeaterPowerOn, PLCService.OutputCmd1Index.LineHeaterPower, "Line Heater Power On/Off") == true)
            {
                
            }
        }
        [RelayCommand]
        private void AnalogDeviceSettingSave()
        {
            if (PLCConnectionState.Instance.Online == true)
            {
                PLCService.CommitAnalogDeviceAlarmWarningSettingStateToPLC();
            }
            AlarmSettingSave();
        }
        [RelayCommand]
        private void DigitalDeviceSettingSave()
        {
            if (PLCConnectionState.Instance.Online == true)
            {
                PLCService.CommitDigitalDeviceAlarmWarningSettingStateToPLC();
            }
            AlarmSettingSave();
        }
        [RelayCommand]
        private void InterlockSettingSave()
        {
            if (PLCConnectionState.Instance.Online == true)
            {
                PLCService.CommitInterlockEnableToPLC();
                PLCService.CommitInterlockValueToPLC();
            }
            AlarmSettingSave();
        }
    }
}
