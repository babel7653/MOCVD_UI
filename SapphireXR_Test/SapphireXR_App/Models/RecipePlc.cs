using System.Runtime.InteropServices;

namespace SapphireXR_App.Models
{
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public class RecipePlc
	{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public short[] aRecipeShort = new short[10];

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
		public float[] aRecipeFloat = new float[26];

		public int iValve;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
    public string sName = "";
  }

}
