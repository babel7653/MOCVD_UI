using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using System.Collections.ObjectModel;

namespace SapphireXR_App.ViewModels
{
    internal partial class EventLogViewModel: ObservableObject
    {
        public EventLogViewModel()
        {
            var filterEventLogs = () => {
                EventLogs = new (Models.EventLogs.Instance.EventLogList.Where((log) =>
                {
                    switch (log.Type)
                    {
                        case EventLog.LogType.Alarm:
                            return ShowAlarmMessage == true;

                        case EventLog.LogType.Warning:
                            return ShowWarningMessage == true;

                        case EventLog.LogType.Information:
                            return ShowInformationMessage == true;

                        case EventLog.LogType.None:
                            return false;

                        default:
                            return true;
                    }
                }));
            };
            Models.EventLogs.Instance.EventLogList.CollectionChanged += (sender, args) =>
            {
                ClearCommand.NotifyCanExecuteChanged();
                filterEventLogs();
            };
           
            PropertyChanged += (sender, args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(ShowAlarmMessage):
                    case nameof(ShowWarningMessage):
                    case nameof(ShowInformationMessage):
                        filterEventLogs();
                        break;
                }
            };
            EventLogs = Models.EventLogs.Instance.EventLogList;
        }

        [RelayCommand]
        private void ToggleShowAlarmMessage()
        {
            ShowAlarmMessage = !ShowAlarmMessage;
        }

        [RelayCommand]
        private void ToggleShowWarningMessage()
        {
            ShowWarningMessage = !ShowWarningMessage;
        }

        [RelayCommand]
        private void ToggleShowInformationMessage()
        {
            ShowInformationMessage = !ShowInformationMessage;
        }

        [RelayCommand]
        private void Clear()
        {
            Models.EventLogs.Instance.EventLogList.Clear();
        }

        [ObservableProperty]
        private bool _showAlarmMessage = true;
        [ObservableProperty]
        private bool _showWarningMessage = true;
        [ObservableProperty]
        private bool _showInformationMessage = true;

        [ObservableProperty]
        private ObservableCollection<EventLog> _eventLogs = new ObservableCollection<EventLog>();
    }
}
