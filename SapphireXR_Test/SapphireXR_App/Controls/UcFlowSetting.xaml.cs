using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
  /// <summary>
  /// UcFlowSetting.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class UcFlowSetting : UserControl
  {


    public int UcTargetVal
    {
      get { return (int)GetValue(UcTargetValProperty); }
      set { SetValue(UcTargetValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcTargetVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcTargetValProperty =
        DependencyProperty.Register("UcTargetVal", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int UcRampTime
    {
      get { return (int)GetValue(UcRampTimeProperty); }
      set { SetValue(UcRampTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcRampTime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcRampTimeProperty =
        DependencyProperty.Register("UcRampTime", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int UcDeviation
    {
      get { return (int)GetValue(UcDeviationProperty); }
      set { SetValue(UcDeviationProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcDeviation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcDeviationProperty =
        DependencyProperty.Register("UcDeviation", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int UcCurVal
    {
      get { return (int)GetValue(UcCurValProperty); }
      set { SetValue(UcCurValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcCurVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcCurValProperty =
        DependencyProperty.Register("UcCurVal", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int UcConVal
    {
      get { return (int)GetValue(UcConValProperty); }
      set { SetValue(UcConValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcConVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcConValProperty =
        DependencyProperty.Register("UcConVal", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int UcMaxVal
    {
      get { return (int)GetValue(UcMaxValProperty); }
      set { SetValue(UcMaxValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcMaxVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcMaxValProperty =
        DependencyProperty.Register("UcMaxVal", typeof(int), typeof(UcFlowSetting), new PropertyMetadata(default));



    public int TargetVal = 3450;
    public int RampTime = 120;
    public int Deviation = 10;
    public int CurVal = 2340;
    public int ConVal = 333;
    public int MaxVal = 4000;
    UcFlowSetting ucFlowSetting { get; set; }



    public UcFlowSetting()
    {
      InitializeComponent();
    }


    private void OK_Click(object sender, RoutedEventArgs e)
    {
      //Button btn = (Button)sender;
      //Button btnOK = e.Source as Button;
      //ucFlowSetting = btnOK.Parent as UcFlowSetting;
      var name = ucFlowSetting.Name;

      TargetVal = ucFlowSetting.UcTargetVal;
      RampTime = ucFlowSetting.UcRampTime;
      Deviation = ucFlowSetting.UcDeviation;
      CurVal = ucFlowSetting.UcCurVal;
      ConVal = ucFlowSetting.UcConVal;
      MaxVal = ucFlowSetting.UcMaxVal;


    }

    private void Cancel_Click_1(object sender, RoutedEventArgs e)
    {

    }

    private void ucfs_Loaded(object sender, RoutedEventArgs e)
    {

      ucFlowSetting = e.Source as UcFlowSetting;
      var name = ucFlowSetting.Name;
      ucFlowSetting.UcTargetVal = TargetVal;
      ucFlowSetting.UcRampTime = RampTime;
      ucFlowSetting.UcDeviation = Deviation;
      ucFlowSetting.UcCurVal = CurVal;
      ucFlowSetting.UcConVal = ConVal;
      ucFlowSetting.UcMaxVal = MaxVal;

    }

  }
}
