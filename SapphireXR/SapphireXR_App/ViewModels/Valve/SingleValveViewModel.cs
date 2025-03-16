using SapphireXR_App.ViewModels.Valve;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public class SingleValveViewModel: OnOffValveViewModel
    {
        protected override PopupMessage getPopupMessage()
        {
            return popUpMessage;
        }

        public SingleValveViewModel(): base()
        {
            OnLoadedCommand = new RelayCommand<object?>((object? args) =>
            {
                if (args != null)
                {
                    object[] argArray = (object[])args;
                    if (4 <= argArray.Length)
                    {
                        if (argArray[0] is string && argArray[1] is Controls.Valve.UpdateTarget)
                        {
                            Init((string)argArray[0], (Controls.Valve.UpdateTarget)argArray[1]);
                            if (argArray[2] is Controls.SingleValve.Interlock && argArray[3] is string)
                            {
                                switch((Controls.SingleValve.Interlock)argArray[2])
                                {
                                    case Controls.SingleValve.Interlock.Right:
                                        OnColor = Brushes.OrangeRed; 
                                        OffColor = Brushes.White;
                                        PLCService.AddCoupledValves((string)argArray[3], ValveID!);
                                        popUpMessage = CreateDefaultPopupMessage(ValveID!);
                                        break;

                                    case Controls.SingleValve.Interlock.Left:
                                        popUpMessage = CreateInversePopupMessage(ValveID!);
                                        OnColor = Brushes.White;
                                        OffColor = Brushes.Lime;
                                        break;

                                    default:
                                        OnColor = Brushes.Lime;
                                        OffColor = Brushes.White;
                                        popUpMessage = CreateDefaultPopupMessage(ValveID!);
                                        break;
                                }
                               
                            }
                        }
                    }
                }
            });
        }

        private static PopupMessage CreateDefaultPopupMessage(string valveID)
        {
            return new PopupMessage()
            {
                messageWithOpen = $"{valveID} 밸브를 닫으시겠습니까?",
                confirmWithOpen = $"{valveID} 밸브 닫음",
                cancelWithOpen = $"{valveID} 취소됨",
                messageWithoutOpen = $"{valveID} 밸브를 열겠습니까?",
                confirmWithoutOpen = $"{valveID} 밸브 열음",
                cancelWithoutOpen = $"{valveID} 취소됨"
            };
        }

        private static PopupMessage CreateInversePopupMessage(string valveID)
        {
            return new PopupMessage()
            {
                messageWithoutOpen = $"{valveID} 밸브를 닫으시겠습니까?",
                confirmWithoutOpen = $"{valveID} 밸브 닫음",
                cancelWithOpen = $"{valveID} 취소됨",
                messageWithOpen = $"{valveID} 밸브를 열겠습니까?",
                confirmWithOpen = $"{valveID} 밸브 열음",
                cancelWithoutOpen = $"{valveID} 취소됨"
            };
        }

        private PopupMessage popUpMessage = new PopupMessage() { cancelWithOpen = "", cancelWithoutOpen = "", confirmWithOpen = "", confirmWithoutOpen = "", messageWithOpen = "", messageWithoutOpen = ""}; 

        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }
        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(Brush), typeof(SingleValveViewModel), new PropertyMetadata(Brushes.Transparent));

        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }
        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(Brush), typeof(SingleValveViewModel), new PropertyMetadata(Brushes.Transparent));

        public bool IsNormallyOpen
        {
            get { return (bool)GetValue(IsNormallyOpenProperty); }
            set { SetValue(IsNormallyOpenProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsNormallyOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormallyOpenProperty =
            DependencyProperty.Register("IsNormallyOpen", typeof(bool), typeof(SingleValveViewModel), new PropertyMetadata(default));
    }
}
