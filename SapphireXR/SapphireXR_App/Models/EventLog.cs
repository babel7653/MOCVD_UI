using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SapphireXR_App.Models
{
    public partial class EventLog : ObservableObject
    {
        public enum LogType { Alarm = 0, Warning, Information, None };

        [ObservableProperty]
        private LogType _type = LogType.None;
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private string _message = "";
        [ObservableProperty]
        private DateTime _date = DateTime.Now;
    }

    public partial class EventLogs: ObservableObject
    {
        public static EventLogs Instance { get; } = new EventLogs();

        private EventLogs() { }

        [ObservableProperty]
        private ObservableCollection<EventLog> _eventLogList = new ObservableCollection<EventLog>();
    }
}
