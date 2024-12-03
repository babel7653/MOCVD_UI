using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
  /// <summary>
  /// UcFlowControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class UcFlowControl : UserControl
  {
    public UcFlowControl()
    {
      InitializeComponent();
      //DataContext = App.Current.Services.GetService(typeof(UcFlowControlViewModel)) as UcFlowControlViewModel;
    }




    public string UcName
    {
      get { return (string)GetValue(UcNameProperty); }
      set { SetValue(UcNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NumValve.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcNameProperty =
        DependencyProperty.Register("UcName", typeof(string), typeof(UcFlowControl), new PropertyMetadata(default));



    public int UcSetVal
    {
      get { return (int)GetValue(UcSetValProperty); }
      set { SetValue(UcSetValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SetVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcSetValProperty =
        DependencyProperty.Register("UcSetVal", typeof(int), typeof(UcFlowControl), new PropertyMetadata(0));



    public int UcCurVal
    {
      get { return (int)GetValue(UcCurValProperty); }
      set { SetValue(UcCurValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UcCurValProperty =
        DependencyProperty.Register("UcCurVal", typeof(int), typeof(UcFlowControl), new PropertyMetadata(0));


  }
}
