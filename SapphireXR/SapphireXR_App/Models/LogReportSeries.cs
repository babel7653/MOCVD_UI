﻿using OxyPlot;
using System.Drawing;

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
                    return recipeLog.PV_M01 / PLCService.ReadMaxValue("MFC01") * 100.0f;

                case nameof(RecipeLog.PV_M02):
                    return recipeLog.PV_M02 / PLCService.ReadMaxValue("MFC02") * 100.0f;

                case nameof(RecipeLog.PV_M03):
                    return recipeLog.PV_M03 / PLCService.ReadMaxValue("MFC03") * 100.0f;

                case nameof(RecipeLog.PV_M04):
                    return recipeLog.PV_M04 / PLCService.ReadMaxValue("MFC04") * 100.0f;

                case nameof(RecipeLog.PV_M05):
                    return recipeLog.PV_M05 / PLCService.ReadMaxValue("MFC05") * 100.0f;

                case nameof(RecipeLog.PV_M06):
                    return recipeLog.PV_M06 / PLCService.ReadMaxValue("MFC06") * 100.0f;

                case nameof(RecipeLog.PV_M07):
                    return recipeLog.PV_M07 / PLCService.ReadMaxValue("MFC07") * 100.0f;

                case nameof(RecipeLog.PV_M08):
                    return recipeLog.PV_M08 / PLCService.ReadMaxValue("MFC08") * 100.0f;

                case nameof(RecipeLog.PV_M09):
                    return recipeLog.PV_M09 / PLCService.ReadMaxValue("MFC09") * 100.0f;

                case nameof(RecipeLog.PV_M10):
                    return recipeLog.PV_M10 / PLCService.ReadMaxValue("MFC10") * 100.0f;

                case nameof(RecipeLog.PV_M11):
                    return recipeLog.PV_M11 / PLCService.ReadMaxValue("MFC11") * 100.0f;

                case nameof(RecipeLog.PV_M12):
                    return recipeLog.PV_M12 / PLCService.ReadMaxValue("MFC12") * 100.0f;

                case nameof(RecipeLog.PV_M13):
                    return recipeLog.PV_M13 / PLCService.ReadMaxValue("MFC13") * 100.0f;

                case nameof(RecipeLog.PV_M14):
                    return recipeLog.PV_M14 / PLCService.ReadMaxValue("MFC14") * 100.0f;

                case nameof(RecipeLog.PV_M15):
                    return recipeLog.PV_M15 / PLCService.ReadMaxValue("MFC15") * 100.0f;

                case nameof(RecipeLog.PV_M16):
                    return recipeLog.PV_M16 / PLCService.ReadMaxValue("MFC16") * 100.0f;

                case nameof(RecipeLog.PV_M17):
                    return recipeLog.PV_M17 / PLCService.ReadMaxValue("MFC17") * 100.0f;

                case nameof(RecipeLog.PV_M18):
                    return recipeLog.PV_M18 / PLCService.ReadMaxValue("MFC18") * 100.0f;

                case nameof(RecipeLog.PV_M19):
                    return recipeLog.PV_M19 / PLCService.ReadMaxValue("MFC19") * 100.0f;

                case nameof(RecipeLog.PV_E01):
                    return recipeLog.PV_E01 / PLCService.ReadMaxValue("EPC01") * 100.0f;

                case nameof(RecipeLog.PV_E02):
                    return recipeLog.PV_E02 / PLCService.ReadMaxValue("EPC02") * 100.0f;

                case nameof(RecipeLog.PV_E03):
                    return recipeLog.PV_E03 / PLCService.ReadMaxValue("EPC03") * 100.0f;

                case nameof(RecipeLog.PV_E04):
                    return recipeLog.PV_E04 / PLCService.ReadMaxValue("EPC04") * 100.0f;

                case nameof(RecipeLog.PV_E05):
                    return recipeLog.PV_E05 / PLCService.ReadMaxValue("EPC05") * 100.0f;

                case nameof(RecipeLog.PV_E06):
                    return recipeLog.PV_E06 / PLCService.ReadMaxValue("EPC06") * 100.0f;

                case nameof(RecipeLog.PV_E07):
                    return recipeLog.PV_E07 / PLCService.ReadMaxValue("EPC07") * 100.0f;

                case nameof(RecipeLog.PV_TEMP):
                    return recipeLog.PV_TEMP / PLCService.ReadMaxValue("Temperature") * 100.0f;

                case nameof(RecipeLog.PV_ROT):
                    return recipeLog.PV_ROT / PLCService.ReadMaxValue("Rotation") * 100.0f;

                case nameof(RecipeLog.PV_PRES):
                    return recipeLog.PV_PRES / PLCService.ReadMaxValue("Pressure") * 100.0f;

                case nameof(RecipeLog.PV_IHT_KW):
                    return recipeLog.PV_IHT_KW / 100.0f;

                case nameof(RecipeLog.PV_SH_CW):
                    return recipeLog.PV_SH_CW / 100.0f;

                case nameof(RecipeLog.PV_IHT_CW):
                    return recipeLog.PV_IHT_CW / 100.0f;

                case nameof(RecipeLog.SV_M01):
                    return recipeLog.SV_M01 / PLCService.ReadMaxValue("MFC01") * 100.0f;

                case nameof(RecipeLog.SV_M02):
                    return recipeLog.SV_M02 / PLCService.ReadMaxValue("MFC02") * 100.0f;

                case nameof(RecipeLog.SV_M03):
                    return recipeLog.SV_M03 / PLCService.ReadMaxValue("MFC03") * 100.0f;

                case nameof(RecipeLog.SV_M04):
                    return recipeLog.SV_M04 / PLCService.ReadMaxValue("MFC04") * 100.0f;

                case nameof(RecipeLog.SV_M05):
                    return recipeLog.SV_M05 / PLCService.ReadMaxValue("MFC05") * 100.0f;

                case nameof(RecipeLog.SV_M06):
                    return recipeLog.SV_M06 / PLCService.ReadMaxValue("MFC06") * 100.0f;

                case nameof(RecipeLog.SV_M07):
                    return recipeLog.SV_M07 / PLCService.ReadMaxValue("MFC07") * 100.0f;

                case nameof(RecipeLog.SV_M08):
                    return recipeLog.SV_M08 / PLCService.ReadMaxValue("MFC08") * 100.0f;

                case nameof(RecipeLog.SV_M09):
                    return recipeLog.SV_M09 / PLCService.ReadMaxValue("MFC09") * 100.0f;

                case nameof(RecipeLog.SV_M10):
                    return recipeLog.SV_M10 / PLCService.ReadMaxValue("MFC10") * 100.0f;

                case nameof(RecipeLog.SV_M11):
                    return recipeLog.SV_M11 / PLCService.ReadMaxValue("MFC11") * 100.0f;

                case nameof(RecipeLog.SV_M12):
                    return recipeLog.SV_M12 / PLCService.ReadMaxValue("MFC12") * 100.0f;

                case nameof(RecipeLog.SV_M13):
                    return recipeLog.SV_M13 / PLCService.ReadMaxValue("MFC13") * 100.0f;

                case nameof(RecipeLog.SV_M14):
                    return recipeLog.SV_M14 / PLCService.ReadMaxValue("MFC14") * 100.0f;

                case nameof(RecipeLog.SV_M15):
                    return recipeLog.SV_M15 / PLCService.ReadMaxValue("MFC15") * 100.0f;

                case nameof(RecipeLog.SV_M16):
                    return recipeLog.SV_M16 / PLCService.ReadMaxValue("MFC16") * 100.0f;

                case nameof(RecipeLog.SV_M17):
                    return recipeLog.SV_M17 / PLCService.ReadMaxValue("MFC17") * 100.0f;

                case nameof(RecipeLog.SV_M18):
                    return recipeLog.SV_M18 / PLCService.ReadMaxValue("MFC18") * 100.0f;

                case nameof(RecipeLog.SV_M19):
                    return recipeLog.SV_M19 / PLCService.ReadMaxValue("MFC19") * 100.0f;

                case nameof(RecipeLog.SV_E01):
                    return recipeLog.SV_E01 / PLCService.ReadMaxValue("EPC01") * 100.0f;

                case nameof(RecipeLog.SV_E02):
                    return recipeLog.SV_E02 / PLCService.ReadMaxValue("EPC02") * 100.0f;

                case nameof(RecipeLog.SV_E03):
                    return recipeLog.SV_E03 / PLCService.ReadMaxValue("EPC03") * 100.0f;

                case nameof(RecipeLog.SV_E04):
                    return recipeLog.SV_E04 / PLCService.ReadMaxValue("EPC04") * 100.0f;

                case nameof(RecipeLog.SV_E05):
                    return recipeLog.SV_E05 / PLCService.ReadMaxValue("EPC05") * 100.0f;

                case nameof(RecipeLog.SV_E06):
                    return recipeLog.SV_E06 / PLCService.ReadMaxValue("EPC06") * 100.0f;

                case nameof(RecipeLog.SV_E07):
                    return recipeLog.SV_E07 / PLCService.ReadMaxValue("EPC07") * 100.0f;

                case nameof(RecipeLog.SV_TEMP):
                    return recipeLog.SV_TEMP / PLCService.ReadMaxValue("Temperature") * 100.0f;

                case nameof(RecipeLog.SV_ROT):
                    return recipeLog.SV_ROT / PLCService.ReadMaxValue("Rotation") * 100.0f;

                case nameof(RecipeLog.SV_PRES):
                    return recipeLog.SV_PRES / PLCService.ReadMaxValue("Pressure") * 100.0f;

                default:
                    return null;
            }
        }

        public static float? GetMaxValue(string deviceName)
        {
            switch (deviceName)
            {
                case nameof(RecipeLog.PV_M01):
                    return PLCService.ReadMaxValue("MFC01");

                case nameof(RecipeLog.PV_M02):
                    return PLCService.ReadMaxValue("MFC02");

                case nameof(RecipeLog.PV_M03):
                    return PLCService.ReadMaxValue("MFC03");

                case nameof(RecipeLog.PV_M04):
                    return PLCService.ReadMaxValue("MFC04");

                case nameof(RecipeLog.PV_M05):
                    return PLCService.ReadMaxValue("MFC05");

                case nameof(RecipeLog.PV_M06):
                    return PLCService.ReadMaxValue("MFC06");

                case nameof(RecipeLog.PV_M07):
                    return PLCService.ReadMaxValue("MFC07");

                case nameof(RecipeLog.PV_M08):
                    return PLCService.ReadMaxValue("MFC08");

                case nameof(RecipeLog.PV_M09):
                    return PLCService.ReadMaxValue("MFC09");

                case nameof(RecipeLog.PV_M10):
                    return PLCService.ReadMaxValue("MFC10");

                case nameof(RecipeLog.PV_M11):
                    return PLCService.ReadMaxValue("MFC11");

                case nameof(RecipeLog.PV_M12):
                    return PLCService.ReadMaxValue("MFC12");

                case nameof(RecipeLog.PV_M13):
                    return PLCService.ReadMaxValue("MFC13");

                case nameof(RecipeLog.PV_M14):
                    return PLCService.ReadMaxValue("MFC14");

                case nameof(RecipeLog.PV_M15):
                    return  PLCService.ReadMaxValue("MFC15");

                case nameof(RecipeLog.PV_M16):
                    return PLCService.ReadMaxValue("MFC16");

                case nameof(RecipeLog.PV_M17):
                    return PLCService.ReadMaxValue("MFC17");

                case nameof(RecipeLog.PV_M18):
                    return PLCService.ReadMaxValue("MFC18");

                case nameof(RecipeLog.PV_M19):
                    return PLCService.ReadMaxValue("MFC19");

                case nameof(RecipeLog.PV_E01):
                    return PLCService.ReadMaxValue("EPC01");

                case nameof(RecipeLog.PV_E02):
                    return PLCService.ReadMaxValue("EPC02");

                case nameof(RecipeLog.PV_E03):
                    return PLCService.ReadMaxValue("EPC03");

                case nameof(RecipeLog.PV_E04):
                    return PLCService.ReadMaxValue("EPC04");

                case nameof(RecipeLog.PV_E05):
                    return PLCService.ReadMaxValue("EPC05");

                case nameof(RecipeLog.PV_E06):
                    return PLCService.ReadMaxValue("EPC06");

                case nameof(RecipeLog.PV_E07):
                    return PLCService.ReadMaxValue("EPC07");

                case nameof(RecipeLog.PV_TEMP):
                    return PLCService.ReadMaxValue("Temperature");

                case nameof(RecipeLog.PV_ROT):
                    return PLCService.ReadMaxValue("Rotation");

                case nameof(RecipeLog.PV_PRES):
                    return PLCService.ReadMaxValue("Pressure");

                case nameof(RecipeLog.PV_IHT_KW):
                    return 100.0f;

                case nameof(RecipeLog.PV_SH_CW):
                    return 100.0f;

                case nameof(RecipeLog.PV_IHT_CW):
                    return 100.0f;

                case nameof(RecipeLog.SV_M01):
                    return PLCService.ReadMaxValue("MFC01");

                case nameof(RecipeLog.SV_M02):
                    return PLCService.ReadMaxValue("MFC02");

                case nameof(RecipeLog.SV_M03):
                    return PLCService.ReadMaxValue("MFC03");

                case nameof(RecipeLog.SV_M04):
                    return PLCService.ReadMaxValue("MFC04");

                case nameof(RecipeLog.SV_M05):
                    return PLCService.ReadMaxValue("MFC05");

                case nameof(RecipeLog.SV_M06):
                    return PLCService.ReadMaxValue("MFC06");

                case nameof(RecipeLog.SV_M07):
                    return PLCService.ReadMaxValue("MFC07");

                case nameof(RecipeLog.SV_M08):
                    return PLCService.ReadMaxValue("MFC08");

                case nameof(RecipeLog.SV_M09):
                    return PLCService.ReadMaxValue("MFC09");

                case nameof(RecipeLog.SV_M10):
                    return PLCService.ReadMaxValue("MFC10");

                case nameof(RecipeLog.SV_M11):
                    return PLCService.ReadMaxValue("MFC11");

                case nameof(RecipeLog.SV_M12):
                    return PLCService.ReadMaxValue("MFC12");

                case nameof(RecipeLog.SV_M13):
                    return PLCService.ReadMaxValue("MFC13");

                case nameof(RecipeLog.SV_M14):
                    return PLCService.ReadMaxValue("MFC14");

                case nameof(RecipeLog.SV_M15):
                    return PLCService.ReadMaxValue("MFC15");

                case nameof(RecipeLog.SV_M16):
                    return PLCService.ReadMaxValue("MFC16");

                case nameof(RecipeLog.SV_M17):
                    return PLCService.ReadMaxValue("MFC17");

                case nameof(RecipeLog.SV_M18):
                    return PLCService.ReadMaxValue("MFC18");

                case nameof(RecipeLog.SV_M19):
                    return PLCService.ReadMaxValue("MFC19");

                case nameof(RecipeLog.SV_E01):
                    return PLCService.ReadMaxValue("EPC01");

                case nameof(RecipeLog.SV_E02):
                    return PLCService.ReadMaxValue("EPC02");

                case nameof(RecipeLog.SV_E03):
                    return PLCService.ReadMaxValue("EPC03");

                case nameof(RecipeLog.SV_E04):
                    return PLCService.ReadMaxValue("EPC04");

                case nameof(RecipeLog.SV_E05):
                    return PLCService.ReadMaxValue("EPC05");

                case nameof(RecipeLog.SV_E06):
                    return PLCService.ReadMaxValue("EPC06");

                case nameof(RecipeLog.SV_E07):
                    return PLCService.ReadMaxValue("EPC07");

                case nameof(RecipeLog.SV_TEMP):
                    return PLCService.ReadMaxValue("Temperature");

                case nameof(RecipeLog.SV_ROT):
                    return PLCService.ReadMaxValue("Rotation");

                case nameof(RecipeLog.SV_PRES):
                    return PLCService.ReadMaxValue("Pressure");

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
