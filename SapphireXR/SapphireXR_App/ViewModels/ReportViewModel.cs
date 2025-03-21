using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SapphireXR_App.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        public ReportViewModel() 
        {
        }

        [ObservableProperty]
        private string _logFilePath1 = "";

        [ObservableProperty]
        private string _logFilePath2 = "";

        [RelayCommand]
        public void OpenLogFile1()
        {

        }

        [RelayCommand]
        public void OpenLogFile2()
        {

        }
    }
}
