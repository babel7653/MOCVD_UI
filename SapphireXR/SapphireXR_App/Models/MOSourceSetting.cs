using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireXR_App.Common;
using SapphireXR_App.Controls;
using System.IO;

namespace SapphireXR_App.Models
{
    static class MOSourceSetting
    {
        private class AppClosingSubscriber : IObserver<bool>
        {
            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                MOSourceSetting.Save();
            }
        }

        static MOSourceSetting()
        {
            try
            {
                var fdevice = File.ReadAllText(MOSourceFilePath);
                JToken? jDeviceInit = JToken.Parse(fdevice);
                if (jDeviceInit == null)
                {
                    throw new InvalidOperationException("MO Source setting file, " + MOSourceFilePath + " is invalid file");
                }

                JToken? jAbsoluteTemp = jDeviceInit["AbsoluteTemperature"];
                if (jAbsoluteTemp != null)
                {
                    AbsoluteTemp = JsonConvert.DeserializeObject<float>(jAbsoluteTemp.ToString());
                }

                JToken? jMOSource = jDeviceInit["MOSource"];
                if (jMOSource != null)
                {
                    MOSourceModels = JsonConvert.DeserializeObject<Dictionary<string, MOSourceModel>>(jMOSource.ToString()) ?? MOSourceModels;

                    //string errorList = string.Empty;
                    //foreach ((string sourceName, MOSourceModel? moSourceModel) in MOSourceModels)
                    //{
                    //    if (moSourceModel != null)
                    //    {
                    //        bool invalidSetting = true;
                    //        if (moSourceModel.MFC == null || PLCService.dIndexController.ContainsKey(moSourceModel.MFC) == false)
                    //        {
                    //            invalidSetting = false;
                    //        }
                    //        else
                    //        {
                    //            errorList += "\r\n" + sourceName + "과 연결된" + moSourceModel.MFC + "는 존재하지 않습니다.";
                    //        }
                    //        if (moSourceModel.EPC == null || PLCService.dIndexController.ContainsKey(moSourceModel.EPC) == false)
                    //        {
                    //            invalidSetting = false;
                    //        }
                    //        else
                    //        {
                    //            errorList += "\r\n" + sourceName + "과 연결된" + moSourceModel.EPC + "는 존재하지 않습니다.";
                    //        }
                    //        if (moSourceModel.Valve == null || PLCService.ValveIDtoOutputSolValveIdx1.ContainsKey(moSourceModel.Valve) == false || PLCService.ValveIDtoOutputSolValveIdx2.ContainsKey(moSourceModel.Valve) == false)
                    //        {
                    //            invalidSetting = false;
                    //        }
                    //        else
                    //        {
                    //            errorList += "\r\n" + sourceName + "과 연결된" + moSourceModel.MFC + "는 존재하지 않습니다.";
                    //        }

                    //        if (invalidSetting == false)
                    //        {
                    //            MOSourceModels[sourceName] = null;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception exception)
            {
                MOSourceSettingEnable = false;
                throw new Exception(exception.Message);
            }

            ObservableManager<bool>.Subscribe("App.Closing", appClosingSubscriber = new AppClosingSubscriber());
        }

        internal static void Read() { }

        internal static void Save()
        {
            JObject jDeviceIO = new JObject(new JProperty("AbsoluteTemperature", JsonConvert.SerializeObject(AbsoluteTemp)),
               new JProperty("MOSource", JsonConvert.SerializeObject(MOSourceModels)));
            File.WriteAllText(MOSourceFilePath, jDeviceIO.ToString());
        }

        internal static MOSourceModel? GetModel(string sourceName)
        {
            if(MOSourceModels.TryGetValue(sourceName, out var model) == true)
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        public static readonly string MOSourceFilePath = Util.GetResourceAbsoluteFilePath("/Configurations/MOSource.json");
        public static readonly float AbsoluteTemp = 273.15f;
        public static readonly Dictionary<string, MOSourceModel> MOSourceModels = [];
        public static bool MOSourceSettingEnable = true;

        private static AppClosingSubscriber appClosingSubscriber;
    }
}
