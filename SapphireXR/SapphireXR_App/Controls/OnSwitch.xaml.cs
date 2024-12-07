using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    public partial class OnSwitch : UserControl
    {
        public OnSwitch()
        {
            InitializeComponent();
        }
        public string SwitchID
        {
            get { return (string)GetValue(SwitchIDProperty); }
            set { SetValue(SwitchIDProperty, value); }
        }
        public static readonly DependencyProperty SwitchIDProperty =
            DependencyProperty.Register("SwitchID", typeof(string), typeof(OnSwitch), new PropertyMetadata(default));

        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(OnSwitch), new PropertyMetadata(default));

        private void OnSwitch_Click(object sender, RoutedEventArgs e)
        {
            OnSwitch Switch = (OnSwitch)((Button)e.OriginalSource).Parent;
            if (Switch.IsOn == true)
            {
                var result = ValveOperationEx.Show("Switch Operation", $"{Switch.SwitchID} OFF 하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        Switch.IsOn = !(Switch.IsOn);
                        MessageBox.Show($"{Switch.SwitchID} 스위치 OFF");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{Switch.SwitchID} 취소됨1");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Switch Operation", $"{Switch.SwitchID} ON 하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        Switch.IsOn = !(Switch.IsOn);
                        MessageBox.Show($"{Switch.SwitchID} 스위치 ON");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{Switch.SwitchID} 취소됨2");
                        break;
                }
            }
        }
    }
}
