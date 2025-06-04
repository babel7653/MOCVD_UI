using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using SapphireXR_App.ViewModels.FlowController;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// FlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeFlowController : UserControl
    {
        private class AppClosingSubscriber : IObserver<bool>
        {
            public AppClosingSubscriber(HomeFlowController view)
            {
                homeFlowController = view;
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
                if(homeFlowController.flowControlView != null)
                {
                    homeFlowController.flowControlView.Close();
                }
            }

            private HomeFlowController homeFlowController;
        }

        public HomeFlowController()
        {
            InitializeComponent();
            DataContext = new HomeFlowControllerViewModel();
            ObservableManager<bool>.Subscribe("App.Closing", onAppClosingSubscriber = new AppClosingSubscriber(this));
        }

        public string? Type { get; set; }
        required public string ControllerID { get; set; }

        private void FlowController_Click(object sender, RoutedEventArgs e) 
        {
            if (flowControlView == null)
            {
                homeFlowController = (HomeFlowController)((Button)sender).Parent;
                if (homeFlowController != null)
                {
                    flowControlView = FlowControllerEx.Show("Flow Controller", $"{ControllerID} 유량을 변경하시겠습니까?", ControllerID, (PopupExResult result, FlowControlViewModel.ControlValues controlValues) => { 
                        if(OnFlowControllerConfirmed == null)
                        {
                            OnFlowControllerConfirmed = ((HomeFlowControllerViewModel)DataContext).OnFlowControllerConfirmedCommand;
                        }
                        OnFlowControllerConfirmed?.Execute(new object[2] { result, controlValues }); 
                    },
                    (PopupExResult result) => {
                        if(OnFlowControllerCanceled == null)
                        {
                            OnFlowControllerCanceled = ((HomeFlowControllerViewModel)DataContext).OnFlowControllerCanceledCommand;
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
        private AppClosingSubscriber onAppClosingSubscriber;
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
