using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public class FlowEditorViewModel : DependencyObject, INotifyPropertyChanged
    {
        protected void Init(string? cl)
        {
            if (cl != null)
            {
                ContentLabel = cl;
            }
        }
        public ICommand OnLoadedCommand => new RelayCommand<string?>(Init);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }
        public static readonly DependencyProperty ControllerIDProperty =
           DependencyProperty.Register("ControllerID", typeof(string), typeof(FlowEditorViewModel), new PropertyMetadata(default));

        public float EditValue
        {
            get { return (float)GetValue(EditValueProperty); }
            set { SetValue(EditValueProperty, value); }
        }

        public static readonly DependencyProperty EditValueProperty =
            DependencyProperty.Register("EditValue", typeof(float), typeof(FlowEditorViewModel), new PropertyMetadata(default));

        public string ContentLabel
        {
            get { return (string)GetValue(contentLabelProperty); }
            set { 
                SetValue(contentLabelProperty, value);
                OnPropertyChanged(nameof(ContentLabel));
            }
        }
        static uint ContentLabelCount = 0;
        public readonly DependencyProperty contentLabelProperty =
            DependencyProperty.Register("ContentLabelProperty" + (ContentLabelCount++), typeof(string), typeof(FlowEditorViewModel), new PropertyMetadata(default));
    }
}
