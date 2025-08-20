using SapphireXR_App.Enums;
using System.Collections;
using System.Windows;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        private static void ReadStateFromPLC(object? sender, EventArgs e)
        {
            try
            {
                ReadCurrentValueFromPLC();
                if (aDeviceControlValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlValueIssuers?[kv.Key].Publish(aDeviceControlValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dCurrentValueIssuers?[kv.Key].Publish(aDeviceCurrentValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceControlValues != null && aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlCurrentValueIssuers?[kv.Key].Publish((aDeviceCurrentValues[dIndexController[kv.Key]], aDeviceControlValues[dIndexController[kv.Key]]));
                    }
                }

                if (aMonitoring_PVs != null)
                {
                    foreach (KeyValuePair<string, int> kv in dMonitoringMeterIndex)
                    {
                        aMonitoringCurrentValueIssuers?[kv.Key].Publish(aMonitoring_PVs[kv.Value]);
                    }
                }

                if (aInputState != null)
                {
                    short value = aInputState[0];
                    baHardWiringInterlockStateIssuers?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray()));
                    dThrottleValveStatusIssuer?.Publish(aInputState[4]);

                    bool[] ioList = new bool[64];
                    for (int inputState = 1; inputState < aInputState.Length; ++inputState)
                    {
                        new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(aInputState[inputState]) : BitConverter.GetBytes(aInputState[inputState]).Reverse().ToArray()).CopyTo(ioList, (inputState - 1) * sizeof(short) * 8);
                    }
                    dIOStateList?.Publish(new BitArray(ioList));
                }

                if (baReadValveStatePLC1 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx1)
                    {
                        dValveStateIssuers?[valveID].Publish(baReadValveStatePLC1[index]);
                    }
                }
                if (baReadValveStatePLC2 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx2)
                    {
                        dValveStateIssuers?[valveID].Publish(baReadValveStatePLC2[index]);
                    }
                }
                dLineHeaterTemperatureIssuers?.Publish(Ads.ReadAny<float[]>(hTemperaturePV, [(int)LineHeaterTemperature]));

                byte[] digitalOutput = Ads.ReadAny<byte[]>(hDigitalOutput, [4]);
                dDigitalOutput2?.Publish(new BitArray(new byte[1] { digitalOutput[1] }));
                dDigitalOutput3?.Publish(new BitArray(new byte[1] { digitalOutput[2] }));
                short[] outputCmd = Ads.ReadAny<short[]>(hOutputCmd, [3]);
                dOutputCmd1?.Publish(bOutputCmd1 = new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd[0]) : BitConverter.GetBytes(outputCmd[0]).Reverse().ToArray()));
                dThrottleValveControlMode?.Publish(outputCmd[1]);
                ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
                dInputManAuto?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray()));
                dPressureControlModeIssuer?.Publish(Ads.ReadAny<ushort>(hOutputSetType));

                int iterlock1 = Ads.ReadAny<int>(hInterlock[0]);
                dLogicalInterlockStateIssuer?.Publish(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(iterlock1) : BitConverter.GetBytes(iterlock1).Reverse().ToArray()));

                foreach (Action task in AddOnPLCStateUpdateTask)
                {
                    task();
                }

                string exceptionStr = string.Empty;
                if (aDeviceControlValues == null)
                {
                    exceptionStr += "aDeviceControlValues is null in OnTick PLCService";
                }
                if (aDeviceCurrentValues == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aDeviceCurrentValues is null in OnTick PLCService";
                }
                if (aMonitoring_PVs == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aMonitoring_PVs is null in OnTick PLCService";
                }
                if (baReadValveStatePLC1 == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "baReadValveStatePLC1 is null in OnTick PLCService";
                }
                if (baReadValveStatePLC2 == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "baReadValveStatePLC2 is null in OnTick PLCService";
                }
                if (exceptionStr != string.Empty)
                {
                    throw new ReadBufferException(exceptionStr);
                }
            }
            catch (ReadBufferException exception)
            {
                if (ShowMessageOnOnTick == true)
                {
                    ShowMessageOnOnTick = MessageBox.Show("PLC로부터 상태 (Analog Device Control/Valve 상태)를 읽어오는데 실패했습니다. 이 메시지를 다시 표시하지 않으려면 Yes를 클릭하세요. 원인은 다음과 같습니다: " + exception.Message, "",
                        MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes ? false : true;
                }
            }
            catch (Exception)
            {
                Connected = PLCConnection.Disconnected;
            }
        }

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
