using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public class SingleValveViewModel: ValveViewModel
    {
        protected override void OnLoaded(string? valveID)
        {
            base.OnLoaded(valveID);
            model = new OnOffModel(ValveID!);
        }

        public bool IsNormallyOpen
        {
            get { return (bool)GetValue(IsNormallyOpenProperty); }
            set { SetValue(IsNormallyOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNormallyOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormallyOpenProperty =
            DependencyProperty.Register("IsNormallyOpen", typeof(bool), typeof(SingleValve), new PropertyMetadata(default));

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
