using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration.Attributes;
using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.Models
{
    public partial class Recipe : ObservableObject
    {
        public Recipe() 
        {
            initialize();
        }

        public Recipe(Recipe rhs)
        {
            initialize();

            No = rhs.No;
            Name = rhs.Name;
            cTemp = rhs.cTemp;
            HTime = rhs.HTime;
            LoopEndStep = rhs.LoopEndStep;
            RPress = rhs.RPress;
            SRotation = rhs.SRotation;
            LoopRepeat = rhs.LoopRepeat;
            RTime = rhs.RTime;
            STemp = rhs.STemp;
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
        }

        private void initialize()
        {
            PropertyChanged += (sender, args) =>
            {
                var constraintValue = (string fullName, float curValue) =>
                {
                    int? maxValue = SettingViewModel.ReadMaxValue(fullName) ?? 0;
                    if (maxValue < curValue)
                    {
                        maxValueExceedPublihser.Publish(fullName);
                        return (float)maxValue;
                    }
                    else
                    {
                        return curValue;
                    }
                };
                switch (args.PropertyName)
                {
                    case nameof(M01):
                        M01 = constraintValue("MFC01", M01);
                        break;

                    case nameof(M02):
                        M02 = constraintValue("MFC02", M02);
                        break;

                    case nameof(M03):
                        M03 = constraintValue("MFC03", M03);
                        break;

                    case nameof(M04):
                        M04 = constraintValue("MFC04", M04);
                        break;

                    case nameof(M05):
                        M05 = constraintValue("MFC05", M05);
                        break;

                    case nameof(M06):
                        M06 = constraintValue("MFC06", M06);
                        break;

                    case nameof(M07):
                        M07 = constraintValue("MFC07", M07);
                        break;

                    case nameof(M08):
                        M08 = constraintValue("MFC08", M08);
                        break;

                    case nameof(M09):
                        M09 = constraintValue("MFC09", M09);
                        break;

                    case nameof(M10):
                        M10 = constraintValue("MFC10", M10);
                        break;

                    case nameof(M11):
                        M11 = constraintValue("MFC11", M11);
                        break;

                    case nameof(M12):
                        M12 = constraintValue("MFC12", M12);
                        break;

                    case nameof(M13):
                        M13 = constraintValue("MFC13", M13);
                        break;

                    case nameof(M14):
                        M12 = constraintValue("MFC14", M14);
                        break;

                    case nameof(M15):
                        M12 = constraintValue("MFC15", M15);
                        break;

                    case nameof(M16):
                        M16 = constraintValue("MFC16", M16);
                        break;

                    case nameof(M17):
                        M17 = constraintValue("MFC17", M17);
                        break;

                    case nameof(M18):
                        M18 = constraintValue("MFC18", M18);
                        break;

                    case nameof(M19):
                        M19 = constraintValue("MFC19", M19);
                        break;

                    case nameof(E01):
                        E01 = constraintValue("EPC01", E01);
                        break;

                    case nameof(E02):
                        E02 = constraintValue("EPC02", E02);
                        break;

                    case nameof(E03):
                        E03 = constraintValue("EPC03", E03);
                        break;

                    case nameof(E04):
                        E04 = constraintValue("EPC04", E04);
                        break;

                    case nameof(E05):
                        E05 = constraintValue("EPC05", E05);
                        break;

                    case nameof(E06):
                        E06 = constraintValue("EPC06", E06);
                        break;

                    case nameof(E07):
                        E07 = constraintValue("EPC07", E07);
                        break;

                    case nameof(STemp):
                        STemp = (short)constraintValue("Temperature", STemp);
                        break;

                    case nameof(RPress):
                        RPress = (short)constraintValue("Pressure", RPress);
                        break;

                    case nameof(SRotation):
                        SRotation = (short)constraintValue("Rotation", SRotation);
                        break;
                }
            };
        }

        public string Name { get; set; } = "";
        // RecipeInt Array
        [ObservableProperty]
        public short no;
        [ObservableProperty]
        private short _rTime;
        [ObservableProperty]
        public short _hTime;
        [ObservableProperty]
        public short _sTemp;
        [ObservableProperty]
        public short _rPress;
        [ObservableProperty]
        public short _sRotation;
        public short cTemp { get; set; }
        public short LoopRepeat { get; set; }
        public short LoopEndStep { get; set; }
        //RecipeFloat Array
        [ObservableProperty]
        private float _m01;
        [ObservableProperty]
        private float _m02;
        [ObservableProperty]
        private float _m03;
        [ObservableProperty]
        private float _m04;
        [ObservableProperty]
        private float _m05;
        [ObservableProperty]
        private float _m06;
        [ObservableProperty]
        private float _m07;
        [ObservableProperty]
        private float _m08;
        [ObservableProperty]
        private float _m09;
        [ObservableProperty]
        private float _m10;
        [ObservableProperty]
        private float _m11;
        [ObservableProperty]
        private float _m12;
        [ObservableProperty]
        private float _m13;
        [ObservableProperty]
        private float _m14;
        [ObservableProperty]
        private float _m15;
        [ObservableProperty]
        private float _m16;
        [ObservableProperty]
        private float _m17;
        [ObservableProperty]
        private float _m18;
        [ObservableProperty]
        private float _m19;
        [ObservableProperty]
        private float _e01;
        [ObservableProperty]
        private float _e02;
        [ObservableProperty]
        private float _e03;
        [ObservableProperty]
        private float _e04;
        [ObservableProperty]
        private float _e05;
        [ObservableProperty]
        private float _e06;
        [ObservableProperty]
        private float _e07;
        //RecipeDouble Array
        [ObservableProperty]
        private bool _v01;
        [ObservableProperty]
        private bool _v02;
        [ObservableProperty]
        private bool _v03;
        [ObservableProperty]
        private bool _v04;
        [ObservableProperty]
        private bool _v05;  //TEB
        [ObservableProperty]
        private bool _v07;
        [ObservableProperty]
        private bool _v08; //TMAl
        [ObservableProperty]
        private bool _v10;
        [ObservableProperty]
        private bool _v11; //TMIn
        [ObservableProperty]
        private bool _v13;
        [ObservableProperty]
        private bool _v14; //TMGa
        [ObservableProperty]
        private bool _v16;
        [ObservableProperty]
        private bool _v17; //DTMGa
        [ObservableProperty]
        private bool _v19;
        [ObservableProperty]
        private bool _v20; //Cp2Mg
        [ObservableProperty]
        private bool _v22;
        [ObservableProperty]
        private bool _v23; //TEB Manifold
        [ObservableProperty]
        private bool _v24; //TMAlManifold
        [ObservableProperty]
        private bool _v25; //TMIn Manifold
        [ObservableProperty]
        private bool _v26; //TMGa Manifold
        [ObservableProperty]
        private bool _v27; //DTMGa Manifold
        [ObservableProperty]
        private bool _v28; //Cp2Mg Manifold
        [ObservableProperty]
        private bool _v29; //NH3_1 Manifold
        [ObservableProperty]
        private bool _v30; //NH3_2 Manifold
        [ObservableProperty]
        private bool _v31; //SiH4 Manifold
        [ObservableProperty]
        private bool _v32; //_vent

        public static readonly Brush DefaultBackground = Application.Current.FindResource("DefaultRecipeListBackground") as Brush ?? new SolidColorBrush(Color.FromRgb(0x16, 0x16, 0x16));
        public static readonly Brush DefaultForeground = Application.Current.FindResource("DefaultRecipeListForeground") as Brush ?? new SolidColorBrush(Color.FromRgb(0xC2, 0xC2, 0xC2));

        Brush _background = DefaultBackground;
        [Ignore]
        public Brush Background
        {
            get { return _background; }
            set { SetProperty(ref _background, value); }
        }

        bool _isEnabled = true;
        [Ignore]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        Brush _foreground = DefaultForeground;
        [Ignore]
        public Brush Foreground
        {
            get { return _foreground;  }
            set { SetProperty(ref _foreground, value);  }
        }

        [Ignore]
        public short JumpStride
        {
            set;
            get;
        } = 0;

        [Ignore]
        public short LoopCount
        {
            set;
            get;
        } = 0;

        [Ignore]
        private static readonly ObservableManager<string>.Publisher maxValueExceedPublihser = ObservableManager<string>.Get("Recipe.MaxValueExceed");
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class PlcRecipe
    {
        public PlcRecipe(Recipe rhs)
        {
            //Short Type Array
            aRecipeShort[0] = rhs.No;
            aRecipeShort[1] = rhs.RTime;
            aRecipeShort[2] = rhs.HTime;
            aRecipeShort[3] = rhs.STemp;
            aRecipeShort[4] = rhs.RPress;
            aRecipeShort[5] = rhs.SRotation;
            aRecipeShort[6] = rhs.cTemp;
            aRecipeShort[7] = rhs.JumpStride;
            aRecipeShort[8] = rhs.LoopCount;
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public float[] aRecipeFloat = new float[26];

        public int iValve;
    }
}
