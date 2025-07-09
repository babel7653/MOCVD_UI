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
                ConnectionRetryMilleseconds = (uint?)(Int64?)Util.GetSettingValue(appSettingRootToken, "ConnectionRetryMilleseconds") ?? ConnectionRetryMilleseconds;
                RecipeRunRecipeInitialPath = (string?)Util.GetSettingValue(appSettingRootToken, "RecipeRunRecipeInitialPath");
                RecipeEditRecipeInitialPath = (string?)Util.GetSettingValue(appSettingRootToken, "RecipeEditRecipeInitialPath");
                RecipeLog1InitialPath = (string?)Util.GetSettingValue(appSettingRootToken, "RecipeLog1InitialPath");
                RecipeLog2InitialPath = (string?)Util.GetSettingValue(appSettingRootToken, "RecipeLog2InitialPath");

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
                File.WriteAllText(AppSettingFilePath, new JObject(new JProperty("LogFileDirectory", JsonConvert.SerializeObject(LogFileDirectory)), 
                    new JProperty("UnderFlowControlFallbackRatePercentage", JsonConvert.SerializeObject(UnderFlowControlFallbackRatePercentage)),
                    new JProperty("FloatingPointMaxNumberDigit", JsonConvert.SerializeObject(FloatingPointMaxNumberDigit)), new JProperty("PLCAddress", JsonConvert.SerializeObject(PLCAddress)),
                    new JProperty("PLCPort", JsonConvert.SerializeObject(PLCPort)), new JProperty("ConfigMode", JsonConvert.SerializeObject(ConfigMode)), 
                    new JProperty("PrecursorSourceMonitorLabel", JsonConvert.SerializeObject(PrecursorSourceMonitorLabel)), new JProperty("ConnectionRetryMilleseconds", JsonConvert.SerializeObject(ConnectionRetryMilleseconds)),
                    new JProperty("RecipeRunRecipeInitialPath", JsonConvert.SerializeObject(RecipeRunRecipeInitialPath)), new JProperty("RecipeEditRecipeInitialPath", JsonConvert.SerializeObject(RecipeEditRecipeInitialPath)),
                    new JProperty("RecipeLog1InitialPath", JsonConvert.SerializeObject(RecipeLog1InitialPath)), new JProperty("RecipeLog2InitialPath", JsonConvert.SerializeObject(RecipeLog2InitialPath))).ToString());
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
                logIntervalInRecipeRunIssuer.Publish(value);
            }
        }

        private static ObservableManager<int>.Publisher logIntervalInRecipeRunIssuer = ObservableManager<int>.Get("AppSetting.LogIntervalInRecipeRun");
        public static string LogFileDirectory = Util.GetAbsoluteFilePathFromAppRelativePath("Log");
        private static readonly int UnderFlowControlFallbackRatePercentage = 1;
        public static readonly float UnderFlowControlFallbackRate;
        public static uint ConnectionRetryMilleseconds = 1000;
        public static string? RecipeRunRecipeInitialPath;
        public static string? RecipeEditRecipeInitialPath;
        public static string? RecipeLog1InitialPath ;
        public static string? RecipeLog2InitialPath;
        public static string PLCAddress = "Local";
        public static int PLCPort = 851;
        public static bool ConfigMode = false;
        public static Dictionary<string, string> PrecursorSourceMonitorLabel = new Dictionary<string, string> { { "GasMonitor1", "H2" }, { "GasMonitor2", "N2" }, { "GasMonitor3", "SiH4" }, { "GasMonitor4", "NH3" },
            { "SourceMonitor1", "TEB" }, { "SourceMonitor2", "TMAl" }, { "SourceMonitor3", "TMIn" }, { "SourceMonitor4", "TMGa" },  { "SourceMonitor5", "DTMGa" }, { "SourceMonitor6", "Cp2Mg" }};
    }
}
