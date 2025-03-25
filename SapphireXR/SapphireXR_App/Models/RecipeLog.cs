﻿namespace SapphireXR_App.Models
{
    public class RecipeLog
    {
#pragma warning disable CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        public RecipeLog() { }
#pragma warning restore CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        public RecipeLog(Recipe recipe)
        {
            Step = recipe.Name;
            SV_M01 = recipe.M01;
            SV_M02 = recipe.M02;
            SV_M03 = recipe.M03;
            SV_M04 = recipe.M04;
            SV_M05 = recipe.M05;
            SV_M06 = recipe.M06;
            SV_M07 = recipe.M07;
            SV_M08 = recipe.M08;
            SV_M09 = recipe.M09;
            SV_M10 = recipe.M10;
            SV_M11 = recipe.M11;
            SV_M12 = recipe.M12;
            SV_M13 = recipe.M13;
            SV_M14 = recipe.M14;
            SV_M15 = recipe.M15;
            SV_M16 = recipe.M16;
            SV_M17 = recipe.M17;
            SV_M18 = recipe.M18;
            SV_M19 = recipe.M19;
            SV_E01 = recipe.E01;
            SV_E02 = recipe.E02;
            SV_E03 = recipe.E03;
            SV_E04 = recipe.E04;
            SV_E05 = recipe.E05;
            SV_E06 = recipe.E06;
            SV_E07 = recipe.E07;
            SV_TEMP = recipe.STemp;
            SV_PRES = recipe.RPress;
            SV_ROT = recipe.SRotation;

            PV_M01 = PLCService.ReadCurrentValue("MFC01");
            PV_M02 = PLCService.ReadCurrentValue("MFC02");
            PV_M03 = PLCService.ReadCurrentValue("MFC03");
            PV_M04 = PLCService.ReadCurrentValue("MFC04");
            PV_M05 = PLCService.ReadCurrentValue("MFC05");
            PV_M06 = PLCService.ReadCurrentValue("MFC06");
            PV_M07 = PLCService.ReadCurrentValue("MFC07");
            PV_M08 = PLCService.ReadCurrentValue("MFC08");
            PV_M09 = PLCService.ReadCurrentValue("MFC09");
            PV_M10 = PLCService.ReadCurrentValue("MFC10");
            PV_M11 = PLCService.ReadCurrentValue("MFC11");
            PV_M12 = PLCService.ReadCurrentValue("MFC12");
            PV_M13 = PLCService.ReadCurrentValue("MFC13");
            PV_M14 = PLCService.ReadCurrentValue("MFC14");
            PV_M15 = PLCService.ReadCurrentValue("MFC15");
            PV_M16 = PLCService.ReadCurrentValue("MFC16");
            PV_M17 = PLCService.ReadCurrentValue("MFC17");
            PV_M18 = PLCService.ReadCurrentValue("MFC18");
            PV_M19 = PLCService.ReadCurrentValue("MFC19");
            PV_E01 = PLCService.ReadCurrentValue("EPC01");
            PV_E02 = PLCService.ReadCurrentValue("EPC02");
            PV_E03 = PLCService.ReadCurrentValue("EPC03");
            PV_E04 = PLCService.ReadCurrentValue("EPC04");
            PV_E05 = PLCService.ReadCurrentValue("EPC05");
            PV_E06 = PLCService.ReadCurrentValue("EPC06");
            PV_E07 = PLCService.ReadCurrentValue("EPC07");
            PV_TEMP = PLCService.ReadCurrentValue("Temperature");
            PV_PRES = PLCService.ReadCurrentValue("Pressure");
            PV_ROT = PLCService.ReadCurrentValue("Rotation");

            LogTime = DateTime.Now;
        }

        public string Step { get; set; }
        public float PV_M01 { get; set; }
        public float PV_M02 { get; set; }
        public float PV_M03 { get; set; }
        public float PV_M04 { get; set; }
        public float PV_M05 { get; set; }
        public float PV_M06 { get; set; }
        public float PV_M07 { get; set; }
        public float PV_M08 { get; set; }
        public float PV_M09 { get; set; }
        public float PV_M10 { get; set; }
        public float PV_M11 { get; set; }
        public float PV_M12 { get; set; }
        public float PV_M13 { get; set; }
        public float PV_M14 { get; set; }
        public float PV_M15 { get; set; }
        public float PV_M16 { get; set; }
        public float PV_M17 { get; set; }
        public float PV_M18 { get; set; }
        public float PV_M19 { get; set; }
        public float PV_E01 { get; set; }
        public float PV_E02 { get; set; }
        public float PV_E03 { get; set; }
        public float PV_E04 { get; set; }
        public float PV_E05 { get; set; }
        public float PV_E06 { get; set; }
        public float PV_E07 { get; set; }
        public float PV_TEMP { get; set; }
        public float PV_PRES { get; set; }
        public float PV_ROT { get; set; }
        public float PV_IHT_KW { get; set; }
        public float PV_SH_CW { get; set; }
        public float PV_IHT_CW { get; set; }
        public float SV_M01 { get; set; }
        public float SV_M02 { get; set; }
        public float SV_M03 { get; set; }
        public float SV_M04 { get; set; }
        public float SV_M05 { get; set; }
        public float SV_M06 { get; set; }
        public float SV_M07 { get; set; }
        public float SV_M08 { get; set; }
        public float SV_M09 { get; set; }
        public float SV_M10 { get; set; }
        public float SV_M11 { get; set; }
        public float SV_M12 { get; set; }
        public float SV_M13 { get; set; }
        public float SV_M14 { get; set; }
        public float SV_M15 { get; set; }
        public float SV_M16 { get; set; }
        public float SV_M17 { get; set; }
        public float SV_M18 { get; set; }
        public float SV_M19 { get; set; }
        public float SV_E01 { get; set; }
        public float SV_E02 { get; set; }
        public float SV_E03 { get; set; }
        public float SV_E04 { get; set; }
        public float SV_E05 { get; set; }
        public float SV_E06 { get; set; }
        public float SV_E07 { get; set; }
        public float SV_TEMP { get; set; }
        public float SV_PRES { get; set; }
        public float SV_ROT { get; set; }
                          
        public DateTime LogTime { get; set; }
    }
}
