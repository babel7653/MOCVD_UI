using SapphireXR_App.Common;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        private static void ReadValveStateFromPLC()
        {
            uint aReadValveStatePLC1 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array
            uint aReadValveStatePLC2 = (uint)Ads.ReadAny(hReadValveStatePLC2, typeof(uint)); // Convert to Array

            baReadValveStatePLC1 = new BitArray([(int)aReadValveStatePLC1]);
            baReadValveStatePLC2 = new BitArray([(int)aReadValveStatePLC2]);
        }

        private static void ReadInitialStateValueFromPLC()
        {
            if (AppSetting.ConfigMode == false)
            {
                ReadValveStateFromPLC();
                ReadMaxValueFromPLC();
                ReadCurrentValueFromPLC();

                dCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".CurrentValue"));
                }
                dControlValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dControlValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".ControlValue"));
                }
                dTargetValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dTargetValueIssuers.Add(kv.Key, ObservableManager<float>.Get("FlowControl." + kv.Key + ".TargetValue"));
                }
                dControlCurrentValueIssuers = new Dictionary<string, ObservableManager<(float, float)>.DataIssuer>();
                foreach (KeyValuePair<string, int> kv in dIndexController)
                {
                    dControlCurrentValueIssuers.Add(kv.Key, ObservableManager<(float, float)>.Get("FlowControl." + kv.Key + ".ControlTargetValue.CurrentPLCState"));
                }
                aMonitoringCurrentValueIssuers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
                foreach (KeyValuePair<string, int> kv in dMonitoringMeterIndex)
                {
                    aMonitoringCurrentValueIssuers.Add(kv.Key, ObservableManager<float>.Get("MonitoringPresentValue." + kv.Key + ".CurrentValue"));
                }
                dValveStateIssuers = new Dictionary<string, ObservableManager<bool>.DataIssuer>();
                foreach ((string valveID, int valveIndex) in ValveIDtoOutputSolValveIdx1)
                {
                    dValveStateIssuers.Add(valveID, ObservableManager<bool>.Get("Valve.OnOff." + valveID + ".CurrentPLCState"));
                }
                foreach ((string valveID, int valveIndex) in ValveIDtoOutputSolValveIdx2)
                {
                    dValveStateIssuers.Add(valveID, ObservableManager<bool>.Get("Valve.OnOff." + valveID + ".CurrentPLCState"));
                }
                dCurrentActiveRecipeIssue = ObservableManager<short>.Get("RecipeRun.CurrentActiveRecipe");
                baHardWiringInterlockStateIssuers = ObservableManager<BitArray>.Get("HardWiringInterlockState");
                dIOStateList = ObservableManager<BitArray>.Get("DeviceIOList");
                dRecipeEndedPublisher = ObservableManager<bool>.Get("RecipeEnded");
                dLineHeaterTemperatureIssuers = ObservableManager<float[]>.Get("LineHeaterTemperature");
                dRecipeControlHoldTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Hold");
                dRecipeControlPauseTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Pause");
                dRecipeControlRampTimeIssuer = ObservableManager<int>.Get("RecipeControlTime.Ramp");
                dDigitalOutput2 = ObservableManager<BitArray>.Get("DigitalOutput2");
                dDigitalOutput3 = ObservableManager<BitArray>.Get("DigitalOutput3");
                dOutputCmd1 = ObservableManager<BitArray>.Get("OutputCmd1");
                dInputManAuto = ObservableManager<BitArray>.Get("InputManAuto");
                dThrottleValveControlMode = ObservableManager<short>.Get("ThrottleValveControlMode");
                dPressureControlModeIssuer = ObservableManager<ushort>.Get("PressureControlMode");
                dThrottleValveStatusIssuer = ObservableManager<short>.Get("ThrottleValveStatus");
                dLogicalInterlockStateIssuer = ObservableManager<BitArray>.Get("LogicalInterlockState");

                ObservableManager<bool>.Subscribe("Leak Test Mode", leakTestModeSubscriber = new LeakTestModeSubscriber());
                ObservableManager<bool>.Subscribe("App.Closing", appCloseSubscriber = new AppCloseSubscriber());

                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(2000000);
                timer.Tick += OnTick;
                timer.Start();

                currentActiveRecipeListener = new DispatcherTimer();
                currentActiveRecipeListener.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 500);
                currentActiveRecipeListener.Tick += (object? sender, EventArgs e) =>
                {
                    dCurrentActiveRecipeIssue.Issue(Ads.ReadAny<short>(hRcpStepN));
                    if (RecipeRunEndNotified == false && Ads.ReadAny<short>(hCmd_RcpOperation) == 50)
                    {
                        dRecipeEndedPublisher.Issue(true);
                        RecipeRunEndNotified = true;
                    }
                    else
                        if (RecipeRunEndNotified == true && Ads.ReadAny<short>(hCmd_RcpOperation) == 0)
                    {
                        RecipeRunEndNotified = false;
                    }
                };
                currentActiveRecipeListener.Start();
            }
            else
            {
                ReadMaxValueFromFile();
            }
        }

        public static void ReadMaxValueFromPLC()
        {
            aDeviceMaxValue = Ads.ReadAny<float[]>(hDeviceMaxValuePLC, [dIndexController.Count]);
        }

        public static float ReadMaxValue(string flowControlID)
        {
            if (aDeviceMaxValue == null)
            {
                throw new Exception("aDeviceMaxValue is null in ReadMaxValue(), WriteDeviceMaxValue() must be called before");
            }
            return aDeviceMaxValue[dIndexController[flowControlID]];
        }

        private static void ReadCurrentValueFromPLC()
        {
            aDeviceCurrentValues = Ads.ReadAny<float[]>(hDeviceCurrentValuePLC, [NumControllers]);
            aDeviceControlValues = Ads.ReadAny<float[]>(hDeviceControlValuePLC, [NumControllers]);
            aDeviceTargetValues = Ads.ReadAny<float[]>(hWriteDeviceTargetValuePLC, [NumControllers]);
            aMonitoring_PVs = Ads.ReadAny<float[]>(hMonitoring_PV, [18]);
            aInputState = Ads.ReadAny<short[]>(hInputState, [5]);
            ReadValveStateFromPLC();
        }

        public static float ReadCurrentValue(string controllerID)
        {
            if (aDeviceCurrentValues != null)
            {
                return aDeviceCurrentValues[dIndexController[controllerID]];
            }
            else
            {
                return float.NaN;
            }
        }

        public static short ReadRCPOperationState()
        {
            return Ads.ReadAny<short>(hState_RcpOperation);
        }

        public static short ReadUserState()
        {
            return Ads.ReadAny<short>(hUserState);
        }

        public static BitArray ReadOutputCmd1()
        {
            short outputCmd1 = Ads.ReadAny<short>(hOutputCmd1);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd1) : BitConverter.GetBytes(outputCmd1).Reverse().ToArray());
        }

        public static ushort ReadPressureControlMode()
        {
            return Ads.ReadAny<ushort>(hOutputSetType);
        }

        public static ushort ReadThrottleValveMode()
        {
            return Ads.ReadAny<ushort>(hOutputMode);
        }

        public static bool ReadInputManAuto(uint index)
        {
            ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray())[0];
        }

        public static bool ReadDigitalOutputIO2(int bitIndex)
        {
            return new BitArray(new byte[1] { Ads.ReadAny<byte>(hDigitalOutput2) })[bitIndex];
        }

        public static bool ReadInputState4(int bitIndex)
        {
            short inputState = Ads.ReadAny<short>(hInputState4);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputState) : BitConverter.GetBytes(inputState).Reverse().ToArray())[bitIndex];
        }

        private static void ReadMaxValueFromFile()
        {
            string maxValuesFilePath = Util.GetResourceAbsoluteFilePath("\\Configurations\\MaxValue.json");
            try
            {
                JToken? maxValuesRootToken = JToken.Parse(File.ReadAllText(maxValuesFilePath));
                if (maxValuesRootToken != null)
                {
                    JToken? maxValuesToken = maxValuesRootToken["MaxValues"];
                    if(maxValuesToken != null)
                    {
                        aDeviceMaxValue = JsonConvert.DeserializeObject<float[]?>(maxValuesToken.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Max Value 로그 파일 (" + maxValuesFilePath + ")로부터 현재 MaxValue값들을 저장하는데 문제가 생겼습니다. 애플리케이션이 종료됩니다. 원인은 다음과 같습니다: " + ex.ToString());
            }
        }
    }
}
