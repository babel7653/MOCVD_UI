using System.Globalization;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using static SapphireXR_App.ViewModels.FlowControlViewModel;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// FlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowController : UserControl
    {
        public FlowController()
        {
            InitializeComponent();
            DataContext = new FlowControllerViewModel();
        }

        public string? Type { get; set; }
        public string? ControllerID { get; set; }

        private void FlowController_Click(object sender, RoutedEventArgs e) 
        {
            if (flowControlView == null)
            {
                flowController = (FlowController)((Button)e.OriginalSource).Parent;
                if (flowController != null)
                {
                    flowControlView = FlowControllerEx.Show("Flow Controller", $"{ControllerID} 유량을 변경하시겠습니까?", (PopupExResult result, ControlValues controlValues) => { 
                        if(OnFlowControllerConfirmed == null)
                        {
                            OnFlowControllerConfirmed = ((FlowControllerViewModel)DataContext).OnFlowControllerConfirmedCommand;
                        }
                        OnFlowControllerConfirmed?.Execute(new object[2] { result, controlValues }); 
                    },
                    (PopupExResult result) => {
                        if(OnFlowControllerCanceled == null)
                        {
                            OnFlowControllerCanceled = ((FlowControllerViewModel)DataContext).OnFlowControllerCanceledCommand;
                        }
                        OnFlowControllerCanceled?.Execute(result); 
                    });
                    Point p = TransformToAncestor(Util.FindParent<Window>(this, "mainWindow")).Transform(new Point(0, 0));
                    flowControlView.Left = p.X;
                    flowControlView.Top = p.Y;
                    flowControlView.Topmost = true;
                    flowControlView.Closed += (object? sender, EventArgs e) =>
                    {
                        flowControlView = null; 
                    };
                }
            }
            else
            {
                flowControlView.Focus();
            }
        }

        private FlowControlView? flowControlView = null;
        private ICommand? OnFlowControllerConfirmed { get; set; }
        private ICommand? OnFlowControllerCanceled { get; set; }
    }

    public class OnLoadedCommandParamConverver : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[]) value;
        }
    }
}
