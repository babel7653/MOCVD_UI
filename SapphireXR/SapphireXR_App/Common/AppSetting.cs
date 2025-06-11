using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Common
{
    internal static class AppSetting
    {
        static AppSetting()
        {
            try
            {
                JToken? appSettingRootToken = JToken.Parse(File.ReadAllText(AppSettingFilePath));

                LogFileDirectory = (string?)Util.GetSettingValue(appSettingRootToken, "LogFileDirectory") ?? LogFileDirectory;
                UnderFlowControlFallbackRatePercentage = (int?)(Int64?)Util.GetSettingValue(appSettingRootToken, "UnderFlowControlFallbackRatePercentage") ?? UnderFlowControlFallbackRatePercentage;
                UnderFlowControlFallbackRate = UnderFlowControlFallbackRatePercentage / 100.0f;
                FloatingPointMaxNumberDigit = (int?)(Int64?)Util.GetSettingValue(appSettingRootToken, "FloatingPointMaxNumberDigit") ?? FloatingPointMaxNumberDigit;
                PLCAddress = (string?)Util.GetSettingValue(appSettingRootToken, "PLCAddress") ?? PLCAddress;
                PLCPort = (int?)(Int64?)Util.GetSettingValue(appSettingRootToken, "PLCPort") ?? PLCPort;
                ConfigMode = (bool?)Util.GetSettingValue(appSettingRootToken, "ConfigMode") ?? ConfigMode;

                JToken? token = appSettingRootToken["PrecursorSourceMonitorLabel"];
                if (token != null)
                {
                    Dictionary<string, string>? precursorSourceMonitorLabel = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.ToString());
                    if (precursorSourceMonitorLabel != null)
                    {
                        PrecursorSourceMonitorLabel = precursorSourceMonitorLabel;
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("애플리케이션 설정 파일 (" + AppSettingFilePath + ")을 읽어오는데 문제가 생겼습니다. App setting 값은 디폴트 값으로 읽어옵니다. 원인은 다음과 같습니다: " + ex.Message);
            }
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(AppSettingFilePath, new JObject(new JProperty("LogFileDirectory", JsonConvert.SerializeObject(LogFileDirectory)), new JProperty("UnderFlowControlFallbackRatePercentage", JsonConvert.SerializeObject(UnderFlowControlFallbackRatePercentage)),
                        new JProperty("FloatingPointMaxNumberDigit", JsonConvert.SerializeObject(FloatingPointMaxNumberDigit)), new JProperty("PLCAddress", JsonConvert.SerializeObject(PLCAddress)),
                        new JProperty("PLCPort", JsonConvert.SerializeObject(PLCPort)), new JProperty("ConfigMode", JsonConvert.SerializeObject(ConfigMode)), 
                        new JProperty("PrecursorSourceMonitorLabel", JsonConvert.SerializeObject(PrecursorSourceMonitorLabel))).ToString());
            }
            catch(Exception exception)
            {
                MessageBox.Show("애플리케이션 설정 파일 (" + AppSettingFilePath + ")을 저장하는데 문제가 생겼습니다. 원인은 다음과 같습니다: " + exception.Message);
            }
        }
       
        public static readonly int FloatingPointMaxNumberDigit = 4;
        public static readonly string AppSettingFilePath = Util.GetResourceAbsoluteFilePath("\\Configurations\\AppSetting.json");

        private static int _logIntervalInRecipeRunInMS = 1000;
        public static int LogIntervalInRecipeRunInMS
        {
            get { return _logIntervalInRecipeRunInMS; }
            set
            {
                _logIntervalInRecipeRunInMS = value;
                logIntervalInRecipeRunIssuer.Issue(value);
            }
        }

        private static ObservableManager<int>.DataIssuer logIntervalInRecipeRunIssuer = ObservableManager<int>.Get("AppSetting.LogIntervalInRecipeRun");
        public static string LogFileDirectory = Util.GetAbsoluteFilePathFromAppRelativePath("Log");
        private static readonly int UnderFlowControlFallbackRatePercentage = 1;
        public static readonly float UnderFlowControlFallbackRate;
        public static string PLCAddress = "Local";
        public static int PLCPort = 851;
        public static bool ConfigMode = false;
        public static Dictionary<string, string> PrecursorSourceMonitorLabel = new Dictionary<string, string> { { "GasMonitor1", "H2" }, { "GasMonitor2", "N2" }, { "GasMonitor3", "SiH4" }, { "GasMonitor4", "NH3" },
            { "SourceMonitor1", "TEB" }, { "SourceMonitor2", "TMAl" }, { "SourceMonitor3", "TMIn" }, { "SourceMonitor4", "TMGa" },  { "SourceMonitor5", "DTMGa" }, { "SourceMonitor6", "Cp2Mg" }};
    }
}
