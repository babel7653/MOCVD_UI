using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.Models
{
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public class DevicePlc
  {
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		public byte[] aDigitalInputIO = new byte[9];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] aDigitalOutputIO = new byte[4];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
		public short[] aAnalogInputIO = new short[40];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
		public short[] aAnalogOutputIO = new short[28];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public ushort[] aAnalogInputIO2 = new ushort[2];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public int[] aOutputSolValve = new int[2];
	}

}
