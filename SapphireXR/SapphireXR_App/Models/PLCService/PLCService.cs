﻿using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using System.Collections;
using System.Windows;
using TwinCAT.Ads;
using TwinCAT.PlcOpen;

namespace SapphireXR_App.Models
{
    public static partial class PLCService
    {
        static PLCService()
        {
            Ads = new AdsClient();
        }

        public static void Connect()
        {
            try
            {
                if (AppSetting.PLCAddress != "Local")
                {
                    Ads.Connect(new AmsAddress(AppSetting.PLCAddress + ":" + AppSetting.PLCPort));
                }
                else
                {
                    Ads.Connect(AmsNetId.Local, AppSetting.PLCPort);
                }

                if (Ads.IsConnected == true)
                {
                    //Read Set Value from PLC 
                    hDeviceControlValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_CV");
                    //Read Present Value from Device of PLC
                    hDeviceCurrentValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_PV");
                    //Read and Write Max Value of PLC 
                    hDeviceMaxValuePLC = Ads.CreateVariableHandle("GVL_IO.aMaxValueController");

                    hReadValveStatePLC1 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[1]");
                    hReadValveStatePLC2 = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve[2]");
                    hWriteDeviceTargetValuePLC = Ads.CreateVariableHandle("GVL_IO.aController_TV");
                    hWriteDeviceRampTimePLC = Ads.CreateVariableHandle("GVL_IO.aController_RampTime");

                    hMonitoring_PV = Ads.CreateVariableHandle("GVL_IO.aMonitoring_PV");
                    hInputState = Ads.CreateVariableHandle("GVL_IO.aInputState");
                    hInputState4 = Ads.CreateVariableHandle("GVL_IO.aInputState[4]");
                    hDigitalOutput = Ads.CreateVariableHandle("GVL_IO.aDigitalOutputIO");
                    hDigitalOutput2 = Ads.CreateVariableHandle("GVL_IO.aDigitalOutputIO[2]");
                    hOutputCmd = Ads.CreateVariableHandle("GVL_IO.aOutputCmd");
                    hOutputCmd1 = Ads.CreateVariableHandle("GVL_IO.aOutputCmd[1]");
                    hOutputCmd2 = Ads.CreateVariableHandle("GVL_IO.aOutputCmd[2]");
                    hInterlock1 = Ads.CreateVariableHandle("GVL_IO.aInterlock[1]");

                    hRcp = Ads.CreateVariableHandle("RCP.aRecipe");
                    hRcpTotalStep = Ads.CreateVariableHandle("RCP.iRcpTotalStep");
                    hCmd_RcpOperation = Ads.CreateVariableHandle("RCP.cmd_RcpOperation");
                    hState_RcpOperation = Ads.CreateVariableHandle("RCP.state_RcpOperation");
                    hRcpStepN = Ads.CreateVariableHandle("P50_RecipeControl.nRcpIndex");
                    hTemperaturePV = Ads.CreateVariableHandle("P13_LineHeater.rTemperaturePV");
                    hOperationMode = Ads.CreateVariableHandle("MAIN.bOperationMode");
                    hUserState = Ads.CreateVariableHandle("RCP.userState");
                    hRecipeControlHoldTime = Ads.CreateVariableHandle("P50_RecipeControl.Hold_ET");
                    hRecipeControlRampTime = Ads.CreateVariableHandle("P50_RecipeControl.Ramp_ET");
                    hRecipeControlPauseTime = Ads.CreateVariableHandle("P50_RecipeControl.Pause_ET");
                    hE3508InputManAuto = Ads.CreateVariableHandle("P11_E3508.nInputManAutoBytes");
                    hOutputSetType = Ads.CreateVariableHandle("P12_IQ_PLUS.nOutputSetType");
                    hOutputMode = Ads.CreateVariableHandle("P12_IQ_PLUS.nOutputMode");

                    aDeviceRampTimes = new short[dIndexController.Count];
                    aDeviceTargetValues = new float[dIndexController.Count];

                    ReadInitialStateValueFromPLC();

                    ObservableManager<PLCConnection>.Get("PLCService.Connected").Issue(PLCConnection.Connecrted);
                }
                else
                {
                    throw new Exception(string.Empty);
                }
            }
            catch (Exception e)
            {
                throw new Exception("PLC로의 연결이 실패했습니다. 물리적 연결이나 서비스가 실행 중인지 확인해 보십시요." + (e.Message != string.Empty ? "문제의 원인은 다음과 같습니다: " + e.Message : e.Message));
            }
        }

