using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.WindowServices;
using SapphireXR_App.Enums;
using System.Windows;
using SapphireXR_App.Models;
using System.Collections;

namespace SapphireXR_App.ViewModels.Valve
{
    internal partial class OnSwitchViewModel: ObservableObject
    {
        internal class SwitchStateUpdater 
        {
            internal SwitchStateUpdater(OnSwitchViewModel vm)
            {
                viewModel = vm;
            }

            protected OnSwitchViewModel viewModel;
        }
        internal class ClampSwitchStateUpdater : SwitchStateUpdater 
        {
            internal ClampSwitchStateUpdater(OnSwitchViewModel vm) : base(vm) 
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(IsOn))
                    {
                        onOffChanged();
                    }
                };

                PLCConnectionState.Instance.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(PLCConnectionState.Online))
                    {
                        initOnOff();
                    }
                };
                if (PLCConnectionState.Instance.Online == true)
                {
                    initOnOff();
                }
            }

            private void onOffChanged()
            {
                PLCService.WriteClampOnOff(viewModel.IsOn);
            }

            private void initOnOff()
            {
                BitArray outputCmd1 = PLCService.ReadOutputCmd1();
                if (outputCmd1[(int)PLCService.OutputCmd1Index.ClampOpen] == true)
                {
                    viewModel.IsOn = true;
                }
                else
                {
                    viewModel.IsOn = false;
                }
            }
        }

        [ObservableProperty]
        private bool isOn = false;

        private string? id = null;
        private SwitchStateUpdater? switchStateUpdater = null;

        [RelayCommand]
        private void OnLoaded(object? args)
        {
            if (args != null)
            {
                string? switchID = args as string;
                if (switchID is not null)
                {
                    id = switchID;
                    switch (switchID)
                    {
                        case "Clamp":
                            switchStateUpdater = new ClampSwitchStateUpdater(this);
                            break;
                    }
                }
            }
        }

        [RelayCommand]
        private void OnClick()
        {
            string onOff = IsOn == true ? "Off" : "On";
            var result = ValveOperationEx.Show("Switch Operation", $"{id} {onOff} 하시겠습니까?");
            switch (result)
            {
                case DialogResult.Ok:
                    IsOn = !IsOn;
                    MessageBox.Show($"{id} 스위치 {onOff}");
                    break;
                case DialogResult.Cancel:
                    MessageBox.Show($"{id} 스위치 변경 취소");
                    break;
            }
        }
    }
}
