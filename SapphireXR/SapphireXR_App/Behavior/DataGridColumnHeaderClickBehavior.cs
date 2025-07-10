using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;

namespace SapphireXR_App.Behavior
{
    public static class DataGridColumnHeaderClickBehavior
    {
        // Define the attached property for the Command
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "ClickCommand",
                typeof(ICommand),
                typeof(DataGridColumnHeaderClickBehavior),
                new PropertyMetadata(null, OnClickCommandChanged)); // Callback when command changes

        // Getter for the attached property
        public static ICommand GetClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClickCommandProperty);
        }

        // Setter for the attached property
        public static void SetClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClickCommandProperty, value);
        }

        // Callback method when the ClickCommand attached property is set or changed
        private static void OnClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridColumnHeader header)
            {
                if (e.NewValue is ICommand newCommand)
                {
                    // Hook the Click event when the command is set
                    header.Click += Header_Click;
                }
                else if (e.OldValue is ICommand oldCommand)
                {
                    // Unhook the Click event when the command is unset
                    header.Click -= Header_Click;
                }
            }
        }

        // Event handler for the DataGridColumnHeader's Click event
        private static void Header_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridColumnHeader header)
            {
                ICommand command = GetClickCommand(header); // Get the bound command
                if (command != null)
                {
                    // You can choose what to pass as the command parameter.
                    // Common choices:
                    // 1. header.Column: The DataGridColumn itself (useful for sorting based on the column)
                    // 2. header.Column.SortMemberPath: The property name to sort by
                    // 3. header.Content: The header's display text
                    object commandParameter = header.Column; // Or header.Column.SortMemberPath;

                    if (command.CanExecute(commandParameter))
                    {
                        command.Execute(commandParameter);
                    }
                }
            }
        }
    }
}
