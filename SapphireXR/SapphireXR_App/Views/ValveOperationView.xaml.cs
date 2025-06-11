﻿using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    public partial class ValveOperationView : Window
    {
        private void MessageBoxView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public ValveOperationView()
        {
            InitializeComponent();
            MouseLeftButtonDown += MessageBoxView_MouseLeftButtonDown;
        }
    }
}
