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




    public string UcGasName
    {
      get { return (string)GetValue(UcGasNameProperty); }
      set { SetValue(UcGasNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcGasName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcGasNameProperty =
        DependencyProperty.Register("UcGasName", typeof(string), typeof(UcGasState), new PropertyMetadata(default));



    public int UcGasPressure
    {
      get { return (int)GetValue(UcGasPressureProperty); }
      set { SetValue(UcGasPressureProperty, value); }
    }

    // Using a DependencyProperty as the backing store for GasPressure.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcGasPressureProperty =
        DependencyProperty.Register("UcGasPressure", typeof(int), typeof(UcGasState), new PropertyMetadata(default));


  }
}
