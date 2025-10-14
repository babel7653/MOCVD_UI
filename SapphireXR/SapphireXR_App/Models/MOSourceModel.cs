using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using System.Text.Json.Serialization;

namespace SapphireXR_App.Models
{
    internal partial class MOSourceModel: ObservableObject, IDisposable
    {
        private class ConnectedMFCPVSubscriber: IObserver<float>
        {
            internal ConnectedMFCPVSubscriber(MOSourceModel model)
            {
                moSourceModel = model;
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
                moSourceModel.QMFC = value;
            }

            private MOSourceModel moSourceModel;
        }

        private class ConnectedEPCPVSubscriber : IObserver<float>
        {
            internal ConnectedEPCPVSubscriber(MOSourceModel model)
            {
                moSourceModel = model;
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
                moSourceModel.BubblePressure = value;
            }

            private MOSourceModel moSourceModel;
        }

        private class ConnectedValveStateSubscriber: IObserver<bool>
        {
            internal ConnectedValveStateSubscriber(MOSourceModel model)
            {
                moSourceModel = model;
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
                moSourceModel.update = value;
            }

            private MOSourceModel moSourceModel;
        }
      
        internal enum MOMaterial { Liquid = 0, Solid }

        [Newtonsoft.Json.JsonConstructor]
        internal MOSourceModel(string mFC, string ePC, string valve, MOMaterial material)
        {
            MFC = mFC;
            EPC = ePC;
            Valve = valve;
            Material = material;

            connectedMFCPVSubscriberDisposer = ObservableManager<float>.Subscribe("FlowControl." + MFC + ".CurrentValue", connectedMFCPVSubscriber = new ConnectedMFCPVSubscriber(this));
            connectedEPCPVSubscriberDisposer = ObservableManager<float>.Subscribe("FlowControl." + EPC + ".CurrentValue", connectedEPCPVSubscriber = new ConnectedEPCPVSubscriber(this));
            connectedValveStateSubscriberDisposer = ObservableManager<bool>.Subscribe("Valve.OnOff." + Valve + ".CurrentPLCState", connectedValveStateSubscriber = new ConnectedValveStateSubscriber(this));
            update = PLCService.ReadValveState(Valve);

            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(BubblerTemp):
                        if (BubblerTemp != null)
                        {
                            T = BubblerTemp.Value + MOSourceSetting.AbsoluteTemp;
                        }
                        else
                        {
                            T = MOSourceSetting.AbsoluteTemp;
                        }
                        break;

                    case nameof(AValue):
                    case nameof(BValue):
                    case nameof(T):
                        if (AValue != null && BValue != null)
                        {
                            try
                            {
                                switch (Material)
                                {
                                    case MOMaterial.Liquid:
                                        PartialPressure = (float)Math.Pow(10.0, AValue.Value - (BValue.Value / T));
                                        break;
                                    case MOMaterial.Solid:
                                        PartialPressure = (float)Math.Pow(10.0, AValue.Value - (2.18 * Math.Log(Math.E, T)) - (BValue.Value / T));
                                        break;
                                }
                            }
                            catch
                            {
                                PartialPressure = null;
                            }
                        }
                        else
                        {
                            PartialPressure = null;
                        }
                        break;

                    case nameof(PartialPressure):
                    case nameof(BubblePressure):
                    case nameof(BubblerConst):
                    case nameof(MolarWeight):
                    case nameof(QMFC):
                        if (PartialPressure != null && BubblePressure != null && BubblerConst != null && MolarWeight != null && QMFC != null)
                        {
                            try
                            {
                                weightDelta = QMFC * PartialPressure.Value / (BubblePressure - PartialPressure) / (22400 * 60) * MolarWeight.Value * BubblerConst.Value;
                            }
                            catch
                            {
                                weightDelta = null;
                            }
                        }
                        else
                        {
                            weightDelta = null;
                        }
                        break;

                    case nameof(InitialWeight):
                        if (InitialWeight != null)
                        {
                            UsedWeight = 0.0f;
                        }
                        else
                        {
                            UsedWeight = null;
                        }
                        break;

                    case nameof(UsedWeight):
                        if (UsedWeight != null && InitialWeight != null)
                        {
                            float remainWeightUpdate = InitialWeight.Value - UsedWeight.Value;
                            RemainWeight = Math.Max(0.0f, remainWeightUpdate);
                        }
                        else
                        {
                            RemainWeight = null;
                        }
                        break;

                    case nameof(RemainWeight):
                        if (0.0f < RemainWeight)
                        {
                            Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ =>
                            {
                                if (UsedWeight == null || weightDelta == null || update == false)
                                {
                                    return;
                                }

                                UsedWeight += weightDelta;

                            }, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                        break;

                    case nameof(TemperatureConstant):
                        if (TemperatureConstant == true)
                        {
                            BubblerTemp = null;
                        }
                        break;
                }
            };
        }

        private void cleanUp()
        {
            connectedMFCPVSubscriberDisposer?.Dispose();
            connectedEPCPVSubscriberDisposer?.Dispose();
            connectedValveStateSubscriberDisposer?.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cleanUp();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~MOSourceModel()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [ObservableProperty]
        private float? aValue= null;

        [ObservableProperty]
        private float? bValue= null;

        [ObservableProperty]
        private float? bubblerTemp= null;

        [ObservableProperty]
        private float? bubblerConst= null;

        [ObservableProperty]
        private float? molarWeight= null;

        [ObservableProperty]
        private float? initialWeight= null;

        [ObservableProperty]
        private float? usedWeight= null;

        [ObservableProperty]
        private bool temperatureConstant = false;

        [ObservableProperty]
        private float t = MOSourceSetting.AbsoluteTemp;

        [ObservableProperty]
        private float? qMFC = null;

        [ObservableProperty]
        private float? partialPressure = null;

        [ObservableProperty]
        private float? bubblePressure = null;

        [ObservableProperty]
        private float? remainWeight = null;

        
        public string MFC { get; private set; }
        public string EPC { get; private set; }
        public string Valve { get; private set; }
        public MOMaterial Material { get; private set; }

        private float? weightDelta = null;
        private bool update = false;

        private ConnectedMFCPVSubscriber? connectedMFCPVSubscriber = null;
        private ConnectedEPCPVSubscriber? connectedEPCPVSubscriber = null;
        private ConnectedValveStateSubscriber? connectedValveStateSubscriber = null;

        private IDisposable? connectedMFCPVSubscriberDisposer = null;
        private IDisposable? connectedEPCPVSubscriberDisposer = null;
        private IDisposable? connectedValveStateSubscriberDisposer = null;

        private bool disposedValue = false;
       
    }
}
