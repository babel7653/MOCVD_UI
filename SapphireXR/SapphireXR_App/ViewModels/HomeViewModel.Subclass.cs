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

        private class RecipeEndedSubscriber : IObserver<bool>
        {
            internal RecipeEndedSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
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
                homeViewModel.loadBatchOnRecipeEnd();
            }
            private HomeViewModel homeViewModel;
        }

        private class ReactorTargetValueSubscriber : IObserver<short>
        {
            internal ReactorTargetValueSubscriber(Action<string> onNewValueAct)
            {
                onNewValue = onNewValueAct;
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
                if (prevTemperatureTV != value)
                {
                    onNewValue(0 <= value ? value.ToString() : "");
                    prevTemperatureTV = value;
                }
            }
            private Action<string> onNewValue;
            private float? prevTemperatureTV = null;
        }
    }
}
