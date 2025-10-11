using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class PresentValueMonitorViewModel: ObservableObject, IObserver<float>
    {
        public ICommand OnLoadedCommand => new RelayCommand<object?>((object? args) =>
        {
            if (args != null)
            {
                string? id = args as string;
                if (id != null && PLCService.dMonitoringMeterIndex.ContainsKey(id) == true)
                {
                    onLoaded(id);
                }
            }
        });

        protected virtual void onLoaded(string id)
        {
            ObservableManager<float>.Subscribe("MonitoringPresentValue." + id + ".CurrentValue", this);
        }

        protected virtual void updatePresentValue(float value)
        {
            PresentValue = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit);
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
            if (prevPresentValue == null || prevPresentValue != value)
            {
                updatePresentValue(value);
                prevPresentValue = value;
            }
        }

        [ObservableProperty]
        private string _presentValue = "";
        private float? prevPresentValue = null;
    }
}
