using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.Models
{
    public partial class EventLog : ObservableObject
    {
        [ObservableProperty]
        private string _type = "";
        [ObservableProperty]
        private string _message = "";
        [ObservableProperty]
        private string _date = "";
    }
}
