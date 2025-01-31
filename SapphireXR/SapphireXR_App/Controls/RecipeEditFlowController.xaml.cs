using SapphireXR_App.ViewModels.FlowController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SapphireXR_App.Controls
{
    public class SelectAllFocusBehavior
    {
        public static bool GetEnable(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(EnableProperty);
        }

        public static void SetEnable(FrameworkElement frameworkElement, bool value)
        {
            frameworkElement.SetValue(EnableProperty, value);
        }

        public static readonly DependencyProperty EnableProperty =
                 DependencyProperty.RegisterAttached("Enable",
                    typeof(bool), typeof(SelectAllFocusBehavior),
                    new FrameworkPropertyMetadata(false, OnEnableChanged));

        private static void OnEnableChanged
                   (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null) return;

            if (e.NewValue is bool == false) return;

            if ((bool)e.NewValue)
            {
                frameworkElement.GotFocus += SelectAll;
                frameworkElement.PreviewMouseDown += IgnoreMouseButton;
                frameworkElement.KeyUp += OnKeyUp;
            }
            else
            {
                frameworkElement.GotFocus -= SelectAll;
                frameworkElement.PreviewMouseDown -= IgnoreMouseButton;
                frameworkElement.KeyUp -= OnKeyUp;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var frameworkElement = e.OriginalSource as FrameworkElement;
            if (frameworkElement is TextBox)
            {
                ((TextBoxBase)frameworkElement).SelectAll();
            }
            else if (frameworkElement is PasswordBox)
            {
                ((PasswordBox)frameworkElement).SelectAll();
            }
        }

        private static void IgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null || frameworkElement.IsKeyboardFocusWithin) return;
            e.Handled = true;
            frameworkElement.Focus();
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (e.OriginalSource is TextBox)
                    {
                        ((TextBox)e.OriginalSource).Undo();
                    }
                    break;

                case Key.Enter:
                    if (e.OriginalSource is TextBox)
                    {
                        ((TextBox)e.OriginalSource).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        e.KeyboardDevice.ClearFocus();
                    }
                    else
                        if (e.OriginalSource is PasswordBox)
                        {
                            e.KeyboardDevice.ClearFocus();
                        }
                    break;
            }
        }
    }

    /// <summary>
    /// RecipeEditFlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipeEditFlowController : UserControl
    {
        public RecipeEditFlowController()
        {
            InitializeComponent();
            DataContext = new RecipeEditFlowControllerViewModel();
        }

        public string? Type { get; set; }
        required public string ControllerID { get; set; }
    }
}
