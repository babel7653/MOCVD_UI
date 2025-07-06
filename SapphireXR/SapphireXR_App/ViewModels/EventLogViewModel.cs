using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Models;
using System.Collections.ObjectModel;


namespace SapphireXR_App.ViewModels
{
    internal partial class EventLogViewModel: ObservableObject
    {
        internal EventLogViewModel()
        {
           
        }

        [ObservableProperty]
        private ObservableCollection<EventLog> _eventLogs = new ObservableCollection<EventLog>();

    }
}
