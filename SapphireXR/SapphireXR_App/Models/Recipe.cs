using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SapphireXR_App.Models
{
    public partial class Recipe : ObservableObject
    {
        public Recipe()
        {
        }

        public Recipe(Recipe rhs)
        {
            Name = rhs.Name;
            cTemp = rhs.cTemp;
            No = rhs.No;
            hTime = rhs.hTime;
            Jump = rhs.Jump;
            rPress = rhs.rPress;
            sRotation = rhs.sRotation;
            Loop = rhs.Loop;
            rTime = rhs.rTime;
            sTemp = rhs.sTemp;
            E01 = rhs.E01;
            E02 = rhs.E02;
            E03 = rhs.E03;
            E04 = rhs.E04;
            E05 = rhs.E05;
            E06 = rhs.E06;
            E07 = rhs.E07;
            M01 = rhs.M01;
            M02 = rhs.M02;
            M03 = rhs.M03;
            M04 = rhs.M04;
            M05 = rhs.M05;
            M06 = rhs.M06;
            M07 = rhs.M07;
            M08 = rhs.M08;
            M09 = rhs.M09;
            M10 = rhs.M10;
            M11 = rhs.M11;
            M12 = rhs.M12;
            M13 = rhs.M13;
            M14 = rhs.M14;
            M15 = rhs.M15;
            M16 = rhs.M16;
            M17 = rhs.M17;
            M18 = rhs.M18;
            M19 = rhs.M19;
            V01 = rhs.V01;
            V02 = rhs.V02;
            V03 = rhs.V03;
            V04 = rhs.V04;
            V05 = rhs.V05;
            V07 = rhs.V07;
            V08 = rhs.V08;
            V10 = rhs.V10;
            V11 = rhs.V11;
            V13 = rhs.V13;
            V14 = rhs.V14;
            V16 = rhs.V16;
            V17 = rhs.V17;
            V19 = rhs.V19;
            V20 = rhs.V20;
            V22 = rhs.V22;
            V23 = rhs.V23;
            V24 = rhs.V24;
            V25 = rhs.V25;
            V26 = rhs.V26;
            V27 = rhs.V27;
            V28 = rhs.V28;
            V29 = rhs.V29;
            V30 = rhs.V30;
            V31 = rhs.V31;
            V32 = rhs.V32;
            Background = rhs.Background;
        }
        public string Name { get; set; } = "";
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

        Brush _background = Brushes.White;
        [Ignore]
        public Brush Background 
        {
            get { return _background; }
            set { SetProperty(ref _background, value); }
         }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class PlcRecipe
    {
        public PlcRecipe(Recipe rhs)
        {
            //Short Type Array
            aRecipeShort[0] = rhs.No;
            aRecipeShort[1] = rhs.rTime;
            aRecipeShort[2] = rhs.hTime;
            aRecipeShort[3] = rhs.sTemp;
            aRecipeShort[4] = rhs.rPress;
            aRecipeShort[5] = rhs.sRotation;
            aRecipeShort[6] = rhs.cTemp;
            aRecipeShort[7] = rhs.Loop;
            aRecipeShort[8] = rhs.Jump;
            //Float Type Array
            aRecipeFloat[0] = rhs.M01;
            aRecipeFloat[1] = rhs.M02;
            aRecipeFloat[2] = rhs.M03;
            aRecipeFloat[3] = rhs.M04;
            aRecipeFloat[4] = rhs.M05;
            aRecipeFloat[5] = rhs.M06;
            aRecipeFloat[6] = rhs.M07;
            aRecipeFloat[7] = rhs.M08;
            aRecipeFloat[8] = rhs.M09;
            aRecipeFloat[9] = rhs.M10;
            aRecipeFloat[10] = rhs.M11;
            aRecipeFloat[11] = rhs.M12;
            aRecipeFloat[12] = rhs.M13;
            aRecipeFloat[13] = rhs.M14;
            aRecipeFloat[14] = rhs.M15;
            aRecipeFloat[15] = rhs.M16;
            aRecipeFloat[16] = rhs.M17;
            aRecipeFloat[17] = rhs.M18;
            aRecipeFloat[18] = rhs.M19;
            aRecipeFloat[19] = rhs.E01;
            aRecipeFloat[20] = rhs.E02;
            aRecipeFloat[21] = rhs.E03;
            aRecipeFloat[22] = rhs.E04;
            aRecipeFloat[23] = rhs.E05;
            aRecipeFloat[24] = rhs.E06;
            aRecipeFloat[25] = rhs.E07;
            //BitArray from Valve Data
            BitArray aRecipeBit = new BitArray(32);
            aRecipeBit[0] = rhs.V01 ? true : false;
            aRecipeBit[1] = rhs.V02 ? true : false;
            aRecipeBit[2] = rhs.V03 ? true : false;
            aRecipeBit[3] = rhs.V04 ? true : false;
            aRecipeBit[4] = rhs.V05 ? true : false;
            aRecipeBit[5] = rhs.V07 ? true : false;
            aRecipeBit[6] = rhs.V08 ? true : false;
            aRecipeBit[7] = rhs.V10 ? true : false;
            aRecipeBit[8] = rhs.V11 ? true : false;
            aRecipeBit[9] = rhs.V13 ? true : false;
            aRecipeBit[10] = rhs.V14 ? true : false;
            aRecipeBit[11] = rhs.V16 ? true : false;
            aRecipeBit[12] = rhs.V17 ? true : false;
            aRecipeBit[13] = rhs.V19 ? true : false;
            aRecipeBit[14] = rhs.V20 ? true : false;
            aRecipeBit[15] = rhs.V22 ? true : false;
            aRecipeBit[16] = rhs.V23 ? true : false;
            aRecipeBit[17] = rhs.V24 ? true : false;
            aRecipeBit[18] = rhs.V25 ? true : false;
            aRecipeBit[19] = rhs.V26 ? true : false;
            aRecipeBit[20] = rhs.V27 ? true : false;
            aRecipeBit[21] = rhs.V28 ? true : false;
            aRecipeBit[22] = rhs.V29 ? true : false;
            aRecipeBit[23] = rhs.V30 ? true : false;
            aRecipeBit[24] = rhs.V31 ? true : false;
            aRecipeBit[25] = rhs.V32 ? true : false;

            sName = rhs.Name;

            if (aRecipeBit.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");
            int[] aValve = new int[1];
            aRecipeBit.CopyTo(aValve, 0);
            iValve = aValve[0];
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string sName = "";

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public short[] aRecipeShort = new short[9];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 29)]
        public float[] aRecipeFloat = new float[PLCService.NumControllers];

        public int iValve;
    }
}
