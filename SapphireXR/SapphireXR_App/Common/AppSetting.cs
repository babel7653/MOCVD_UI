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

            LogFileDirectory = GetSettingValue<string>(appSettingRootToken, "LogFileDirectory") ?? LogFileDirectory;
            BatchOnAlarmState = GetSettingValue<string>(appSettingRootToken, "BatchOnAlarmState");
            BatchOnRecipeEnd = GetSettingValue<string>(appSettingRootToken, "BatchOnRecipeEnd");
            float tempUnderFlowControlFallbackRate = GetSettingValue<float>(appSettingRootToken, "UnderFlowControlFallbackRate");
            if(tempUnderFlowControlFallbackRate != default)
            {
                UnderFlowControlFallbackRate = tempUnderFlowControlFallbackRate / 100.0f;
            }
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(AppSettingFilePath, new JObject(new JProperty("LogFileDirectory", JsonConvert.SerializeObject(LogFileDirectory)), new JProperty("BatchOnAlarmState", JsonConvert.SerializeObject(BatchOnAlarmState)),
                        new JProperty("BatchOnRecipeEnd", JsonConvert.SerializeObject(BatchOnRecipeEnd)), new JProperty("UnderFlowControlFallbackRate", JsonConvert.SerializeObject(UnderFlowControlFallbackRate))).ToString());
            }
            catch(Exception exception)
            {
                MessageBox.Show("애플리케이션 설정 파일 (" + AppSettingFilePath + ")을 저장하는데 문제가 생겼습니다. 원인은 다음과 같습니다: " + exception.Message);
            }
        }

        private static T? GetSettingValue<T>(JToken appSettingRootToken, string key)
        {
            if (appSettingRootToken != null)
            {
                JToken? token = appSettingRootToken[key];
                if (token != null)
                {
                    T? value = JsonConvert.DeserializeObject<T>(token.ToString());
                    return value;
                }
            }

            return default;
        }

        public static readonly int DefaultLogIntervalInRecipeRunInMS = 1000;
        public static readonly int MaxNumberDigit = 4;
        public static readonly string AppSettingFilePath = Util.GetResourceAbsoluteFilePath("\\Configurations\\AppSetting.json");

        private static int _logIntervalInRecipeRunInMS = DefaultLogIntervalInRecipeRunInMS;
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
        public static readonly float UnderFlowControlFallbackRate = 1.0f / 100.0f;
        public static string? BatchOnAlarmState;
        public static string? BatchOnRecipeEnd;
    }
}
