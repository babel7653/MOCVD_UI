using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Models;
using SapphireXR_App.Enums;
using System.Windows;


namespace SapphireXR_App.ViewModels
{
    public class SwitchingValveViewModel: ValveViewModel
    {
        protected override void OnLoaded(string? valveID)
        {
            base.OnLoaded(valveID);
            model = new OnOffModel(ValveID!);
        }

        public ICommand LoadedCommand => new RelayCommand<string>(OnLoaded);
        public ICommand OnClickCommand => new RelayCommand(() => {
            if (IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{ValveID} 밸브를 질소가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        IsOpenObservable = !(IsOpen);

                        var ValveStateValue = HomeViewModel.aSolValvePLC;
                        MessageBox.Show($"{ValveID} 밸브 닫음");
                        //TODO

                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{ValveID} 취소됨1");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{ValveID} 밸브를 공정가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        IsOpenObservable = !(IsOpen);
                        MessageBox.Show($"{ValveID} 밸브 열음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{ValveID} 취소됨2");
                        break;
                }
            }
        });

        private OnOffModel? model;
    }
}
