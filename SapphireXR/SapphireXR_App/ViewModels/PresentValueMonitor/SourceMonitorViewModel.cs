using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    internal partial class SourceMonitorViewModel: PresentValueMonitorViewModel
    {
        private static readonly Brush DefaultSourceMonitorColorOnMouseOver = (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false ? App.Current.Resources.MergedDictionaries[0]["SourceMonitorColorOnMouseOver"] as Brush : null) ?? new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));

        [ObservableProperty]
        private Brush onMouseOverBackground = DefaultSourceMonitorColorOnMouseOver;

        [RelayCommand]
        private void MouseLeftButtonDown()
        { }
    }
}
