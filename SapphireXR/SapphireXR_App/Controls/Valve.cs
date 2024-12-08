using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;


namespace SapphireXR_App.Controls
{
    public class Valve : UserControl
    {
        public string ValveID
        {
            get { return (string)GetValue(ValveIDProperty); }
            set { SetValue(ValveIDProperty, value); }
        }
        public static readonly DependencyProperty ValveIDProperty =
            DependencyProperty.Register("ValveID", typeof(string), typeof(Valve), new PropertyMetadata(default));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Valve), new PropertyMetadata(default));
    }
}
