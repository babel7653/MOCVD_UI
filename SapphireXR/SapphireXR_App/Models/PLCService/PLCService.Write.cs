using System.Collections;
using System.Windows;

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

        public static void WriteDeviceMaxValue(List<AnalogDeviceIO>? analogDeviceIOs)
        {
            // Device Max. Value Write
            try
            {
                if (analogDeviceIOs == null)
                {
                    throw new Exception("AnalogDeviceIO is null in WriteDeviceMaxValue");
                }

                float[] maxValue = new float[29];
                int index = 0;
                foreach (AnalogDeviceIO entry in analogDeviceIOs)
                {
                    if (entry.ID == null)
                    {
                        throw new Exception("entry ID is null for AnalogDeviceIO");
                    }
                    if (index < 3)
                    {
                        maxValue[index + 26] = entry.MaxValue;
                    }
                    else
                    {
                        maxValue[index - 3] = entry.MaxValue;
                    }
                    index++;
                }
                Ads.WriteAny(hDeviceMaxValuePLC, maxValue, [dIndexController.Count]);
                
                // List Analog Device Input / Output
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void WriteTargetValue(string flowControllerID, int targetValue)
        {
            aDeviceTargetValues![dIndexController[flowControllerID]] = (float)targetValue;
            Ads.WriteAny(hWriteDeviceTargetValuePLC, aDeviceTargetValues!, [aDeviceTargetValues!.Length]);
        }

        public static void WriteTargetValue(float[] targetValues)
        {
            if(targetValues.Length == NumControllers)
            {
                Ads.WriteAny(hWriteDeviceTargetValuePLC, targetValues, [targetValues.Length]);
            }
            
        }

        public static void WriteRampTime(string flowControllerID, short currentValue)
        {
            aDeviceRampTimes![dIndexController[flowControllerID]] = currentValue;
            Ads.WriteAny(hWriteDeviceRampTimePLC, aDeviceRampTimes!, [aDeviceRampTimes!.Length]);
        }

        public static void WriteRampTime(short[] rampTimes)
        {
            if (rampTimes.Length == NumControllers)
            {
                Ads.WriteAny(hWriteDeviceRampTimePLC, rampTimes, [rampTimes.Length]);
            }
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

        public static void WriteOperationMode(bool operatonMode)
        {
            Ads.WriteAny(hOperationMode, operatonMode);
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
    }
}
