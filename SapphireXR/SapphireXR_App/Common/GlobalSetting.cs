using System.Security.RightsManagement;

namespace SapphireXR_App.Common
{
    internal static class GlobalSetting
    {
        static GlobalSetting()
        {

        }

        public static readonly int DefaultLogIntervalInRecipeRunInMS = 1000;
        public static readonly int MaxNumberDigit = 4;

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

        private static ObservableManager<int>.DataIssuer logIntervalInRecipeRunIssuer = ObservableManager<int>.Get("GlobalSetting.LogIntervalInRecipeRun");
        public static string LogFileDirectory = "";
    }
}
