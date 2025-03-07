using System.Windows;

namespace SapphireXR_App.ViewModels
{
    class ButterflyValveViewModel : ValveViewModel
    {
        protected override void OnClicked()
        {
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
