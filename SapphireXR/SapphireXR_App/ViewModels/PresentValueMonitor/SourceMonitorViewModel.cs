using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.WindowServices;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    internal partial class SourceMonitorViewModel: PresentValueMonitorViewModel, IObserver<bool>
    {
        internal SourceMonitorViewModel()
        {
            ObservableManager<bool>.Subscribe("App.Closing", this);
        }

        [RelayCommand]
        private void MouseDoubleClick(object? args)
        {
            UserControl? sourceSettingView = args as UserControl;
            if (sourceSettingView != null)
            {
                if (moSourceSettingView == null)
                {
                    Point p = sourceSettingView.TransformToAncestor(Util.FindParent<Window>(sourceSettingView, "mainWindow")).Transform(new Point(0, 0));
                    MOSourceSettingViewModel viewModel = new MOSourceSettingViewModel("MO Source Setting - " + Util.GetGasDeviceName(moControllerID), p.X, p.Y, true, () => moSourceSettingView = null);
                    moSourceSettingView = MOSourceSettingWindow.Show(viewModel);
                }
                else
                {
                    moSourceSettingView.Focus();
                }
            }
        }

        protected override void onLoaded(string id)
        {
            base.onLoaded(id);
            moControllerID = id;
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
            moSourceSettingView?.Close();
        }

        private static readonly Brush DefaultSourceMonitorColorOnMouseOver = (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false ? App.Current.Resources.MergedDictionaries[0]["SourceMonitorColorOnMouseOver"] as Brush : null) ?? new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));

        [ObservableProperty]
        private Brush onMouseOverBackground = DefaultSourceMonitorColorOnMouseOver;
        private Window? moSourceSettingView = null;
        private string moControllerID = string.Empty;
    }
}
