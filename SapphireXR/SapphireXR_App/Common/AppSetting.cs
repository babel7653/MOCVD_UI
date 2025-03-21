using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace SapphireXR_App.Common
{
    internal static class AppSetting
    {
        static AppSetting()
        {
            JToken? appSettingRootToken = JToken.Parse(File.ReadAllText(AppSettingFilePath));

            LogFileDirectory = (string?)GetSettingValue(appSettingRootToken, "LogFileDirectory") ?? LogFileDirectory;
            BatchOnAlarmState = (string?)GetSettingValue(appSettingRootToken, "BatchOnAlarmState");
            BatchOnRecipeEnd = (string?)GetSettingValue(appSettingRootToken, "BatchOnRecipeEnd");
            UnderFlowControlFallbackRatePercentage = (int?)(Int64?)GetSettingValue(appSettingRootToken, "UnderFlowControlFallbackRatePercentage") ?? UnderFlowControlFallbackRatePercentage;
            UnderFlowControlFallbackRate = UnderFlowControlFallbackRatePercentage / 100.0f;
            FloatingPointMaxNumberDigit = (int?)(Int64?)GetSettingValue(appSettingRootToken, "FloatingPointMaxNumberDigit") ?? FloatingPointMaxNumberDigit;
            PLCAddress = (string?)GetSettingValue(appSettingRootToken, "PLCAddress") ?? PLCAddress;
            PLCPort = (int?)(Int64?)GetSettingValue(appSettingRootToken, "PLCPort") ?? PLCPort;
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(AppSettingFilePath, new JObject(new JProperty("LogFileDirectory", JsonConvert.SerializeObject(LogFileDirectory)), new JProperty("BatchOnAlarmState", JsonConvert.SerializeObject(BatchOnAlarmState)),
                        new JProperty("BatchOnRecipeEnd", JsonConvert.SerializeObject(BatchOnRecipeEnd)), new JProperty("UnderFlowControlFallbackRatePercentage", JsonConvert.SerializeObject(UnderFlowControlFallbackRatePercentage)),
                        new JProperty("FloatingPointMaxNumberDigit", JsonConvert.SerializeObject(FloatingPointMaxNumberDigit)), new JProperty("PLCAddress", JsonConvert.SerializeObject(PLCAddress)),
                        new JProperty("PLCPort", JsonConvert.SerializeObject(PLCPort))).ToString());
            }
            catch(Exception exception)
            {
                MessageBox.Show("애플리케이션 설정 파일 (" + AppSettingFilePath + ")을 저장하는데 문제가 생겼습니다. 원인은 다음과 같습니다: " + exception.Message);
            }
        }

        private static object? GetSettingValue(JToken appSettingRootToken, string key)
        {
            if (appSettingRootToken != null)
            {
                JToken? token = appSettingRootToken[key];
                if (token != null)
                {
                    return JsonConvert.DeserializeObject(token.ToString());
                }
            }

            return null;
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
        public static string? BatchOnAlarmState;
        public static string? BatchOnRecipeEnd;
        public static string PLCAddress = "Local";
        public static int PLCPort = 851;
    }
}
