using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class ManualBatchViewModel: ObservableObject
    {
        public partial class IOUserState: ObservableObject
        {
            [ObservableProperty]
            private string _iD = "";

            [ObservableProperty]
            private string _name = "";
        }

        public partial class AnalogIOUserState: IOUserState
        {
            [ObservableProperty]
            private int _value;

            [ObservableProperty]
            private int _maxValue;
        }

        public partial class DigitalIOUserState: IOUserState
        {
            [ObservableProperty]
            private bool _on;
        }

        public partial class Batch: ObservableObject
        {
            [ObservableProperty]
            private string _name = "";

            [ObservableProperty]
            private int? _rampingTime = null;

            [ObservableProperty]
            private IList<AnalogIOUserState> _analogIOUserStates = new List<AnalogIOUserState>();

            [ObservableProperty]
            private IList<DigitalIOUserState> _digitalIOUserStates = new List<DigitalIOUserState>();
        }

        public ManualBatchViewModel()
        {
            Batches.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => {
                MinusCommand.NotifyCanExecuteChanged();
                LoadToPLCCommand.NotifyCanExecuteChanged();
                SaveCommand.NotifyCanExecuteChanged();
            };
        }

        bool batchesEmpty()
        {
            return 0 < Batches.Count;
        }

        [RelayCommand(CanExecute = "batchesEmpty")]
        private void LoadToPLC()
        {

        }


        [RelayCommand(CanExecute = "batchesEmpty")]
        private void Save()
        {

        }

        public ICommand AddCommand => new RelayCommand(() =>
        {
            Batch newBatch = new Batch() { Name = "UserState" + (Batches.Count + 1) };
            foreach((string flowController, int idx) in PLCService.dIndexController)
            {
                newBatch.AnalogIOUserStates.Add(new AnalogIOUserState() { ID = flowController, MaxValue = (int)PLCService.ReadMaxValue(flowController) });
            }
            foreach((string valve, int idx) in PLCService.ValveIDtoOutputSolValveIdx1)
            {
                newBatch.DigitalIOUserStates.Add(new DigitalIOUserState() { ID = valve });
            }
            foreach ((string valve, int idx) in PLCService.ValveIDtoOutputSolValveIdx2)
            {
                newBatch.DigitalIOUserStates.Add(new DigitalIOUserState() { ID = valve });
            }
            Batches.Add(newBatch);
            CurrentBatch = newBatch;

        });

       
        [RelayCommand(CanExecute = "batchesEmpty")]
        private void Minus()
        {
            if (CurrentBatch != null)
            {
                Batches.Remove(CurrentBatch);
                CurrentBatch = null;
            }
        }

        [ObservableProperty]
        private ObservableCollection<Batch> _batches = new ObservableCollection<Batch>();

        [ObservableProperty]
        private Batch? _currentBatch = null;
    }
}
