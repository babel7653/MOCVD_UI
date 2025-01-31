using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class PresentValueMonitorViewModel: ObservableObject, IObserver<float>
    {
        [ObservableProperty]
        private string _presentValue = "";

        public ICommand OnLoadedCommand => new RelayCommand<object?>((object? args) =>
        {
            if (args != null)
            {
                string? id = args as string;
                if (id != null && PLCService.dMonitoringMeterIndex.ContainsKey(id) == true)
                {
                    ObservableManager<float>.Subscribe("MonitoringPresentValue." + id + ".CurrentValue", this);
                }
            }
        });

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
            PresentValue = ((int)value).ToString();
        }
    }
}
