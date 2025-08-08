﻿using SapphireXR_App.Models;

namespace SapphireXR_App.WindowServices
{
    internal static class OutputCmd1ToggleConfirmService
    {
        internal static bool Toggle(PLCService.OutputCmd1Index index, string title, string message, string currentState, string stateOnTrue, string stateOnFalse)
        {
            if (ValveOperationEx.Show(title, message) == Enums.DialogResult.Ok)
            {
                PLCService.WriteOutputCmd1(index, !(currentState == stateOnTrue ? true : false));
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool OnOff(string onOff, PLCService.OutputCmd1Index index, string title)
        {
            return Toggle(index, title, (onOff == "On" ? "Off" : "On") + " 상태로 변경하시겠습니까?", onOff, "On", "Off");
        }
    }
}
