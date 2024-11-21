using CustomDialogSample1.Enums;
using CustomDialogSample1.ViewModels;
using CustomDialogSample1.Views;

namespace CustomDialogSample1.Models
{
    public static class MessageBoxEx
    {
        public static MessageBoxExResult Show(string title, string message)
        {
            return Show(title, message, "확인", null, null);
        }
        public static MessageBoxExResult Show(string title, string message, string okText)
        {
            return Show(title, message, okText, null, null);
        }

        public static MessageBoxExResult Show(string title, string message, string okText, string? cancelText)
        {
            return Show(title, message, okText, null, cancelText);
        }

        public static MessageBoxExResult Show(string title, string message, string button1Text, string? button2Text, string? button3Text)
        {
            var viewModel = new MessageBoxViewModel(title, message, button1Text, button2Text, button3Text);
            MessageBoxView view = new MessageBoxView
            {
                DataContext = viewModel
            };
            view.ShowDialog();

            return viewModel.MessageBoxExResult;
        }
    }
}
