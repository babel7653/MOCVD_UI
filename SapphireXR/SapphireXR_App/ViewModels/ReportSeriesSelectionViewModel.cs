using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SapphireXR_App.Models;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Collections;

namespace SapphireXR_App.ViewModels
{
    internal partial class ReportSeriesSelectionViewModel: ObservableObject
    {
        public ReportSeriesSelectionViewModel()
        {
            foreach((string name, var colors) in LogReportSeries.LogSeriesColor)
            {
                Names.Add(name);
            }
        }

        [RelayCommand]
        private void AddToSelected()
        {
            if (selectedFromLeftList != null)
            {
                foreach (object selected in selectedFromLeftList)
                {
                    string? name = selected as string;
                    if (name != null && Selected.Contains(name) == false)
                    {
                        Selected.Add(name);
                    }
                }
            }
        }

        [RelayCommand]
        private void RemoveFromSelected()
        {
            if(selectedFromRightList != null)
            {
                foreach(object selected in selectedFromRightList)
                {
                    string? name = selected as string;
                    if (name != null && Selected.Contains(name) == true)
                    {
                        Selected.Remove(name);
                    }
                }
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
        });
        public RelayCommand<object?> RightSelectionChangedCommand => new RelayCommand<object?>((object? args) =>
        {
            selectedFromRightList = GetSelectedFromListBox(args);
        });

        public IList<string> Names { get; } = new List<string>();
        public IList? selectedFromLeftList = null;
        public IList? selectedFromRightList = null;

        public ObservableCollection<string> Selected { get; } = new ObservableCollection<string>();
    }
}
