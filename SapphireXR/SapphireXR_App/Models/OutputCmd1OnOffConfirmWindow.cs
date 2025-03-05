using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Models
{
    internal static class OutputCmd1OnOffConfirmWindow
    {
        internal static void Show(string onOff, PLCService.OutputCmd2Index index)
        {
            string nextState = (onOff == "On" ? "Off" : "On");
            if (ValveOperationEx.Show("Moduel Power 상태 변경", nextState + " 상태로 변경하시겠습니까?") == Enums.ValveOperationExResult.Ok)
            {
                PLCService.WriteOutputCmd2OnOffState(index, (nextState == "On") ? true : false);
            }
        }
    }
}
