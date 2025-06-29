using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Collections;

namespace SapphireXR_App.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private abstract class FlowControllerValueSubscriber;
        private class FlowControllerValueSubscriber<T> : FlowControllerValueSubscriber, IObserver<T>
        {
            internal FlowControllerValueSubscriber(Action<T> onNextAct, string topicNameStr)
            {
                onNext = onNextAct;
                topicName = topicNameStr;
            }

            void IObserver<T>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<T>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<T>.OnNext(T value)
            {
                onNext(value);
            }

            public string topicName;
            private readonly Action<T> onNext;
        }

        private class DigitalOutput3Subscriber : IObserver<BitArray>
        {
            internal DigitalOutput3Subscriber(HomeViewModel vm)
            {
                homeViewMode = vm;
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
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.RotationAlaramReset], ref prevRotationAlarmReset, (bool value) => { homeViewMode.RotationReset = (value == true ? "Reset" : "No Reset"); });
            }

            private HomeViewModel homeViewMode;
            private bool? prevRotationAlarmReset;
        }

        private class ThrottleValveStatusSubscriber : IObserver<short>
        {
            internal ThrottleValveStatusSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
            }

            void IObserver<short>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<short>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<short>.OnNext(short value)
            {
                if (prevThrottleValveControlMode == null || prevThrottleValveControlMode != value)
                {
                    if (value < ThrottleValveStatusToString.Length)
                    {
                        homeViewModel.ThrottleValveStatus = ThrottleValveStatusToString[value];
                    }
                    prevThrottleValveControlMode = value;
                }
            }

            private HomeViewModel homeViewModel;
            private short? prevThrottleValveControlMode = null;
        }

        private class EventLogSubscriber : IObserver<EventLog>
        {
            internal EventLogSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
            }

            void IObserver<EventLog>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<EventLog>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<EventLog>.OnNext(EventLog value)
            {
                homeViewModel.EventLogs.Add(value);
                if(value.Type == "Recipe End")
                {
                    if (PLCService.Connected == PLCConnection.Connected)
                    {
                        homeViewModel.loadBatchOnRecipeEnd();
                    }
                }
            }

            private HomeViewModel homeViewModel;
        }

        private class PLCConnectionStateSubscriber : IObserver<Enums.PLCConnection>
        {
            public PLCConnectionStateSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
            }

            void IObserver<PLCConnection>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnNext(PLCConnection value)
            {
                if (value == PLCConnection.Connected)
                {
                    homeViewModel.initRightDashBoard();
                }
                homeViewModel.OnThrottleValveModeChangedCommand.NotifyCanExecuteChanged();
                homeViewModel.TogglePressureControlModeCommand.NotifyCanExecuteChanged();
                homeViewModel.InductionHeaterResetCommand.NotifyCanExecuteChanged();
                homeViewModel.ToggleHeaterControlModeCommand.NotifyCanExecuteChanged();
                homeViewModel.VacuumPumpResetCommand.NotifyCanExecuteChanged();
                homeViewModel.manualBatchViewModel.LoadToPLCCommand.NotifyCanExecuteChanged();
                homeViewModel.PLCConnected = PLCService.Connected == PLCConnection.Connected ? true : false;
            }

            HomeViewModel homeViewModel;
        }
    }
}