        private static void OnTick(object? sender, EventArgs e)
        {
            try
            {
                ReadCurrentValueFromPLC();
                if (aDeviceControlValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlValueIssuers?[kv.Key].Issue(aDeviceControlValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dCurrentValueIssuers?[kv.Key].Issue(aDeviceCurrentValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceTargetValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dTargetValueIssuers?[kv.Key].Issue(aDeviceTargetValues[dIndexController[kv.Key]]);
                    }
                }
                if (aDeviceControlValues != null && aDeviceCurrentValues != null)
                {
                    foreach (KeyValuePair<string, int> kv in dIndexController)
                    {
                        dControlCurrentValueIssuers?[kv.Key].Issue((aDeviceCurrentValues[dIndexController[kv.Key]], aDeviceControlValues[dIndexController[kv.Key]]));
                    }
                }

                if (aMonitoring_PVs != null)
                {
                    foreach (KeyValuePair<string, int> kv in dMonitoringMeterIndex)
                    {
                        aMonitoringCurrentValueIssuers?[kv.Key].Issue(aMonitoring_PVs[kv.Value]);
                    }
                }

                if (aInputState != null)
                {
                    short value = aInputState[0];
                    baHardWiringInterlockStateIssuers?.Issue(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse().ToArray()));
                    dThrottleValveStatusIssuer?.Issue(aInputState[4]);

                    bool[] ioList = new bool[64];
                    for (int inputState = 1; inputState < aInputState.Length; ++inputState)
                    {
                        new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(aInputState[inputState]) : BitConverter.GetBytes(aInputState[inputState]).Reverse().ToArray()).CopyTo(ioList, (inputState - 1) * sizeof(short) * 8);
                    }
                    dIOStateList?.Issue(new BitArray(ioList));
                }

                if (baReadValveStatePLC1 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx1)
                    {
                        dValveStateIssuers?[valveID].Issue(baReadValveStatePLC1[index]);
                    }
                }
                if (baReadValveStatePLC2 != null)
                {
                    foreach ((string valveID, int index) in ValveIDtoOutputSolValveIdx2)
                    {
                        dValveStateIssuers?[valveID].Issue(baReadValveStatePLC2[index]);
                    }
                }
                dLineHeaterTemperatureIssuers?.Issue(Ads.ReadAny<float[]>(hTemperaturePV, [(int)LineHeaterTemperature]));

                byte[] digitalOutput = Ads.ReadAny<byte[]>(hDigitalOutput, [4]);
                dDigitalOutput2?.Issue(new BitArray(new byte[1] { digitalOutput[1] }));
                dDigitalOutput3?.Issue(new BitArray(new byte[1] { digitalOutput[2] }));
                short[] outputCmd = Ads.ReadAny<short[]>(hOutputCmd, [3]);
                dOutputCmd1?.Issue(bOutputCmd1 = new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(outputCmd[0]) : BitConverter.GetBytes(outputCmd[0]).Reverse().ToArray()));
                dThrottleValveControlMode?.Issue(outputCmd[1]);
                ushort inputManAuto = Ads.ReadAny<ushort>(hE3508InputManAuto);
                dInputManAuto?.Issue(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(inputManAuto) : BitConverter.GetBytes(inputManAuto).Reverse().ToArray()));
                dPressureControlModeIssuer?.Issue(Ads.ReadAny<ushort>(hOutputSetType));
                dRecipeControlHoldTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlHoldTime).Time.Seconds);
                dRecipeControlRampTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlRampTime).Time.Seconds);
                dRecipeControlPauseTimeIssuer?.Issue(Ads.ReadAny<TIME>(hRecipeControlPauseTime).Time.Seconds);
                short iterlock1 = Ads.ReadAny<short>(hInterlock1);
                dLogicalInterlockStateIssuer?.Issue(new BitArray(BitConverter.IsLittleEndian == true ? BitConverter.GetBytes(iterlock1) : BitConverter.GetBytes(iterlock1).Reverse().ToArray()));

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
                if (aDeviceTargetValues == null)
                {
                    if (exceptionStr != string.Empty)
                    {
                        exceptionStr += "\r\n";
                    }
                    exceptionStr += "aDeviceTargetValues is null in OnTick PLCService";
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
                    throw new Exception(exceptionStr);
                }
            }
            catch (Exception exception)
            {
                if(ShowMessageOnOnTick == true)
                {
                    ShowMessageOnOnTick = MessageBox.Show("PLC로부터 상태 (Analog Device Control/Valve 상태)를 읽어오는데 실패했습니다. 이 메시지를 다시 표시하지 않으려면 Yes를 클릭하세요. 원인은 다음과 같습니다: " + exception.Message, "",
                        MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes ? false : true;
                }
            }
        }

        private static (BitArray, int, uint) GetBuffer(string valveID)
        {
            int index = -1;
            if (ValveIDtoOutputSolValveIdx1.TryGetValue(valveID, out index) == true)
            {
                if (baReadValveStatePLC1 == null)
                {
                    throw new ReadValveStateException("PLC Service: BaReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (baReadValveStatePLC1, index, hReadValveStatePLC1);
                }
            }
            else
                if (ValveIDtoOutputSolValveIdx2.TryGetValue(valveID, out index) == true)
            {
                if (baReadValveStatePLC2 == null)
                {
                    throw new ReadValveStateException("PLC Service: baReadValveStatePLC1 accessed without initialization \r\n Call ReadValveStateFromPLC first");
                }
                else
                {
                    return (baReadValveStatePLC2, index, hReadValveStatePLC2);
                }
            }
            else
            {
                throw new ReadValveStateException("PLC Service: non-exsiting valve ID entered to GetReadValveStateBuffer()");
            }
        }
    }
}
