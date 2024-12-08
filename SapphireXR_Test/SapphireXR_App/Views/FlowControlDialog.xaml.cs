using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using Newtonsoft.Json;

namespace SapphireXR_App.Views
{
  /// <summary>
  /// TestWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class FlowControlDialog : Window
  {
    public List<GasAIO> GasSets { get; set; }

    public FlowControlDialog(string deviceName)
    {
      InitializeComponent();
      DataContext = this;
      //DataContext = App.Current.Services.GetService(typeof(FlowControlDialogViewModel));
      GasSets = SettingViewModel.sGasAIO;
    }


    public string FsName
    {
      get { return (string)GetValue(FsNameProperty); }
      set { SetValue(FsNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FsName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsNameProperty =
        DependencyProperty.Register("FsName", typeof(string), typeof(FlowControlDialog), new PropertyMetadata(default));


    public float FsTarVal
    {
      get { return (float)GetValue(FsTarValProperty); }
      set { SetValue(FsTarValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcTargetVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsTarValProperty =
        DependencyProperty.Register("FsTarVal", typeof(float), typeof(FlowControlDialog), new PropertyMetadata(default));


    public int FsRampTime
    {
      get { return (int)GetValue(FsRampTimeProperty); }
      set { SetValue(FsRampTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcRamptime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsRampTimeProperty =
        DependencyProperty.Register("FsRampTime", typeof(int), typeof(FlowControlDialog), new PropertyMetadata(default));


    public float FsDeviation
    {
      get { return (float)GetValue(FsDeviationProperty); }
      set { SetValue(FsDeviationProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcDeviation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsDeviationProperty =
        DependencyProperty.Register("FsDeviation", typeof(float), typeof(FlowControlDialog), new PropertyMetadata(default));


    public float FsCurVal
    {
      get { return (float)GetValue(FsCurValProperty); }
      set { SetValue(FsCurValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcCurVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsCurValProperty =
        DependencyProperty.Register("FsCurVal", typeof(float), typeof(FlowControlDialog), new PropertyMetadata(default));


    public int FsConVal
    {
      get { return (int)GetValue(FsConValProperty); }
      set { SetValue(FsConValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcConVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsConValProperty =
        DependencyProperty.Register("FsConVal", typeof(int), typeof(FlowControlDialog), new PropertyMetadata(default));


    public int FsMaxVal
    {
      get { return (int)GetValue(FsMaxValProperty); }
      set { SetValue(FsMaxValProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UcMaxVal.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FsMaxValProperty =
        DependencyProperty.Register("FsMaxVal", typeof(int), typeof(FlowControlDialog), new PropertyMetadata(3000));


    private FlowControlDialog? FcDialog;

    private void fsd_Loaded(object sender, RoutedEventArgs e)
    {
      FcDialog = e.Source as FlowControlDialog;
      if (FcDialog == null) return;
      if (FcDialog.FsMaxVal == 0) return;
      if (FcDialog.Name == null) return;
      if (GasSets is null) return;
      foreach (var gasSet in GasSets)
      {
        if (gasSet.ID == FcDialog.Name)
        {
          FcDialog.FsName = gasSet.ID;
          FcDialog.FsTarVal = gasSet.TargetValue;
          //FcDialog.FsRampTime = gasAIO.RampTime;
          FcDialog.FsCurVal = gasSet.CurrentValue;
          //FcDialog.FsConVal = gasAIO.ControlValue;
          FcDialog.FsMaxVal = gasSet.MaxValue;
          FcDialog.FsDeviation = Math.Abs(gasSet.CurrentValue) / gasSet.MaxValue * 100;
        }

      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      if (FcDialog == null) return;

      foreach (var gasSet in GasSets)
      {
        if (gasSet.ID == FcDialog.Name)
        {
          gasSet.TargetValue = FcDialog.FsTarVal;
          //gasAIO.RampTime = FcDialog.FsRampTime;
          gasSet.CurrentValue = FcDialog.FsCurVal;
          //gasAIO.ControlValue = FcDialog.FsConVal;
          gasSet.MaxValue = FcDialog.FsMaxVal;
        }
      }

      SettingViewModel.sGasAIO= GasSets;


      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
  }
}
