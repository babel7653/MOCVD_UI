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

        private class PLCConnectionStateSubscriber : IObserver<PLCConnection>
        {
            public PLCConnectionStateSubscriber(HomeFlowController vm)
            {
                homeFlowController = vm;
            }

            void IObserver<PLCConnection>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnNext(PLCConnection value)
            {
                homeFlowController.setFlowControllerClickHandlerByPLCConnection(value);
            }

            private HomeFlowController homeFlowController;
        }

#pragma warning disable CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        public HomeFlowController()
        {
            InitializeComponent();
            DataContext = new HomeFlowControllerViewModel();
            setFlowControllerClickHandlerByPLCConnection(PLCService.Connected);

            ObservableManager<bool>.Subscribe("App.Closing", onAppClosingSubscriber = new AppClosingSubscriber(this));
            ObservableManager<PLCConnection>.Subscribe("PLCService.Connected", plcConnectionStateSubscriber = new PLCConnectionStateSubscriber(this));
        }
#pragma warning restore CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.

        public string? Type { get; set; }
        required public string ControllerID { get; set; }

        private void setFlowControllerClickHandlerByPLCConnection(PLCConnection connection)
        {
            switch(connection)
            {
                case PLCConnection.Connected:
                    onFlowControllerClicked = (object sender, RoutedEventArgs e) =>
                    {
                        if (flowControlView == null)
                        {
                            homeFlowController = (HomeFlowController)((Button)sender).Parent;
                            if (homeFlowController != null)
                            {
                                flowControlView = FlowControllerEx.Show("Flow Controller", $"{ControllerID} 유량을 변경하시겠습니까?", ControllerID, (PopupExResult result, FlowControlViewModel.ControlValues controlValues) => {
                                    return ((HomeFlowControllerViewModel)DataContext).OnFlowControllerConfirmed(result, controlValues);
                                },
                                (PopupExResult result) => {
                                    ((HomeFlowControllerViewModel)DataContext).OnFlowControllerCanceled(result);
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
                    };
                    break;

                case PLCConnection.Disconnected:
                    onFlowControllerClicked = null;
                    break;
            }
        }

        private void FlowController_Click(object sender, RoutedEventArgs e) 
        {
            onFlowControllerClicked?.Invoke(sender, e);
        }

        private FlowControlView? flowControlView = null;
        private ICommand? OnFlowControllerConfirmed { get; set; }
        private ICommand? OnFlowControllerCanceled { get; set; }
        private AppClosingSubscriber onAppClosingSubscriber;
        private PLCConnectionStateSubscriber plcConnectionStateSubscriber;
        private Action<object, RoutedEventArgs>? onFlowControllerClicked = null;
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
