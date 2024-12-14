using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Models;
using SapphireXR_App.Enums;
using System.Windows;
using SapphireXR_App.Controls;
using System.Windows.Controls;

namespace SapphireXR_App.ViewModels
{
    public class LeakTestValveViewModel: ValveViewModel
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
                var result = ValveOperationEx.Show("Valve Operation", $"{ValveID} 밸브를 닫으시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        IsOpenObservable = !(IsOpen);
                        MessageBox.Show($"{ValveID} 밸브 닫음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{ValveID} 취소됨1");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{ValveID} 밸브를 열겠습니까?");
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
