﻿using SapphireXR_App.Views;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Models
{
    internal class ManualBatchEx
    {
        internal static void Show(ManualBatchViewModel manualBatchViewModel)
        {
            ManualBatchView manualBatchView = new ManualBatchView(manualBatchViewModel);
            manualBatchView.ShowDialog();
        }
    }
}
