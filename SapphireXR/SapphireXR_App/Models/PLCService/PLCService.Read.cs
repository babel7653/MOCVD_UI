using System.Collections;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        private static void ReadValveStateFromPLC()
        {
            uint aReadValveStatePLC1 = (uint)Ads.ReadAny(hReadValveStatePLC1, typeof(uint)); // Convert to Array
            uint aReadValveStatePLC2 = (uint)Ads.ReadAny(hReadValveStatePLC2, typeof(uint)); // Convert to Array

            baReadValveStatePLC1 = new BitArray([(int)aReadValveStatePLC1]);
            baReadValveStatePLC2 = new BitArray([(int)aReadValveStatePLC2]);
        }

        private static void ReadInitialStateValueFromPLC()
        {
            ReadValveStateFromPLC();
            ReadCurrentValueFromPLC();
        }

        private static void ReadCurrentValueFromPLC()
        {
            aDeviceCurrentValues = Ads.ReadAny<float[]>(hDeviceCurrentValuePLC, [NumControllers]);
            aDeviceControlValues = Ads.ReadAny<float[]>(hDeviceControlValuePLC, [NumControllers]);
            aMonitoring_PVs = Ads.ReadAny<float[]>(hMonitoring_PV, [18]);
            aInputState = Ads.ReadAny<short[]>(hInputState, [5]);
            ReadValveStateFromPLC();
        }

        public static float ReadCurrentValue(string controllerID)
        {
            if (aDeviceCurrentValues != null)
            {
                return aDeviceCurrentValues[dIndexController[controllerID]];
            }
            else
            {
                return float.NaN;
            }
        }

        public static short ReadUserState()
        {
            int length = userStateBuffer.Length;
            Ads.Read(hUserState, userStateBuffer);
            return BitConverter.ToInt16(userStateBuffer.Span);
        }

        public static BitArray ReadOutputCmd1()
        {
            short outputCmd1 = Ads.ReadAny<short>(hOutputCmd1);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd1) : BitConverter.GetBytes(outputCmd1).Reverse().ToArray());
        }

        public static ushort ReadPressureControlMode()
        {
            return Ads.ReadAny<ushort>(hOutputSetType);
        }

        public static ushort ReadThrottleValveMode()
        {
            return Ads.ReadAny<ushort>(hOutputMode);
        }

        public static bool ReadInputManAuto(int index)
        {
            return ReadBit(Ads.ReadAny<ushort>(hE3508InputManAuto), index);
        }

        public static bool ReadDigitalOutputIO2(int bitIndex)
        {
            return new BitArray(new byte[1] { Ads.ReadAny<byte>(hDigitalOutput2) })[bitIndex];
        }

        public static bool ReadInputState4(int bitIndex)
        {
            short inputState4 = Ads.ReadAny<short>(hInputState4);
            return new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputState4) : BitConverter.GetBytes(inputState4).Reverse().ToArray())[bitIndex];
        }

        public static bool ReadBit(int bitField, int bit)
        {
            int bitMask = 1 << bit;
            return ((bitField & bitMask) != 0) ? true : false;
        }

        public static bool ReadBuzzerOnOff()
        {
            return ReadBit(Ads.ReadAny<int>(hInterlockEnable[0]), 2);
        }

        public static int ReadDigitalDeviceAlarms()
        {
            return Ads.ReadAny<int>(hInterlock[1]);
        }

        public static int ReadAnalogDeviceAlarms()
        {
            return Ads.ReadAny<int>(hInterlock[2]);
        }

        public static int ReadDigitalDeviceWarnings()
        {
            return Ads.ReadAny<int>(hInterlock[3]);
        }

        public static int ReadAnalogDeviceWarnings()
        {
            return Ads.ReadAny<int>(hInterlock[4]);
        }

        public static bool ReadRecipeStartAvailable()
        {
            return ReadBit(Ads.ReadAny<int>(hInterlock[0]), 10);
        }

        public static bool ReadAlarmTriggered()
        {
            return ReadBit(Ads.ReadAny<int>(hInterlock[0]), 0);
        }

        public static ControlMode ReadControlMode()
        {
            return (ControlMode)(Ads.ReadAny<short>(hControlMode));
        }
    }
}
