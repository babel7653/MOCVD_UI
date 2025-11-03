using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Collections;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        private static void DoWriteValveState(string valveID, bool onOff)
        {
            (BitArray buffer, int index, uint variableHandle) = GetBuffer(valveID);
            buffer[index] = onOff;

            uint[] sentBuffer = new uint[1];
            buffer.CopyTo(sentBuffer, 0);
            Ads.WriteAny(variableHandle, sentBuffer);
        }

        public static void AddCoupledValves(string leftValveID, string rightValveID)
        {
            LeftCoupled[rightValveID] = leftValveID;
            RightCoupled[leftValveID] = rightValveID;
        }

        public static void WriteValveState(string valveID, bool onOff)
        {
            if (LeakTestMode == false)
            {
                string? coupled = null;
                if (RightCoupled.TryGetValue(valveID, out coupled) == true)
                {
                    DoWriteValveState(coupled, onOff);
                }
            }
            else
            {
                string? coupled = null;
                if (LeftCoupled.TryGetValue(valveID, out coupled) == true)
                {
                    if (onOff == true)
                    {
                        DoWriteValveState(coupled, onOff);
                    }
                }
                else
                    if (RightCoupled.TryGetValue(valveID, out coupled) == true)
                {
                    if (onOff == false)
                    {
                        DoWriteValveState(coupled, onOff);
                    }
                }
            }
            DoWriteValveState(valveID, onOff);
        }

        public static void WriteValveState(BitArray firstValveParts, BitArray secondValveParts)
        {
            var doWrite = (uint variableHandle, BitArray valveParts) =>
            {
                uint[] buffer = new uint[1];
                valveParts.CopyTo(buffer, 0);
                Ads.WriteAny(variableHandle, buffer);
            };

            doWrite(hReadValveStatePLC1, firstValveParts);
            doWrite(hReadValveStatePLC2, secondValveParts);
        }

        public static void WriteRecipe(PlcRecipe[] recipe)
        {
            Ads.WriteAny(hRcp, recipe, [recipe.Length]);
        }

        public static void RefreshRecipe(PlcRecipe[] updates)
        {
            foreach (PlcRecipe recipe in updates)
            {
                Ads.WriteAny(Ads.CreateVariableHandle("RCP.aRecipe[" + recipe.aRecipeShort[0] + "]"), recipe);
            }
        }

        public static void WriteTotalStep(short totalStep)
        {
            Ads.WriteAny(hRcpTotalStep, totalStep);
        }

        public static void WriteRCPOperationCommand(short operationState)
        {
            Ads.WriteAny(hCmd_RcpOperation, operationState);
        }

        public static void WriteControlModeCmd(ControlMode controlMode)
        {
            dControlModeChangingPublisher?.Publish(controlMode);
            Ads.WriteAny(hControlModeCmd, (short)controlMode);
        }

        public static void WriteOutputCmd1(OutputCmd1Index index, bool powerOn)
        {
            if (bOutputCmd1 != null)
            {
                bOutputCmd1[(int)index] = powerOn;
                int[] array = new int[1];
                bOutputCmd1.CopyTo(array, 0);
                Ads.WriteAny(hOutputCmd1, (short)array[0]);
            }
        }

        public static void WriteThrottleValveMode(short value)
        {
            Ads.WriteAny(hOutputCmd2, value);
        }

        private static int SetBit(bool bitValue, int bitField, int bit)
        {
            int invMask = ~(1 << bit);
            bitField &= invMask;
            bitField |= (bitValue ? 1 : 0) << bit;

            return bitField;
        }

        private static bool WriteDeviceAlarmWarningSettingState(string deviceID, int index, bool bitValue, Dictionary<string, int> deviceIDToBit)
        {
            int bit;
            if (deviceIDToBit.TryGetValue(deviceID, out bit) == true)
            {
                InterlockEnables[index] = SetBit(bitValue, InterlockEnables[index], bit);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void WriteAnalogDeviceAlarmState(string deviceID, bool bitValue)
        {
            WriteDeviceAlarmWarningSettingState(deviceID, 1, bitValue, dAnalogDeviceAlarmWarningBit);
            InterlockEnableLowerIndiceToCommit.Add(1);
        }

        public static void WriteAnalogDeviceWarningState(string deviceID, bool bitValue)
        {
            WriteDeviceAlarmWarningSettingState(deviceID, 2, bitValue, dAnalogDeviceAlarmWarningBit);
            InterlockEnableLowerIndiceToCommit.Add(2);
        }

        public static void WriteDigitalDeviceAlarmState(string deviceID, bool bitValue)
        {
            WriteDeviceAlarmWarningSettingState(deviceID, 3, bitValue, dDigitalDeviceAlarmWarningBit);
            InterlockEnableUpperIndiceToCommit.Add(3);
        }

        public static void WriteDigitalDeviceWarningState(string deviceID, bool bitValue)
        {
            WriteDeviceAlarmWarningSettingState(deviceID, 4, bitValue, dDigitalDeviceAlarmWarningBit);
            InterlockEnableUpperIndiceToCommit.Add(4);
        }

        private static void CommitAlarmWarningSettingStateToPLC(HashSet<int> interlockEnableIndiceToCommit)
        {
            foreach (int index in interlockEnableIndiceToCommit)
            {
                Ads.WriteAny(hInterlockEnable[index], InterlockEnables[index]);
            }
            interlockEnableIndiceToCommit.Clear();
        }

        public static void CommitAnalogDeviceAlarmWarningSettingStateToPLC()
        {
            CommitAlarmWarningSettingStateToPLC(InterlockEnableLowerIndiceToCommit);
            CommitAnalogDeviceInterlockSettingToPLC();
        }

        public static void CommitDigitalDeviceAlarmWarningSettingStateToPLC()
        {
            CommitAlarmWarningSettingStateToPLC(InterlockEnableUpperIndiceToCommit);
            CommitDigitalDeviceInterlockSettingToPLC();
        }

        public static void WriteAlarmWarningSetting(List<AnalogDeviceIO> analogDeviceIOs, List<SwitchDI> switchDIs)
        {
            var setBit = (string deviceID, int index, bool bitValue, Dictionary<string, int> deviceIDToBit) =>
            {
                int bit;
                if (deviceIDToBit.TryGetValue(deviceID, out bit) == true)
                {
                    InterlockEnables[index] = SetBit(bitValue, InterlockEnables[index], bit);
                }
            };

            foreach (AnalogDeviceIO analogDeviceIO in analogDeviceIOs)
            {
                if (analogDeviceIO.ID != null)
                {
                    setBit(analogDeviceIO.ID, 1, analogDeviceIO.AlarmSet, dAnalogDeviceAlarmWarningBit);
                    setBit(analogDeviceIO.ID, 2, analogDeviceIO.WarningSet, dAnalogDeviceAlarmWarningBit);
                }
            }
            foreach (SwitchDI switchID in switchDIs)
            {
                if (switchID.ID != null)
                {
                    setBit(switchID.ID, 3, switchID.AlarmSet, dDigitalDeviceAlarmWarningBit);
                    setBit(switchID.ID, 4, switchID.WarningSet, dDigitalDeviceAlarmWarningBit);
                }
            }

            for (uint alarmWarningSettingIndex = 1; alarmWarningSettingIndex < (NumAlarmWarningArraySize - 1); alarmWarningSettingIndex++)
            {
                Ads.WriteAny(hInterlockEnable[alarmWarningSettingIndex], InterlockEnables[alarmWarningSettingIndex]);
            }
        }

        public static void WriteAlarmDeviationState(float deviation)
        {
            AnalogDeviceInterlockSetIndiceToCommit[0] = deviation;
        }

        public static void WriteWarningDeviationState(float deviation)
        {
            AnalogDeviceInterlockSetIndiceToCommit[1] = deviation;
        }

        public static void WriteAnalogDeviceDelayTime(float delayTime)
        {
            AnalogDeviceInterlockSetIndiceToCommit[2] = delayTime;
        }

        public static void WriteDigitalDeviceDelayTime(float delayTime)
        {
            DigitalDevicelnterlockSetToCommit = (false, delayTime);
        }

        public static void CommitAnalogDeviceInterlockSettingToPLC()
        {
            CommitInterlockSetToPLC(AnalogDeviceInterlockSetIndiceToCommit);
        }

        public static void CommitDigitalDeviceInterlockSettingToPLC()
        {
            if (DigitalDevicelnterlockSetToCommit.Item1 == false)
            {
                Ads.WriteAny(hInterlockset[3], DigitalDevicelnterlockSetToCommit.Item2);
                DigitalDevicelnterlockSetToCommit.Item1 = true;
            }
        }

        private static void CommitInterlockSetToPLC(Dictionary<int, float> interlockSetIndiceToCommit)
        {
            foreach ((int index, float setValue) in interlockSetIndiceToCommit)
            {
                Ads.WriteAny(hInterlockset[index], setValue);
            }
            interlockSetIndiceToCommit.Clear();
        }

        private static void WriteFirstInterlockSetting(bool onOff, int bit)
        {
            InterlockEnables[0] = SetBit(onOff, InterlockEnables[0], bit);
            Ads.WriteAny(hInterlockEnable[0], InterlockEnables[0]);
        }

        public static void WriteBuzzerOnOff(bool onOff)
        {
            WriteFirstInterlockSetting(onOff, 2);
        }

        public static void WriteInterlockEnableState(bool onOff, InterlockEnableSetting interlockEnableSetting)
        {
            InterlockEnables[0] = SetBit(onOff, InterlockEnables[0], (int)interlockEnableSetting);
        }

        public static void CommitInterlockEnableToPLC()
        {
            Ads.WriteAny(hInterlockEnable[0], InterlockEnables[0]);
        }

        public static void WriteInterlockValueState(float value, InterlockValueSetting interlockEnableSetting)
        {
            InterlockSetIndiceToCommit[((int)interlockEnableSetting) - 1] = value;
        }

        public static void CommitInterlockValueToPLC()
        {
            CommitInterlockSetToPLC(InterlockSetIndiceToCommit);
        }

        public static void WriteAlarmReset()
        {
            WriteFirstInterlockSetting(true, 0);
        }

        public static void WriteWarningReset()
        {
            WriteFirstInterlockSetting(true, 1);
        }

        public static void WriteFlowControllerTargetValue((string, int?)[] aControllerIdTargetValues, short rampTime)
        {
            foreach ((string id, int? targetValue) in aControllerIdTargetValues)
            {
                if (targetValue != null)
                {
                    WriteFlowControllerTargetValue(Util.RecipeFlowControlFieldToControllerID[id], targetValue.Value, rampTime);
                }
            }
        }

        public static void WriteFlowControllerTargetValue(string controllerID, int targetValue, short rampTime)
        {
            int controllerIDIndex = dIndexController[controllerID];
            int? maxValue = SettingViewModel.ReadMaxValue(controllerID);
            if(maxValue == null)
            {
                throw new ArgumentException(controllerID + " is invalid");
            }
            
            switch (controllerIDIndex)
            {
                case 26:
                    Ads.WriteAny(hAControllerInput[controllerIDIndex], new RampGeneratorInput { restart = true, rampTime = (ushort)rampTime, targetValue = (float)targetValue  });
                    temperatureTVPublisher?.Publish((short)targetValue);
                    break;

                case 27:
                    Ads.WriteAny(hAControllerInput[controllerIDIndex], new RampGeneratorInput { restart = true, rampTime = (ushort)rampTime, targetValue = (float)targetValue });
                    pressureTVPublisher?.Publish((short)targetValue);
                    break;

                case 28:
                    Ads.WriteAny(hAControllerInput[controllerIDIndex], new RampGeneratorInput { restart = true, rampTime = (ushort)rampTime, targetValue = (float)targetValue });
                    rotationTVPublisher?.Publish((short)targetValue);
                    break;

                default:
                    Ads.WriteAny(hAControllerInput[controllerIDIndex], new RampGeneratorInput { restart = true, rampTime = (ushort)rampTime, targetValue = ((float)targetValue / (float)maxValue * AnalogControllerOutputVoltage) });
                    break;
            }
        }
    }
}
