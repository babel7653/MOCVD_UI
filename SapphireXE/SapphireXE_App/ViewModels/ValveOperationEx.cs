using CustomDialogSample1.Enums;
using CustomDialogSample1.ViewModels;
using CustomDialogSample1.Views;

namespace CustomDialogSample1.Models
{
    public static class ValveOperationEx
    {
        public static ValveOperationExResult Show(string title, string message)
        {
            return Show(title, message, "확인", "취소");
        }
        public static ValveOperationExResult Show(string title, string message, string okText)
        {
            return Show(title, message, okText, null);
        }

        public static ValveOperationExResult Show(string title, string message, string okText, string? cancelText)
        {
            var viewModel = new ValveOperationViewModel(title, message, okText, cancelText);
            ValveOperationView view = new ValveOperationView
            {
                DataContext = viewModel
            };
            view.ShowDialog();

            return viewModel.ValveOperationExResult;
        }
    }
}
