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
        protected override void Init(string? valveID)
        {
            base.Init(valveID);
            if (valveID != null)
            {
                IsOpen = PLCService.ReadValveState(valveID);
            }
            else
            {
                throw new Exception("ValveID is null, cannot set/get valve value from PLC\r\nCheck the SingleValve ValveID value");
            }
        }
        protected override PopupMessage getPopupMessage()
        {
            return new PopupMessage()
            {
                messageWithOpen = $"{ValveID} 밸브를 닫으시겠습니까?",
                confirmWithOpen = $"{ValveID} 밸브 닫음",
                cancelWithOpen = $"{ValveID} 취소됨1",
                messageWithoutOpen = $"{ValveID} 밸브를 열겠습니까?",
                confirmWithoutOpen = $"{ValveID} 밸브 열음",
                cancelWithoutOpen = $"{ValveID} 취소됨2"
            };
        }

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
