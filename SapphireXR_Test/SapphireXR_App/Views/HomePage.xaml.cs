using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using SapphireXR_App.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Views
{
  /// <summary>
  /// HomePage.xaml에 대한 상호 작용 논리
  /// </summary>``
  public partial class HomePage : Page
  {

    public HomePage()
    {
      InitializeComponent();
      DataContext = App.Current.Services.GetService(typeof(HomeViewModel));

    }



    private void UcFlowControl_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var fc = e.Source as UcFlowControl;
      FlowControlDialog fcDialog = new FlowControlDialog(fc.Name);
      fcDialog.Name = fc.Name;
      fcDialog.ShowDialog();

    }


    private void UcGasState_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      UcGasState ucGs = sender as UcGasState;
      MessageBoxResult result = MessageBox.Show($" {ucGs.Name} 상태입니다.", $"{ucGs.Name}");

    }

    private void UcValve_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      UcValve ucValve = e.Source as UcValve;
      MessageBoxResult result = MessageBox.Show($"{ucValve.Name} 밸브를 여시겠습니까?", $"밸브", MessageBoxButton.OKCancel);
      if (result == MessageBoxResult.Cancel)
      {

        MessageBox.Show($"밸브를 열었습니다.");
      }
    }


  }
}

