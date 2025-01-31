using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using System.Reactive;
using System.Windows.Controls;

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
                switch(coolingWaterID)
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

        public LeftViewModel()
        {
            ObservableManager<float>.Subscribe("MonitoringPresentValue.ShowerHeadTemp.CurrentValue", showerHeaderTempSubscriber = new CoolingWaterValueSubscriber("ShowerHeadTemp", this));
            ObservableManager<float>.Subscribe("MonitoringPresentValue.InductionCoilTemp.CurrentValue", inductionCoilTempSubscriber = new CoolingWaterValueSubscriber("InductionCoilTemp", this));
        }

        [ObservableProperty]
        private string _showerHeadTemp = "";

        [ObservableProperty]
        private string _inductionCoilTemp = "";

        private CoolingWaterValueSubscriber showerHeaderTempSubscriber;
        private CoolingWaterValueSubscriber inductionCoilTempSubscriber;
    }
}
