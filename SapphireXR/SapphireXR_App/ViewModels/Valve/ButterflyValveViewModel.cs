using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using OxyPlot;
using SapphireXR_App.Controls;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    class ButterflyValveViewModel : ValveViewModel
    {
        protected override void Init(string? valveID)
        {
            base.Init(valveID);
        }

        public bool IsControl
        {
            get { return (bool)GetValue(IsControlProperty); }
            set { SetValue(IsControlProperty, value); }
        }

        public static readonly DependencyProperty IsControlProperty =
            DependencyProperty.Register("IsControl", typeof(bool), typeof(ButterflyValveViewModel), new PropertyMetadata(default));

        public int setValue
        {
            get { return (int)GetValue(setValueProperty); }
            set { SetValue(setValueProperty, value); }
        }

        public static readonly DependencyProperty setValueProperty =
            DependencyProperty.Register("setValue", typeof(int), typeof(ButterflyValveViewModel), new PropertyMetadata(0));
    }
}
