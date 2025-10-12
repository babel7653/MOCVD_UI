using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using SapphireXR_App.Views;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    internal partial class MOSourceSettingViewModel: ObservableObject
    {
        internal MOSourceSettingViewModel(string titleStr, string sourceNameStr, double leftD, double topD, bool topMostB, Action onClosedAC)
        {
            title = titleStr;
            sourceName = sourceNameStr;
            Left = leftD;
            Top = topD;
            TopMost = topMostB;
            onClosed = onClosedAC;
            SourceModel = new MOSourceModel(sourceNameStr);
        }

        [RelayCommand]
        private void Close(Window window)
        {
            window.Close();
        }

        [RelayCommand]
        private void Closed()
        {
            onClosed();
        }

        [RelayCommand]
        private void MouseLeftButtonDown(object? arg)
        {
            object[]? args = arg as object[];
            if(args != null && args.Length == 2)
            {
                Window? moSourceView = args[0] as MOSourceSettingView;
                if (moSourceView != null && (args[1] as MouseButtonEventArgs)?.LeftButton == MouseButtonState.Pressed)
                {
                    moSourceView.DragMove();
                }
            }
        }

        [ObservableProperty]
        private string title;
        [ObservableProperty]
        private double left;
        [ObservableProperty]
        private double top;
        [ObservableProperty]
        private bool topMost;
        [ObservableProperty]
        private string sourceName;

        private Action onClosed;

        public MOSourceModel SourceModel { get; }
    }
}
