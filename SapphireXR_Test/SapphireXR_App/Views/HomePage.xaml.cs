using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
      UcFlowControl fc = e.Source as UcFlowControl;
      FlowControlDialog fcDialog = new FlowControlDialog(fc.Name);
      fcDialog.Name = fc.Name;
      fcDialog.ShowDialog();

    }


    private void UcGasState_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      MessageBox.Show("Gas 상태를 확인합니다.");
    }




  }
}
