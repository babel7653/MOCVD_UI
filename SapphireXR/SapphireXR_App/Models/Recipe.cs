using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;

namespace SapphireXR_App.Models
{
    public class Recipe : ObservableObject
    {
        public string? Name { get; set; }
        // RecipeInt Array
        public short No { get; set; }
        public short rTime { get; set; }
        public short hTime { get; set; }
        public short sTemp { get; set; }
        public short rPress { get; set; }
        public short sRotation { get; set; }
        public short cTemp { get; set; }
        public short Loop { get; set; }
        public short Jump { get; set; }
        //RecipeFloat Array
        public float M01 { get; set; }
        public float M02 { get; set; }
        public float M03 { get; set; }
        public float M04 { get; set; }
        public float M05 { get; set; }
        public float M06 { get; set; }
        public float M07 { get; set; }
        public float M08 { get; set; }
        public float M09 { get; set; }
        public float M10 { get; set; }
        public float M11 { get; set; }
        public float M12 { get; set; }
        public float M13 { get; set; }
        public float M14 { get; set; }
        public float M15 { get; set; }
        public float M16 { get; set; }
        public float M17 { get; set; }
        public float M18 { get; set; }
        public float M19 { get; set; }
        public float E01 { get; set; }
        public float E02 { get; set; }
        public float E03 { get; set; }
        public float E04 { get; set; }
        public float E05 { get; set; }
        public float E06 { get; set; }
        public float E07 { get; set; }
        //RecipeDouble Array
        public bool V01 { get; set; }
        public bool V02 { get; set; }
        public bool V03 { get; set; }
        public bool V04 { get; set; }
        public bool V05 { get; set; }  //TEB
        public bool V07 { get; set; }
        public bool V08 { get; set; } //TMAl
        public bool V10 { get; set; }
        public bool V11 { get; set; } //TMIn
        public bool V13 { get; set; }
        public bool V14 { get; set; } //TMGa
        public bool V16 { get; set; }
        public bool V17 { get; set; } //DTMGa
        public bool V19 { get; set; }
        public bool V20 { get; set; } //Cp2Mg
        public bool V22 { get; set; }
        public bool V23 { get; set; } //TEB Manifold
        public bool V24 { get; set; } //TMAlManifold
        public bool V25 { get; set; } //TMIn Manifold
        public bool V26 { get; set; } //TMGa Manifold
        public bool V27 { get; set; } //DTMGa Manifold
        public bool V28 { get; set; } //Cp2Mg Manifold
        public bool V29 { get; set; } //NH3_1 Manifold
        public bool V30 { get; set; } //NH3_2 Manifold
        public bool V31 { get; set; } //SiH4 Manifold
        public bool V32 { get; set; } //Vent
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class PlcRecipe
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string sName = "";

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public short[] aRecipeShort = new short[9];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public float[] aRecipeFloat = new float[26];

        public int iValve;
    }
}
