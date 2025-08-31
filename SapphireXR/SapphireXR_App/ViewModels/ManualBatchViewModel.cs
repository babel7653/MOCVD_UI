using System.Collections.ObjectModel;
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
            public bool valid()
            {
                return RampingTime != null && 0 < RampingTime;
            }

            [ObservableProperty]
            private string _name = "";

            [ObservableProperty]
            private int? _rampingTime = null;

            [ObservableProperty]
            private IList<AnalogIOUserState> _analogIOUserStates = new List<AnalogIOUserState>();

            [ObservableProperty]
            private IList<DigitalIOUserState> _digitalIOUserStates = new List<DigitalIOUserState>();
        }

        private class AlarmTriggeredSubscriber : IObserver<bool>
        {
            public AlarmTriggeredSubscriber(ManualBatchViewModel vm)
            {
                manualBatchViewModel = vm;
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
                if (value == true)
                {
                    manualBatchViewModel.loadBatchOnAlaramState();
                }
            }

            ManualBatchViewModel manualBatchViewModel;
        }

        public ManualBatchViewModel()
        {
            NotifyCollectionChangedEventHandler batchCollectionChanged = (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Batch batch in e.NewItems!)
                    {
                        BatchesSetForEvent.Add(batch);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Batch batch in e.OldItems!)
                    {
                        BatchesSetForEvent.Remove(batch);
                    }
                }
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
                        MinusCommand.NotifyCanExecuteChanged();
                        break;

                    case nameof(Batches):
                        Batches.CollectionChanged += batchCollectionChanged;
                        foreach(Batch batch in Batches)
                        {
                            batch.PropertyChanged += (sender, e) =>
                            {
                                if(e.PropertyName == nameof(Batch.RampingTime))
                                {
                                    RampingTimeErrorThickness = (batch.RampingTime != null ? RampingTimeThicknessNoError : RampingTimeThicknessError);
                                    LoadToPLCCommand.NotifyCanExecuteChanged();
                                }
                            };
                        }
                        MinusCommand.NotifyCanExecuteChanged();
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
                            
                            List<Batch?> batchesForEvent = new List<Batch?>();
                            batchesForEvent.Add(null);
                            foreach(Batch batch in Batches)
                            {
                                batchesForEvent.Add(batch);
                            }
                            BatchesSetForEvent = new ObservableCollection<Batch?>(batchesForEvent);

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
            ObservableManager<bool>.Subscribe("AlarmTriggered", alarmTriggeredSubscriber = new AlarmTriggeredSubscriber(this));
        }

        private bool canLoadToPLCCommand()
        {
            return PLCConnectionState.Instance.Online == true && CurrentBatch != null && CurrentBatch.valid();
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
            Batch newBatch = new Batch() { Name = "UserState" };
            foreach((string flowController, string fullName) in Util.RecipeFlowControlFieldToControllerID)
            {
                newBatch.AnalogIOUserStates.Add(new AnalogIOUserState() { ID = flowController, MaxValue = (int)SettingViewModel.ReadMaxValue(fullName)!, FullIDName = Util.RecipeFlowControlFieldToControllerID[flowController],
                    Name = SettingViewModel.ReadFlowControllerDeviceName(fullName)! });
            }
            foreach((string valve, int idx) in PLCService.ValveIDtoOutputSolValveIdx1)
            {
                newBatch.DigitalIOUserStates.Add(new DigitalIOUserState() { ID = valve, Name = SettingViewModel.ReadValveDeviceName(valve)! });
            }
            foreach ((string valve, int idx) in PLCService.ValveIDtoOutputSolValveIdx2)
            {
                newBatch.DigitalIOUserStates.Add(new DigitalIOUserState() { ID = valve, Name = SettingViewModel.ReadValveDeviceName(valve)! });
            }
            Batches.Add(newBatch);
            newBatch.PropertyChanged += (sender, e) =>
            {
                LoadToPLCCommand.NotifyCanExecuteChanged();
            };
            CurrentBatch = newBatch;
        });

        bool canMinusCommandExecute()
        {
            return 0 < Batches.Count && CurrentBatch != null;
        }
        [RelayCommand(CanExecute = "canMinusCommandExecute")]
        private void Minus()
        {
            if (CurrentBatch != null)
            {
                Batches.Remove(CurrentBatch);
                CurrentBatch = null;
            }
        }

        public ICommand OnClosingCommand => new RelayCommand<CancelEventArgs>((args) =>
        {
            string message = string.Empty;

            bool recipeEndInvalid = BatchOnRecipeEnd != null && BatchOnRecipeEnd.valid() == false;
            message += (recipeEndInvalid == true) ? "Recipe 종료 시 Batch (" + BatchOnRecipeEnd!.Name + ")" : string.Empty;

            bool recipeAlarmInvalid = BatchOnAlarmState != null && BatchOnAlarmState.valid() == false;
            message += (recipeAlarmInvalid == true) ? ((message != string.Empty) ? "와 " : string.Empty) + "Alarm 시 Batch (" + BatchOnAlarmState!.Name + ")" : string.Empty;
            
            if(message != string.Empty)
            {
                message += "의 Ramp Time 값이 유효한 값이 아닙니다. 값은 1 이상이어야 합니다";
                MessageBox.Show(message);

                if(recipeEndInvalid == true)
                {
                    CurrentBatch = BatchOnRecipeEnd;
                }
                if (recipeAlarmInvalid == true)
                {
                    CurrentBatch = BatchOnAlarmState;
                }

                args!.Cancel = true;
            }
            else
            {
                args!.Cancel = false;
            }
        });

        public void loadBatchOnRecipeEnd()
        {
            if (BatchOnRecipeEnd != null)
            {
                PLCService.WriteControlModeCmd(PLCService.ControlMode.Priority);
                Util.LoadBatchToPLC(BatchOnRecipeEnd);
                WindowServices.ToastMessage.Show("Recipe 종료 시 실행되도록 설정된 사용자 정의 Batch인 " + BatchOnRecipeEnd.Name + "가 실행됩니다. 실행이 완료될때까지 대기상태가 됩니다.", WindowServices.ToastMessage.MessageType.Information);
            }
        }

        private void loadBatchOnAlaramState()
        {
            if (BatchOnAlarmState != null)
            {
                PLCService.WriteControlModeCmd(PLCService.ControlMode.Priority);
                Util.LoadBatchToPLC(BatchOnAlarmState);
                WindowServices.ToastMessage.Show("알람 시 설정된 사용자 정의 Batch인 " + BatchOnAlarmState.Name + "가 실행됩니다. 실행이 완료될때까지 대기상태가 됩니다.", WindowServices.ToastMessage.MessageType.Information);
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

        private static readonly Thickness RampingTimeThicknessNoError = new Thickness(0, 0, 0, 0);
        private static readonly Thickness RampingTimeThicknessError = new Thickness(2, 2, 2, 2);

        [ObservableProperty]
        private ObservableCollection<Batch> _batches = new ObservableCollection<Batch>();

        [ObservableProperty]
        private ObservableCollection<Batch?> batchesSetForEvent = new ObservableCollection<Batch?>();

        [ObservableProperty]
        private Batch? _currentBatch = null;

        [ObservableProperty]
        private bool _nameEnabled = false;
        [ObservableProperty]
        private bool _rampingTimeEnabled = false;

        [ObservableProperty]
        private string? _batchFIlePath = null;

        [ObservableProperty]
        private Batch? _batchOnAlarmState = null;

        [ObservableProperty]
        private Batch? _batchOnRecipeEnd = null;

        [ObservableProperty]
        private Thickness rampingTimeErrorThickness = RampingTimeThicknessNoError;

        private AlarmTriggeredSubscriber alarmTriggeredSubscriber;
    }
}
