﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireXR_App.Common;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class ManualBatchViewModel: ObservableObject, IObserver<bool>
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

            public string FullIDName { get; set; } = "";
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
            NotifyCollectionChangedEventHandler batchCollectionChanged = (object? sender, NotifyCollectionChangedEventArgs e) => {
                MinusCommand.NotifyCanExecuteChanged();
            };
            PropertyChanging += (object? sender, PropertyChangingEventArgs args) => {
                switch(args.PropertyName)
                {
                    case nameof(Batches):
                        Batches.CollectionChanged -= batchCollectionChanged;
                        break;
                }
            };
            PropertyChanged += (object? sender, PropertyChangedEventArgs args) => {
                switch (args.PropertyName)
                {
                    case nameof(CurrentBatch):
                        NameEnabled = RampingTimeEnabled = CurrentBatch != null;
                        LoadToPLCCommand.NotifyCanExecuteChanged();
                        break;

                    case nameof(Batches):
                        Batches.CollectionChanged += batchCollectionChanged;
                        break;

                    case nameof(BatchFIlePath):
                        SaveCommand.NotifyCanExecuteChanged();
                        break;
                }
            };

            BatchFIlePath = Util.GetResourceAbsoluteFilePath("/Configurations/IOBatch.json");
            if (BatchFIlePath != null)
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(BatchFIlePath))
                    {
                        JToken ioBatchRoot = JToken.Parse(streamReader.ReadToEnd());

                        JToken? userStatesToken = ioBatchRoot["UserStates"];
                        if(userStatesToken != null)
                        {
                            Batches = JsonConvert.DeserializeObject<ObservableCollection<Batch>>(userStatesToken.ToString()) ?? Batches;

                            var getUserStateOnEvent = (string name) =>
                            {
                                string? value = (string?)Util.GetSettingValue(ioBatchRoot, name);
                                if(value != null)
                                {
                                    return Batches.FirstOrDefault((Batch element) => element.Name == value);
                                }

                                return null;
                            };
                            BatchOnRecipeEnd = getUserStateOnEvent("BatchOnRecipeEnd");
                            BatchOnAlarmState = getUserStateOnEvent("BatchOnAlarmState");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(BatchFIlePath + "의 파일로부터 로드하는데 실패하셨습니다." + "원인은 다음과 같습니다: " + ex.Message + " 빈 Batch를 로드합니다.");
                }
            }
            else
            {
                MessageBox.Show(BatchFIlePath + "의 경로를 찾을 수 없습니다. 빈 Batch를 로드하며 저장 기능이 비활성화 됩니다.");
            }

            ObservableManager<bool>.Subscribe("App.Closing", this);
        }

        bool batchesEmpty()
        {
            return 0 < Batches.Count;
        }

        private bool canLoadToPLCCommand()
        {
            return PLCService.Connected == Enums.PLCConnection.Connected && CurrentBatch != null && CurrentBatch.RampingTime != null && CurrentBatch.RampingTime != 0;
        }

        [RelayCommand(CanExecute = "canLoadToPLCCommand")]
        private void LoadToPLC()
        {
            if (CurrentBatch != null)
            {
                Util.LoadBatchToPLC(CurrentBatch);
            }
        }

        private bool canSaveCommandExecute()
        {
            return BatchFIlePath != null;
        }
        [RelayCommand(CanExecute = "canSaveCommandExecute")]
        private void Save()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(BatchFIlePath!))
                {
                    streamWriter.Write(new JObject(new JProperty("UserStates", JsonConvert.SerializeObject(Batches)), new JProperty("BatchOnRecipeEnd", JsonConvert.SerializeObject(BatchOnRecipeEnd != null ? BatchOnRecipeEnd.Name : null)), 
                        new JProperty("BatchOnAlarmState", JsonConvert.SerializeObject(BatchOnAlarmState != null ? BatchOnAlarmState.Name : null))).ToString());
                    streamWriter.Flush();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(BatchFIlePath + "로 Batch파일을 저장하면서 문제가 발생하였습니다. 문제는 다음과 같습니다: " + ex.Message);
            }

        }

        public ICommand AddCommand => new RelayCommand(() =>
        {
            Batch newBatch = new Batch() { Name = "UserState" + (Batches.Count + 1) };
            foreach((string flowController, string fullName) in Util.RecipeFlowControlFieldToControllerID)
            {
                newBatch.AnalogIOUserStates.Add(new AnalogIOUserState() { ID = flowController, MaxValue = (int)SettingViewModel.ReadMaxValue(fullName)!, FullIDName = Util.RecipeFlowControlFieldToControllerID[flowController] });
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
            newBatch.PropertyChanged += (sender, e) =>
            {
                LoadToPLCCommand.NotifyCanExecuteChanged();
            };
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

        public void loadBatchOnRecipeEnd()
        {
            if (BatchOnRecipeEnd != null)
            {
                Util.LoadBatchToPLC(BatchOnRecipeEnd);
                WindowServices.ToastMessage.Show("Recipe End Batch가 실행됩니다", WindowServices.ToastMessage.MessageType.Information);
            }
        }

        private void loadBatchOnAlaramState()
        {
            if (BatchOnAlarmState != null)
            {
                Util.LoadBatchToPLC(BatchOnAlarmState);
                WindowServices.ToastMessage.Show("Alarm State Batch가 실행됩니다", WindowServices.ToastMessage.MessageType.Information);
            }
        }

        void IObserver<bool>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<bool>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<bool>.OnNext(bool value)
        {
            Save();
        }

        [ObservableProperty]
        private ObservableCollection<Batch> _batches = new ObservableCollection<Batch>();

        [ObservableProperty]
        private Batch? _currentBatch = null;

        [ObservableProperty]
        private bool _nameEnabled = false;
        [ObservableProperty]
        private bool _rampingTimeEnabled = false;

        [ObservableProperty]
        private string? _batchFIlePath = null;

        [ObservableProperty]
        private Batch? _batchOnAlarmState;

        [ObservableProperty]
        private Batch? _batchOnRecipeEnd;
    }
}
