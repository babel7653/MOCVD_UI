using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwinCAT.TypeSystem;

namespace SapphireXR_App.Controls
{
  /// <summary>
  /// UcGasState.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class UcGasState : UserControl
  {
    public UcGasState()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      UcGasState fc = e.Source as UcGasState;
      GasName.Text = fc.Name;
      PressureValue.Content = 55555.ToString();

    }
  }
}
