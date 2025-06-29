using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SapphireXR_App.Models;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Collections;
using SapphireXR_App.Common;

namespace SapphireXR_App.ViewModels
{
    internal partial class ReportSeriesSelectionViewModel: ObservableObject
    {
        public enum SelectionToShowChanged { Added, Removed };

        public ReportSeriesSelectionViewModel()
        {
            List<string> names = new List<string>();
            foreach(string name in LogReportSeries.LogSeriesColor.Keys)
            {
                names.Add(name);
            }
            names.Sort();
            Names = new ObservableCollection<string>(names);
        }

        private bool canAddToSelectedExecuted()
        {
            return selectedFromLeftList != null && 0 < selectedFromLeftList.Count;
        }
        [RelayCommand(CanExecute = "canAddToSelectedExecuted")]
        private void AddToSelected()
        {
            if (selectedFromLeftList != null)
            {
                IList<string> copy = new List<string>();
                foreach (object selected in selectedFromLeftList)
                {
                    string? name = selected as string;
                    if (name != null && name != string.Empty)
                    {
                        copy.Add(name);
                    }
                }
                foreach (string name in copy)
                {
                    if (SelectedNames.Contains(name) == false)
                    {
                        SelectedNames.Add(name);
                    }
                    Names.Remove(name);
                }
                selectionChangedPublisher.Issue((SelectionToShowChanged.Added, copy));
            }
        }

        private bool canRemoveFromSelectedExecuted()
        {
            return selectedFromRightList != null && 0 < selectedFromRightList.Count;
        }
        [RelayCommand(CanExecute = "canRemoveFromSelectedExecuted")]
        private void RemoveFromSelected()
        {
            if(selectedFromRightList != null)
            {
                IList<string> copy = new List<string>();
                foreach(object selected in selectedFromRightList)
                {
                    string? name = selected as string;
                    if (name != null && name != string.Empty)
                    {
                        copy.Add(name);
                    }
                }
                foreach (string selected in copy)
                {
                    SelectedNames.Remove(selected);
                    if (Names.Contains(selected) == false)
                    {
                        Names.Add(selected);
                    }
                }
                List<string> names = Names.ToList();
                names.Sort();
                Names = new ObservableCollection<string>(names);
                selectionChangedPublisher.Issue((SelectionToShowChanged.Removed, copy));
            }
        }

        private static IList? GetSelectedFromListBox(object? args)
        {
            SelectionChangedEventArgs? selectionChangedEventArgs = args as SelectionChangedEventArgs;
            if (selectionChangedEventArgs != null)
            {
                ListBox? source = selectionChangedEventArgs.Source as ListBox;
                if (source != null)
                {
                    return source.SelectedItems;
                }
            }

            return null;
        }

        public RelayCommand<object?> LeftSelectionChangedCommand => new RelayCommand<object?>((object? args) =>
        {
            selectedFromLeftList = GetSelectedFromListBox(args);
            AddToSelectedCommand.NotifyCanExecuteChanged();
        });
        public RelayCommand<object?> RightSelectionChangedCommand => new RelayCommand<object?>((object? args) =>
        {
            selectedFromRightList = GetSelectedFromListBox(args);
            RemoveFromSelectedCommand.NotifyCanExecuteChanged();
        });

       
        public IList? selectedFromLeftList = null;
        public IList? selectedFromRightList = null;

        [ObservableProperty]
        public ObservableCollection<string> _names;
       
        public ObservableCollection<string> SelectedNames { get; } = new ObservableCollection<string>();
        private ObservableManager<(SelectionToShowChanged, IList<string>)>.Publisher selectionChangedPublisher =  ObservableManager<(SelectionToShowChanged, IList<string>)>.Get("ReportSeriesSelection.ToShowChanged");
    }
}
