using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SapphireXR_App.ViewModels
{
    internal class ManualBatchViewModel: ObservableObject
    {
        public ICommand BatchCommand => new RelayCommand(() =>
        {

        });
        public ICommand SaveCommand => new RelayCommand(() =>
        {

        });
    }
}
