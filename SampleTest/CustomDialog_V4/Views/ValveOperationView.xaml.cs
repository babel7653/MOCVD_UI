﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomDialogSample1.Views
{
    /// <summary>
    /// ValveOperationView.xaml에 대한 상호 작용 논리
    /// </summary>
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
