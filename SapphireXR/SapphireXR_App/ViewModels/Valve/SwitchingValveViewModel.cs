using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Models;
using SapphireXR_App.Enums;
using System.Windows;


namespace SapphireXR_App.ViewModels
{
    public class SwitchingValveViewModel: ValveViewModel
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
                throw new Exception("ValveID is null, cannot set/get valve value from PLC\r\nCheck the SwitchValve ValveID value");
            }
        }

        protected override PopupMessage getPopupMessage()
        {
            return new PopupMessage()
            {
                messageWithOpen = $"{ValveID} 밸브를 질소가스로 변경하시겠습니까?",
                confirmWithOpen = $"{ValveID} 밸브 닫음",
                cancelWithOpen = $"{ValveID} 취소됨1",
                messageWithoutOpen = $"{ValveID} 밸브를 공정가스로 변경하시겠습니까?",
                confirmWithoutOpen = $"{ValveID} 밸브 열음",
                cancelWithoutOpen = $"{ValveID} 취소됨2"
            };
        }
    }
}
