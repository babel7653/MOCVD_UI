using SapphireXE_App.Enums;
using SapphireXE_App.Commands;
using System.Windows;
using System.Windows.Input;

namespace SapphireXE_App.ViewModels
{
    public partial class ValveOperationViewModel : ViewModelBase
    {
        public string Title 
        { 
            get;
            set;
        } = string.Empty;
        public string Message 
        {
            get;
            set;
        } = string.Empty;
        public string OkText 
        {
            get;
            set;
        } = string.Empty;
        public string CancelText 
        {
            get;
            set;
        } = string.Empty;

        public ValveOperationExResult ValveOperationExResult { get; internal set; } = ValveOperationExResult.Canel;

        public ICommand OkCommand => new RelayCommandT<Window>(onOk);
        private void onOk(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Ok;
            window.DialogResult = true;
        }


        public ICommand CancelCommand => new RelayCommandT<Window>(onCancel);
        private void onCancel(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Canel;
            window.DialogResult = false;
        }

        public ValveOperationViewModel(string title, string message, string okText, string? cancelText)
        {
            Title = title;
            OnPropertyChanged(nameof(Title));
            Message = message;
            OnPropertyChanged(nameof(Message));
            OkText = okText;
            OnPropertyChanged(nameof(OkText));
            CancelText = cancelText;
            OnPropertyChanged(nameof(CancelText));
        }
    }
}
