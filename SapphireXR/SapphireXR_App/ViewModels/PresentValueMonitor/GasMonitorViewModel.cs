using SapphireXR_App.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.ViewModels
{
    public class GasMonitorViewModel: PresentValueMonitorViewModel
    {
        protected override void updatePresentValue(float value)
        {
            PresentValue = value.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = Util.NumberDecimalDigits(value, GlobalSetting.MaxNumberDigit) });
        }
    }
}
