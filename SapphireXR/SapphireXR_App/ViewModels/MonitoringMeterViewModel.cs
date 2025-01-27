using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public class MonitoringMeterViewModel : DependencyObject, INotifyPropertyChanged
    {
        static MonitoringMeterViewModel()
        {

        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
