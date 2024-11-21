using System.Windows;

namespace CustomDialogSample1.Views
{
    /// <summary>
    /// OkCancelDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OkCancelDialog : Window
    {
        public OkCancelDialog(string title, string message)
        {
            InitializeComponent();
            this.Title = title;
            tbMessanger.Text = message;
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
