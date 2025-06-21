using System.Collections;
using System.Numerics;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class LeftViewModel : ObservableObject
    {
        internal class CoolingWaterValueSubscriber : IObserver<float>
        {
            internal CoolingWaterValueSubscriber(string coolingWaterIDStr, LeftViewModel vm)
            {
                coolingWaterID = coolingWaterIDStr;
                leftViewModel = vm;
            }

            void IObserver<float>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnNext(float value)
            {
                switch (coolingWaterID)
                {
                    case "ShowerHeadTemp":
                        leftViewModel.ShowerHeadTemp = ((int)value).ToString();
                        break;

                    case "InductionCoilTemp":
                        leftViewModel.InductionCoilTemp = ((int)value).ToString();
                        break;
                }
            }

            private string coolingWaterID;
            private LeftViewModel leftViewModel;
        }

        internal class HardWiringInterlockStateSubscriber : IObserver<BitArray>
        {
            public HardWiringInterlockStateSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<BitArray>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnNext(BitArray value)
            {
                var convertOnOffStateColor = (bool bit) => bit == true ? OnLampColor : OffLampColor;
                var convertThreeStateColor = (BitArray value, int startIndex) =>
                {
                    if (value[startIndex] == true)
                    {
                        return ReadyLampColor;
                    }

                    if (value[startIndex + 1] == true)
                    {
                        return RunLampColor;
                    }

                    if (value[startIndex + 2] == true)
                    {
                        return FaultLampColor;
                    }

                    return null;
                };

                leftViewModel.MaintenanceKeyLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.MaintenanceKey]);
                leftViewModel.DoorReactorCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorReactorCabinet]);
                leftViewModel.DoorGasDeliveryCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorGasDeliveryCabinet]);
                leftViewModel.DoorPowerDistributeCabinetLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.DoorPowerDistributeCabinet]);
                leftViewModel.CleanDryAirLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.CleanDryAir]);
                leftViewModel.CoolingWaterLampColor = convertOnOffStateColor(value[(int)PLCService.HardWiringInterlockStateIndex.CoolingWater]);

                leftViewModel.InductionHeaterLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.InductionHeaterReady) ?? leftViewModel.InductionHeaterLampColor;
                leftViewModel.SusceptorMotorLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.SusceptorMotorStop) ?? leftViewModel.SusceptorMotorLampColor;
                leftViewModel.VacuumPumpLampColor = convertThreeStateColor(value, (int)PLCService.HardWiringInterlockStateIndex.VacuumPumpWarning) ?? leftViewModel.VacuumPumpLampColor;
            }

            LeftViewModel leftViewModel;
        }

        internal class MainViewTabIndexChagedSubscriber : IObserver<int>
        {
            internal MainViewTabIndexChagedSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }
            void IObserver<int>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnNext(int tabIndex)
            {
                switch (tabIndex)
                {
                    case 0:
                    case 1:
                        leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentPLCStateViewModel(leftViewModel);
                        break;

                    case 2:
                        leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentRecipeStepViewModel(leftViewModel);
                        break;
                }
            }

            private LeftViewModel leftViewModel;
        }

        private class SignalTowerStateSubscriber : IObserver<BitArray>
        {
            internal SignalTowerStateSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<BitArray>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnNext(BitArray ioList)
            {
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.SingalTower_RED], ref signalTowerRed, (bool state) => { if (state == true) { leftViewModel.SignalTowerRed = ActiveSignalTowerRed; } else { leftViewModel.SignalTowerRed = InActiveSignalTowerRed; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.SingalTower_YELLOW], ref signalTowerYellow, (bool state) => { if (state == true) { leftViewModel.SignalTowerYellow = ActiveSignalTowerYellow; } else { leftViewModel.SignalTowerYellow = InActiveSignalTowerYellow; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.SingalTower_GREEN], ref signalTowerGreen, (bool state) => { if (state == true) { leftViewModel.SignalTowerGreen = ActiveSignalTowerGreen; } else { leftViewModel.SignalTowerGreen = InActiveSignalTowerGreen; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.SingalTower_BLUE], ref signalTowerBlue, (bool state) => { if (state == true) { leftViewModel.SignalTowerBlue = ActiveSignalTowerBlue; } else { leftViewModel.SignalTowerBlue = InActiveSignalTowerBlue; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.SingalTower_WHITE], ref signalTowerWhite, (bool state) => { if (state == true) { leftViewModel.SignalTowerWhite = ActiveSignalTowerWhite; } else { leftViewModel.SignalTowerWhite = InActiveSignalTowerWhite; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.DOR_Vaccum_State], ref dorVaccumState, (bool state) => { if (state == true) { leftViewModel.DorVacuumStateLampColor = OnLampColor; } else { leftViewModel.DorVacuumStateLampColor = ReadyLampColor; } });
                Util.SetIfChanged(ioList[(int)PLCService.IOListIndex.Temp_Controller_Alarm], ref tempControllerAlarm, (bool state) => { if (state == true) { leftViewModel.TempControllerAlarmLampColor = FaultLampColor; } else { leftViewModel.TempControllerAlarmLampColor = OffLampColor; } });
            }

            private LeftViewModel leftViewModel;
            private bool? signalTowerRed = null;
            private bool? signalTowerYellow = null;
            private bool? signalTowerGreen = null;
            private bool? signalTowerBlue = null;
            private bool? signalTowerWhite = null;
            private bool? signalTowerBuzzwer = null;
            private bool? dorVaccumState = null;
            private bool? tempControllerAlarm = null;
        }

        private class LineHeaterTemperatureSubscriber : IObserver<float[]>
        {
            internal LineHeaterTemperatureSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<float[]>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float[]>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void setIfChanged(float newValue, Action<float> onChanged, ref float? lineHeaterTemperature)
            {
                if (lineHeaterTemperature != newValue)
                {
                    onChanged(newValue);
                    lineHeaterTemperature = newValue;
                }
            }

            void IObserver<float[]>.OnNext(float[] currentLineHeaterTemperatures)
            {
                setIfChanged(currentLineHeaterTemperatures[0], (float newTemperature) => { leftViewModel.LineHeater1 = (int)newTemperature; }, ref lineHeater1Temperatures);
                setIfChanged(currentLineHeaterTemperatures[1], (float newTemperature) => { leftViewModel.LineHeater2 = (int)newTemperature; }, ref lineHeater2Temperatures);
                setIfChanged(currentLineHeaterTemperatures[2], (float newTemperature) => { leftViewModel.LineHeater3 = (int)newTemperature; }, ref lineHeater3Temperatures);
                setIfChanged(currentLineHeaterTemperatures[3], (float newTemperature) => { leftViewModel.LineHeater4 = (int)newTemperature; }, ref lineHeater4Temperatures);
                setIfChanged(currentLineHeaterTemperatures[4], (float newTemperature) => { leftViewModel.LineHeater5 = (int)newTemperature; }, ref lineHeater5Temperatures);
                setIfChanged(currentLineHeaterTemperatures[5], (float newTemperature) => { leftViewModel.LineHeater6 = (int)newTemperature; }, ref lineHeater6Temperatures);
                setIfChanged(currentLineHeaterTemperatures[6], (float newTemperature) => { leftViewModel.LineHeater7 = (int)newTemperature; }, ref lineHeater7Temperatures);
                setIfChanged(currentLineHeaterTemperatures[7], (float newTemperature) => { leftViewModel.LineHeater8 = (int)newTemperature; }, ref lineHeater8Temperatures);
            }

            LeftViewModel leftViewModel;
            float? lineHeater1Temperatures;
            float? lineHeater2Temperatures;
            float? lineHeater3Temperatures;
            float? lineHeater4Temperatures;
            float? lineHeater5Temperatures;
            float? lineHeater6Temperatures;
            float? lineHeater7Temperatures;
            float? lineHeater8Temperatures;
        }

        private class ResetCurrentRecipeSubscriber : IObserver<bool>
        {
            internal ResetCurrentRecipeSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                if (leftViewModel.CurrentSourceStatusViewModel is SourceStatusFromCurrentRecipeStepViewModel)
                {
                    leftViewModel.CurrentSourceStatusViewModel = new SourceStatusFromCurrentRecipeStepViewModel(leftViewModel);
                }
            }

            private LeftViewModel leftViewModel;
        }

        private class LogicalInterlockSubscriber : IObserver<BitArray>
        {
            public LogicalInterlockSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<BitArray>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            private static void SetIfChanged<T>(T newValue, ref T? prevValue, Action<T> onChanged) where T : struct, IComparisonOperators<T, T, bool>
            {
                if (prevValue == null || prevValue != newValue)
                {
                    onChanged(newValue);
                    prevValue = newValue;
                }
            }

           
            void IObserver<BitArray>.OnNext(BitArray value)
            {
                var getGasState = (BitArray bitArray, int upperBitIndex, int lowerBitIndex) => (byte)(Convert.ToUInt32(bitArray[upperBitIndex]) << 1 | Convert.ToUInt32(bitArray[lowerBitIndex]));
                var getGasStateColor = (byte state) =>
                {
                    switch (state)
                    {
                        case 0:
                            return RunLampColor;

                        case 1:
                            return ReadyLampColor;

                        case 2:
                            return FaultLampColor;

                        default:
                            return Brushes.Transparent;
                    }
                };
                var getDeviceColor = (bool state) => state == true ? OnLampColor : OffLampColor;

                SetIfChanged(getGasState(value, 3, 2), ref prevGasPressureN2State, (byte state) => leftViewModel.GasPressureGas2StateColor = getGasStateColor(state));
                SetIfChanged(getGasState(value, 5, 4), ref prevGasPressureH2State, (byte state) => leftViewModel.GasPressureGas1StateColor = getGasStateColor(state));
                SetIfChanged(getGasState(value, 7, 6), ref prevGasPressureNH3State, (byte state) => leftViewModel.GasPressureGas3StateColor = getGasStateColor(state));
                SetIfChanged(getGasState(value, 9, 8), ref prevGasPressureSiH4State, (byte state) => leftViewModel.GasPressureGas4StateColor = getGasStateColor(state));
                Util.SetIfChanged(value[10], ref prevRecipeStartState, (bool state) => leftViewModel.RecipeStartStateColor = getDeviceColor(state));
                Util.SetIfChanged(value[11], ref prevReactorOpenState, (bool state) => leftViewModel.ReactorOpenStateColor = getDeviceColor(state));
                Util.SetIfChanged(value[12], ref prevHeaterTurnOnState, (bool state) => leftViewModel.HeaterTurnOnStateColor = getDeviceColor(state));
                Util.SetIfChanged(value[13], ref prevPumpTurnOnState, (bool state) => leftViewModel.PumpTurnOnStateColor = getDeviceColor(state));
            }

            byte? prevGasPressureH2State = null;
            byte? prevGasPressureN2State = null;
            byte? prevGasPressureSiH4State = null;
            byte? prevGasPressureNH3State = null;
            bool? prevRecipeStartState = null;
            bool? prevReactorOpenState = null;
            bool? prevHeaterTurnOnState = null;
            bool? prevPumpTurnOnState = null;

            LeftViewModel leftViewModel;
        }

        private class GasIOLabelSubscriber : IObserver<(string, string)>
        {
            public GasIOLabelSubscriber(LeftViewModel vm)
            {
                leftViewModel = vm;
            }

            void IObserver<(string, string)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnNext((string, string) value)
            {
                var updateCarrierStatus = (string prevGasName, string gasName) =>
                {
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.NH3_1Carrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.NH3_1Carrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.NH3_2Carrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.NH3_2Carrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.SiH4Carrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.SiH4Carrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.TEBCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.TEBCarrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.TMAlCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.TMAlCarrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.TMGaCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.TMGaCarrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.DTMGaCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.DTMGaCarrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.Cp2MgCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.Cp2MgCarrier = gasName;
                    }
                    if (prevGasName == leftViewModel.CurrentSourceStatusViewModel.TMInCarrier)
                    {
                        leftViewModel.CurrentSourceStatusViewModel.TMInCarrier = gasName;
                    }
                };
                switch (value.Item1)
                {
                    case "Gas1":
                        updateCarrierStatus(leftViewModel.Gas1, value.Item2);
                        leftViewModel.Gas1 = value.Item2;
                        leftViewModel.LogicalInterlockGas1 = LeftViewModel.GetIogicalInterlockLabel(value.Item2);
                        break;

                    case "Gas2":
                        updateCarrierStatus(leftViewModel.Gas2, value.Item2);
                        leftViewModel.Gas2 = value.Item2;
                        leftViewModel.LogicalInterlockGas2 = LeftViewModel.GetIogicalInterlockLabel(value.Item2);
                        break;

                    case "Gas3":
                        leftViewModel.Gas3_1 = LeftViewModel.GetGas3Label(value.Item2, 1);
                        leftViewModel.Gas3_2 = LeftViewModel.GetGas3Label(value.Item2, 2);
                        leftViewModel.LogicalInterlockGas3 = LeftViewModel.GetIogicalInterlockLabel(value.Item2);
                        break;

                    case "Gas4":
                        leftViewModel.Gas4 = value.Item2;
                        leftViewModel.LogicalInterlockGas4 = LeftViewModel.GetIogicalInterlockLabel(value.Item2);
                        break;

                    case "Source1":
                        leftViewModel.Source1 = value.Item2;
                        break;

                    case "Source2":
                        leftViewModel.Source2 = value.Item2;
                        break;

                    case "Source3":
                        leftViewModel.Source3 = value.Item2;
                        break;

                    case "Source4":
                        leftViewModel.Source4 = value.Item2;
                        break;

                    case "Source5":
                        leftViewModel.Source5 = value.Item2;
                        break;

                    case "Source6":
                        leftViewModel.Source6 = value.Item2;
                        break;
                }
            }

            private LeftViewModel leftViewModel;
        }
    }
}
