using OxyPlot;
using System.Drawing;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Models
{
    static class LogReportSeries
    {
        public static float? GetFlowValue(RecipeLog recipeLog, string deviceName)
        {
            switch(deviceName)
            {
                case nameof(RecipeLog.PV_M01):
                    return recipeLog.PV_M01;

                case nameof(RecipeLog.PV_M02):
                    return recipeLog.PV_M02;

                case nameof(RecipeLog.PV_M03):
                    return recipeLog.PV_M03;

                case nameof(RecipeLog.PV_M04):
                    return recipeLog.PV_M04;

                case nameof(RecipeLog.PV_M05):
                    return recipeLog.PV_M05;

                case nameof(RecipeLog.PV_M06):
                    return recipeLog.PV_M06;

                case nameof(RecipeLog.PV_M07):
                    return recipeLog.PV_M07;

                case nameof(RecipeLog.PV_M08):
                    return recipeLog.PV_M08;

                case nameof(RecipeLog.PV_M09):
                    return recipeLog.PV_M09;

                case nameof(RecipeLog.PV_M10):
                    return recipeLog.PV_M10;

                case nameof(RecipeLog.PV_M11):
                    return recipeLog.PV_M11;

                case nameof(RecipeLog.PV_M12):
                    return recipeLog.PV_M12;

                case nameof(RecipeLog.PV_M13):
                    return recipeLog.PV_M13;

                case nameof(RecipeLog.PV_M14):
                    return recipeLog.PV_M14;

                case nameof(RecipeLog.PV_M15):
                    return recipeLog.PV_M15;

                case nameof(RecipeLog.PV_M16):
                    return recipeLog.PV_M16;

                case nameof(RecipeLog.PV_M17):
                    return recipeLog.PV_M17;

                case nameof(RecipeLog.PV_M18):
                    return recipeLog.PV_M18;

                case nameof(RecipeLog.PV_M19):
                    return recipeLog.PV_M19;

                case nameof(RecipeLog.PV_E01):
                    return recipeLog.PV_E01;

                case nameof(RecipeLog.PV_E02):
                    return recipeLog.PV_E02;

                case nameof(RecipeLog.PV_E03):
                    return recipeLog.PV_E03;

                case nameof(RecipeLog.PV_E04):
                    return recipeLog.PV_E04;

                case nameof(RecipeLog.PV_E05):
                    return recipeLog.PV_E05;

                case nameof(RecipeLog.PV_E06):
                    return recipeLog.PV_E06;

                case nameof(RecipeLog.PV_E07):
                    return recipeLog.PV_E07;

                case nameof(RecipeLog.PV_TEMP):
                    return recipeLog.PV_TEMP;

                case nameof(RecipeLog.PV_ROT):
                    return recipeLog.PV_ROT;

                case nameof(RecipeLog.PV_PRES):
                    return recipeLog.PV_PRES;

                case nameof(RecipeLog.PV_IHT_KW):
                    return recipeLog.PV_IHT_KW;

                case nameof(RecipeLog.PV_SH_CW):
                    return recipeLog.PV_SH_CW;

                case nameof(RecipeLog.PV_IHT_CW):
                    return recipeLog.PV_IHT_CW;

                case nameof(RecipeLog.SV_M01):
                    return recipeLog.SV_M01;

                case nameof(RecipeLog.SV_M02):
                    return recipeLog.SV_M02;

                case nameof(RecipeLog.SV_M03):
                    return recipeLog.SV_M03;

                case nameof(RecipeLog.SV_M04):
                    return recipeLog.SV_M04;

                case nameof(RecipeLog.SV_M05):
                    return recipeLog.SV_M05;

                case nameof(RecipeLog.SV_M06):
                    return recipeLog.SV_M06;

                case nameof(RecipeLog.SV_M07):
                    return recipeLog.SV_M07;

                case nameof(RecipeLog.SV_M08):
                    return recipeLog.SV_M08;

                case nameof(RecipeLog.SV_M09):
                    return recipeLog.SV_M09;

                case nameof(RecipeLog.SV_M10):
                    return recipeLog.SV_M10;

                case nameof(RecipeLog.SV_M11):
                    return recipeLog.SV_M11;

                case nameof(RecipeLog.SV_M12):
                    return recipeLog.SV_M12;

                case nameof(RecipeLog.SV_M13):
                    return recipeLog.SV_M13;

                case nameof(RecipeLog.SV_M14):
                    return recipeLog.SV_M14;

                case nameof(RecipeLog.SV_M15):
                    return recipeLog.SV_M15;

                case nameof(RecipeLog.SV_M16):
                    return recipeLog.SV_M16;

                case nameof(RecipeLog.SV_M17):
                    return recipeLog.SV_M17;

                case nameof(RecipeLog.SV_M18):
                    return recipeLog.SV_M18;

                case nameof(RecipeLog.SV_M19):
                    return recipeLog.SV_M19;

                case nameof(RecipeLog.SV_E01):
                    return recipeLog.SV_E01;

                case nameof(RecipeLog.SV_E02):
                    return recipeLog.SV_E02;

                case nameof(RecipeLog.SV_E03):
                    return recipeLog.SV_E03;

                case nameof(RecipeLog.SV_E04):
                    return recipeLog.SV_E04;

                case nameof(RecipeLog.SV_E05):
                    return recipeLog.SV_E05;

                case nameof(RecipeLog.SV_E06):
                    return recipeLog.SV_E06;

                case nameof(RecipeLog.SV_E07):
                    return recipeLog.SV_E07;

                case nameof(RecipeLog.SV_TEMP):
                    return recipeLog.SV_TEMP;

                case nameof(RecipeLog.SV_ROT):
                    return recipeLog.SV_ROT;

                case nameof(RecipeLog.SV_PRES):
                    return recipeLog.SV_PRES;

                default:
                    return null;
            }
        }
        public static float? GetFlowPercentageValue(RecipeLog recipeLog, string deviceName)
        {
            switch (deviceName)
            {
                case nameof(RecipeLog.PV_M01):
                    return recipeLog.PV_M01 / SettingViewModel.ReadMaxValue("MFC01") * 100.0f;

                case nameof(RecipeLog.PV_M02):
                    return recipeLog.PV_M02 / SettingViewModel.ReadMaxValue("MFC02") * 100.0f;

                case nameof(RecipeLog.PV_M03):
                    return recipeLog.PV_M03 / SettingViewModel.ReadMaxValue("MFC03") * 100.0f;

                case nameof(RecipeLog.PV_M04):
                    return recipeLog.PV_M04 / SettingViewModel.ReadMaxValue("MFC04") * 100.0f;

                case nameof(RecipeLog.PV_M05):
                    return recipeLog.PV_M05 / SettingViewModel.ReadMaxValue("MFC05") * 100.0f;

                case nameof(RecipeLog.PV_M06):
                    return recipeLog.PV_M06 / SettingViewModel.ReadMaxValue("MFC06") * 100.0f;

                case nameof(RecipeLog.PV_M07):
                    return recipeLog.PV_M07 / SettingViewModel.ReadMaxValue("MFC07") * 100.0f;

                case nameof(RecipeLog.PV_M08):
                    return recipeLog.PV_M08 / SettingViewModel.ReadMaxValue("MFC08") * 100.0f;

                case nameof(RecipeLog.PV_M09):
                    return recipeLog.PV_M09 / SettingViewModel.ReadMaxValue("MFC09") * 100.0f;

                case nameof(RecipeLog.PV_M10):
                    return recipeLog.PV_M10 / SettingViewModel.ReadMaxValue("MFC10") * 100.0f;

                case nameof(RecipeLog.PV_M11):
                    return recipeLog.PV_M11 / SettingViewModel.ReadMaxValue("MFC11") * 100.0f;

                case nameof(RecipeLog.PV_M12):
                    return recipeLog.PV_M12 / SettingViewModel.ReadMaxValue("MFC12") * 100.0f;

                case nameof(RecipeLog.PV_M13):
                    return recipeLog.PV_M13 / SettingViewModel.ReadMaxValue("MFC13") * 100.0f;

                case nameof(RecipeLog.PV_M14):
                    return recipeLog.PV_M14 / SettingViewModel.ReadMaxValue("MFC14") * 100.0f;

                case nameof(RecipeLog.PV_M15):
                    return recipeLog.PV_M15 / SettingViewModel.ReadMaxValue("MFC15") * 100.0f;

                case nameof(RecipeLog.PV_M16):
                    return recipeLog.PV_M16 / SettingViewModel.ReadMaxValue("MFC16") * 100.0f;

                case nameof(RecipeLog.PV_M17):
                    return recipeLog.PV_M17 / SettingViewModel.ReadMaxValue("MFC17") * 100.0f;

                case nameof(RecipeLog.PV_M18):
                    return recipeLog.PV_M18 / SettingViewModel.ReadMaxValue("MFC18") * 100.0f;

                case nameof(RecipeLog.PV_M19):
                    return recipeLog.PV_M19 / SettingViewModel.ReadMaxValue("MFC19") * 100.0f;

                case nameof(RecipeLog.PV_E01):
                    return recipeLog.PV_E01 / SettingViewModel.ReadMaxValue("EPC01") * 100.0f;

                case nameof(RecipeLog.PV_E02):
                    return recipeLog.PV_E02 / SettingViewModel.ReadMaxValue("EPC02") * 100.0f;

                case nameof(RecipeLog.PV_E03):
                    return recipeLog.PV_E03 / SettingViewModel.ReadMaxValue("EPC03") * 100.0f;

                case nameof(RecipeLog.PV_E04):
                    return recipeLog.PV_E04 / SettingViewModel.ReadMaxValue("EPC04") * 100.0f;

                case nameof(RecipeLog.PV_E05):
                    return recipeLog.PV_E05 / SettingViewModel.ReadMaxValue("EPC05") * 100.0f;

                case nameof(RecipeLog.PV_E06):
                    return recipeLog.PV_E06 / SettingViewModel.ReadMaxValue("EPC06") * 100.0f;

                case nameof(RecipeLog.PV_E07):
                    return recipeLog.PV_E07 / SettingViewModel.ReadMaxValue("EPC07") * 100.0f;

                case nameof(RecipeLog.PV_TEMP):
                    return recipeLog.PV_TEMP / SettingViewModel.ReadMaxValue("Temperature") * 100.0f;

                case nameof(RecipeLog.PV_ROT):
                    return recipeLog.PV_ROT / SettingViewModel.ReadMaxValue("Rotation") * 100.0f;

                case nameof(RecipeLog.PV_PRES):
                    return recipeLog.PV_PRES / SettingViewModel.ReadMaxValue("Pressure") * 100.0f;

                case nameof(RecipeLog.PV_IHT_KW):
                    return recipeLog.PV_IHT_KW / 100.0f;

                case nameof(RecipeLog.PV_SH_CW):
                    return recipeLog.PV_SH_CW / 100.0f;

                case nameof(RecipeLog.PV_IHT_CW):
                    return recipeLog.PV_IHT_CW / 100.0f;

                case nameof(RecipeLog.SV_M01):
                    return recipeLog.SV_M01 / SettingViewModel.ReadMaxValue("MFC01") * 100.0f;

                case nameof(RecipeLog.SV_M02):
                    return recipeLog.SV_M02 / SettingViewModel.ReadMaxValue("MFC02") * 100.0f;

                case nameof(RecipeLog.SV_M03):
                    return recipeLog.SV_M03 / SettingViewModel.ReadMaxValue("MFC03") * 100.0f;

                case nameof(RecipeLog.SV_M04):
                    return recipeLog.SV_M04 / SettingViewModel.ReadMaxValue("MFC04") * 100.0f;

                case nameof(RecipeLog.SV_M05):
                    return recipeLog.SV_M05 / SettingViewModel.ReadMaxValue("MFC05") * 100.0f;

                case nameof(RecipeLog.SV_M06):
                    return recipeLog.SV_M06 / SettingViewModel.ReadMaxValue("MFC06") * 100.0f;

                case nameof(RecipeLog.SV_M07):
                    return recipeLog.SV_M07 / SettingViewModel.ReadMaxValue("MFC07") * 100.0f;

                case nameof(RecipeLog.SV_M08):
                    return recipeLog.SV_M08 / SettingViewModel.ReadMaxValue("MFC08") * 100.0f;

                case nameof(RecipeLog.SV_M09):
                    return recipeLog.SV_M09 / SettingViewModel.ReadMaxValue("MFC09") * 100.0f;

                case nameof(RecipeLog.SV_M10):
                    return recipeLog.SV_M10 / SettingViewModel.ReadMaxValue("MFC10") * 100.0f;

                case nameof(RecipeLog.SV_M11):
                    return recipeLog.SV_M11 / SettingViewModel.ReadMaxValue("MFC11") * 100.0f;

                case nameof(RecipeLog.SV_M12):
                    return recipeLog.SV_M12 / SettingViewModel.ReadMaxValue("MFC12") * 100.0f;

                case nameof(RecipeLog.SV_M13):
                    return recipeLog.SV_M13 / SettingViewModel.ReadMaxValue("MFC13") * 100.0f;

                case nameof(RecipeLog.SV_M14):
                    return recipeLog.SV_M14 / SettingViewModel.ReadMaxValue("MFC14") * 100.0f;

                case nameof(RecipeLog.SV_M15):
                    return recipeLog.SV_M15 / SettingViewModel.ReadMaxValue("MFC15") * 100.0f;

                case nameof(RecipeLog.SV_M16):
                    return recipeLog.SV_M16 / SettingViewModel.ReadMaxValue("MFC16") * 100.0f;

                case nameof(RecipeLog.SV_M17):
                    return recipeLog.SV_M17 / SettingViewModel.ReadMaxValue("MFC17") * 100.0f;

                case nameof(RecipeLog.SV_M18):
                    return recipeLog.SV_M18 / SettingViewModel.ReadMaxValue("MFC18") * 100.0f;

                case nameof(RecipeLog.SV_M19):
                    return recipeLog.SV_M19 / SettingViewModel.ReadMaxValue("MFC19") * 100.0f;

                case nameof(RecipeLog.SV_E01):
                    return recipeLog.SV_E01 / SettingViewModel.ReadMaxValue("EPC01") * 100.0f;

                case nameof(RecipeLog.SV_E02):
                    return recipeLog.SV_E02 / SettingViewModel.ReadMaxValue("EPC02") * 100.0f;

                case nameof(RecipeLog.SV_E03):
                    return recipeLog.SV_E03 / SettingViewModel.ReadMaxValue("EPC03") * 100.0f;

                case nameof(RecipeLog.SV_E04):
                    return recipeLog.SV_E04 / SettingViewModel.ReadMaxValue("EPC04") * 100.0f;

                case nameof(RecipeLog.SV_E05):
                    return recipeLog.SV_E05 / SettingViewModel.ReadMaxValue("EPC05") * 100.0f;

                case nameof(RecipeLog.SV_E06):
                    return recipeLog.SV_E06 / SettingViewModel.ReadMaxValue("EPC06") * 100.0f;

                case nameof(RecipeLog.SV_E07):
                    return recipeLog.SV_E07 / SettingViewModel.ReadMaxValue("EPC07") * 100.0f;

                case nameof(RecipeLog.SV_TEMP):
                    return recipeLog.SV_TEMP / SettingViewModel.ReadMaxValue("Temperature") * 100.0f;

                case nameof(RecipeLog.SV_ROT):
                    return recipeLog.SV_ROT / SettingViewModel.ReadMaxValue("Rotation") * 100.0f;

                case nameof(RecipeLog.SV_PRES):
                    return recipeLog.SV_PRES / SettingViewModel.ReadMaxValue("Pressure") * 100.0f;

                default:
                    return null;
            }
        }

        public static float? GetMaxValue(string deviceName)
        {
            switch (deviceName)
            {
                case nameof(RecipeLog.PV_M01):
                    return SettingViewModel.ReadMaxValue("MFC01");

                case nameof(RecipeLog.PV_M02):
                    return SettingViewModel.ReadMaxValue("MFC02");

                case nameof(RecipeLog.PV_M03):
                    return SettingViewModel.ReadMaxValue("MFC03");

                case nameof(RecipeLog.PV_M04):
                    return SettingViewModel.ReadMaxValue("MFC04");

                case nameof(RecipeLog.PV_M05):
                    return SettingViewModel.ReadMaxValue("MFC05");

                case nameof(RecipeLog.PV_M06):
                    return SettingViewModel.ReadMaxValue("MFC06");

                case nameof(RecipeLog.PV_M07):
                    return SettingViewModel.ReadMaxValue("MFC07");

                case nameof(RecipeLog.PV_M08):
                    return SettingViewModel.ReadMaxValue("MFC08");

                case nameof(RecipeLog.PV_M09):
                    return SettingViewModel.ReadMaxValue("MFC09");

                case nameof(RecipeLog.PV_M10):
                    return SettingViewModel.ReadMaxValue("MFC10");

                case nameof(RecipeLog.PV_M11):
                    return SettingViewModel.ReadMaxValue("MFC11");

                case nameof(RecipeLog.PV_M12):
                    return SettingViewModel.ReadMaxValue("MFC12");

                case nameof(RecipeLog.PV_M13):
                    return SettingViewModel.ReadMaxValue("MFC13");

                case nameof(RecipeLog.PV_M14):
                    return SettingViewModel.ReadMaxValue("MFC14");

                case nameof(RecipeLog.PV_M15):
                    return  SettingViewModel.ReadMaxValue("MFC15");

                case nameof(RecipeLog.PV_M16):
                    return SettingViewModel.ReadMaxValue("MFC16");

                case nameof(RecipeLog.PV_M17):
                    return SettingViewModel.ReadMaxValue("MFC17");

                case nameof(RecipeLog.PV_M18):
                    return SettingViewModel.ReadMaxValue("MFC18");

                case nameof(RecipeLog.PV_M19):
                    return SettingViewModel.ReadMaxValue("MFC19");

                case nameof(RecipeLog.PV_E01):
                    return SettingViewModel.ReadMaxValue("EPC01");

                case nameof(RecipeLog.PV_E02):
                    return SettingViewModel.ReadMaxValue("EPC02");

                case nameof(RecipeLog.PV_E03):
                    return SettingViewModel.ReadMaxValue("EPC03");

                case nameof(RecipeLog.PV_E04):
                    return SettingViewModel.ReadMaxValue("EPC04");

                case nameof(RecipeLog.PV_E05):
                    return SettingViewModel.ReadMaxValue("EPC05");

                case nameof(RecipeLog.PV_E06):
                    return SettingViewModel.ReadMaxValue("EPC06");

                case nameof(RecipeLog.PV_E07):
                    return SettingViewModel.ReadMaxValue("EPC07");

                case nameof(RecipeLog.PV_TEMP):
                    return SettingViewModel.ReadMaxValue("Temperature");

                case nameof(RecipeLog.PV_ROT):
                    return SettingViewModel.ReadMaxValue("Rotation");

                case nameof(RecipeLog.PV_PRES):
                    return SettingViewModel.ReadMaxValue("Pressure");

                case nameof(RecipeLog.PV_IHT_KW):
                    return 100.0f;

                case nameof(RecipeLog.PV_SH_CW):
                    return 100.0f;

                case nameof(RecipeLog.PV_IHT_CW):
                    return 100.0f;

                case nameof(RecipeLog.SV_M01):
                    return SettingViewModel.ReadMaxValue("MFC01");

                case nameof(RecipeLog.SV_M02):
                    return SettingViewModel.ReadMaxValue("MFC02");

                case nameof(RecipeLog.SV_M03):
                    return SettingViewModel.ReadMaxValue("MFC03");

                case nameof(RecipeLog.SV_M04):
                    return SettingViewModel.ReadMaxValue("MFC04");

                case nameof(RecipeLog.SV_M05):
                    return SettingViewModel.ReadMaxValue("MFC05");

                case nameof(RecipeLog.SV_M06):
                    return SettingViewModel.ReadMaxValue("MFC06");

                case nameof(RecipeLog.SV_M07):
                    return SettingViewModel.ReadMaxValue("MFC07");

                case nameof(RecipeLog.SV_M08):
                    return SettingViewModel.ReadMaxValue("MFC08");

                case nameof(RecipeLog.SV_M09):
                    return SettingViewModel.ReadMaxValue("MFC09");

                case nameof(RecipeLog.SV_M10):
                    return SettingViewModel.ReadMaxValue("MFC10");

                case nameof(RecipeLog.SV_M11):
                    return SettingViewModel.ReadMaxValue("MFC11");

                case nameof(RecipeLog.SV_M12):
                    return SettingViewModel.ReadMaxValue("MFC12");

                case nameof(RecipeLog.SV_M13):
                    return SettingViewModel.ReadMaxValue("MFC13");

                case nameof(RecipeLog.SV_M14):
                    return SettingViewModel.ReadMaxValue("MFC14");

                case nameof(RecipeLog.SV_M15):
                    return SettingViewModel.ReadMaxValue("MFC15");

                case nameof(RecipeLog.SV_M16):
                    return SettingViewModel.ReadMaxValue("MFC16");

                case nameof(RecipeLog.SV_M17):
                    return SettingViewModel.ReadMaxValue("MFC17");

                case nameof(RecipeLog.SV_M18):
                    return SettingViewModel.ReadMaxValue("MFC18");

                case nameof(RecipeLog.SV_M19):
                    return SettingViewModel.ReadMaxValue("MFC19");

                case nameof(RecipeLog.SV_E01):
                    return SettingViewModel.ReadMaxValue("EPC01");

                case nameof(RecipeLog.SV_E02):
                    return SettingViewModel.ReadMaxValue("EPC02");

                case nameof(RecipeLog.SV_E03):
                    return SettingViewModel.ReadMaxValue("EPC03");

                case nameof(RecipeLog.SV_E04):
                    return SettingViewModel.ReadMaxValue("EPC04");

                case nameof(RecipeLog.SV_E05):
                    return SettingViewModel.ReadMaxValue("EPC05");

                case nameof(RecipeLog.SV_E06):
                    return SettingViewModel.ReadMaxValue("EPC06");

                case nameof(RecipeLog.SV_E07):
                    return SettingViewModel.ReadMaxValue("EPC07");

                case nameof(RecipeLog.SV_TEMP):
                    return SettingViewModel.ReadMaxValue("Temperature");

                case nameof(RecipeLog.SV_ROT):
                    return SettingViewModel.ReadMaxValue("Rotation");

                case nameof(RecipeLog.SV_PRES):
                    return SettingViewModel.ReadMaxValue("Pressure");

                default:
                    return null;
            }
        }


        private static OxyColor GenerateColor()
        {
            int index = Random.Shared.Next(AllKnownColors.Length);
            Color randomColor = Color.FromKnownColor(AllKnownColors[index]);

            return OxyColor.FromRgb(randomColor.R, randomColor.G, randomColor.B);
        }

        private static readonly KnownColor[] AllKnownColors = Enum.GetValues<KnownColor>();

        public static readonly Dictionary<string, (OxyColor, OxyColor)>  LogSeriesColor = new Dictionary<string, (OxyColor, OxyColor)> {
            { "PV_M01",( GenerateColor(),  GenerateColor()) }, { "SV_M01",( GenerateColor(),  GenerateColor()) },  { "PV_M02",( GenerateColor(),  GenerateColor()) }, { "SV_M02",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M03",( GenerateColor(),  GenerateColor()) },  { "SV_M03",( GenerateColor(),  GenerateColor()) }, { "PV_M04",( GenerateColor(),  GenerateColor()) }, { "SV_M04",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M05",( GenerateColor(),  GenerateColor()) }, { "SV_M05",( GenerateColor(),  GenerateColor()) }, { "PV_M06",( GenerateColor(),  GenerateColor()) }, { "SV_M06",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M07",( GenerateColor(),  GenerateColor()) }, { "SV_M07",( GenerateColor(),  GenerateColor()) }, { "PV_M08",( GenerateColor(),  GenerateColor()) }, { "SV_M08",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M09",( GenerateColor(),  GenerateColor()) },  { "SV_M09",( GenerateColor(),  GenerateColor()) }, { "PV_M10",( GenerateColor(),  GenerateColor()) }, { "SV_M10",( GenerateColor(),  GenerateColor()) },
            { "PV_M11",( GenerateColor(),  GenerateColor()) }, { "SV_M11",( GenerateColor(),  GenerateColor()) }, { "PV_M12",( GenerateColor(),  GenerateColor()) }, { "SV_M12",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M13",( GenerateColor(),  GenerateColor()) }, { "SV_M13",( GenerateColor(),  GenerateColor()) }, { "PV_M14",( GenerateColor(),  GenerateColor()) }, { "SV_M14",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M15",( GenerateColor(),  GenerateColor()) }, { "SV_M15",( GenerateColor(),  GenerateColor()) }, { "PV_M16",( GenerateColor(),  GenerateColor()) }, { "SV_M16",( GenerateColor(),  GenerateColor()) },
            { "PV_M17",( GenerateColor(),  GenerateColor()) }, { "SV_M17",( GenerateColor(),  GenerateColor()) }, { "PV_M18",( GenerateColor(),  GenerateColor()) }, { "SV_M18",( GenerateColor(),  GenerateColor()) },
            { "PV_M19",( GenerateColor(),  GenerateColor()) }, { "SV_M19",( GenerateColor(),  GenerateColor()) }, 
            { "PV_E01",( GenerateColor(),  GenerateColor()) }, { "SV_E01",( GenerateColor(),  GenerateColor()) }, { "PV_E02",( GenerateColor(),  GenerateColor()) }, { "SV_E02",( GenerateColor(),  GenerateColor()) },
            { "PV_E03",( GenerateColor(),  GenerateColor()) }, { "SV_E03",( GenerateColor(),  GenerateColor()) }, { "PV_E04",( GenerateColor(),  GenerateColor()) }, { "SV_E04",( GenerateColor(),  GenerateColor()) }, 
            { "PV_E05",( GenerateColor(),  GenerateColor()) }, { "SV_E05",( GenerateColor(),  GenerateColor()) }, { "PV_E06",( GenerateColor(),  GenerateColor()) }, { "SV_E06",( GenerateColor(),  GenerateColor()) },
            { "PV_E07",( GenerateColor(),  GenerateColor()) }, { "SV_E07",( GenerateColor(),  GenerateColor()) }, 
            { "PV_TEMP",( GenerateColor(),  GenerateColor()) }, { "SV_TEMP",( GenerateColor(),  GenerateColor()) }, { "PV_PRES",( GenerateColor(),  GenerateColor()) }, { "SV_PRES",( GenerateColor(),  GenerateColor()) }, 
            { "PV_ROT",( GenerateColor(),  GenerateColor()) }, { "SV_ROT",( GenerateColor(),  GenerateColor()) },  { "PV_IHT_KW",( GenerateColor(),  GenerateColor()) }, { "PV_SH_CW",( GenerateColor(),  GenerateColor()) }, { "PV_IHT_CW",( GenerateColor(),  GenerateColor()) }
        };
    }
}
