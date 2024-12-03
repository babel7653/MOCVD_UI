using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphireXR_App.Controls;
using System.Windows;
using TwinCAT.TypeSystem;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
  public class UcFlowControlViewModel : ObservableObject
  {
    public static DispatcherTimer? _timer;

    /// <summary>
    /// 기본 생성자
    /// </summary>
    public UcFlowControlViewModel()
    {
      ValveStateUpdate();

    }

    public string NumValve { get; set; }
    public string SetVal { get; set; }
    public string CurVal { get; set; }


    int initSetValue = 3000;
    int initCurValue = 300;
    public string sNumValve { get; set; }
    public string sSetVal { get; set; }
    public string sCurVal { get; set; }

    public UcFlowControl MFC02 { get; set; }
    public void ValveStateUpdate()
    {
      if (_timer is null)
      {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        _timer.Tick += ValveStateData;
        _timer.Tick += UserControl_Loaded;
      }
      _timer.Start();
    }

    private void ValveStateData(object? sender, EventArgs e)
    {
      var eName = e.GetType().Name;
      sNumValve = "mfc001";
      initSetValue = initSetValue + 2;
      initCurValue = initCurValue + 3;
      sSetVal = initSetValue.ToString();
      sCurVal = initCurValue.ToString();
      OnPropertyChanged(nameof(sNumValve));
      OnPropertyChanged(nameof(sSetVal));
      OnPropertyChanged(nameof(sCurVal));


    }
    private void UserControl_Loaded(object sender, EventArgs e)
    {
      for (int i = 0; i < 4; i++)
      {
        if (MFC02.Name == "MFC01")
        {
          sNumValve = MFC02.Name;
          sSetVal = MainViewModel.MfcSetValue[0].ToString();
          sCurVal = MainViewModel.MfcCurValue[0].ToString();
        }
        else if (MFC02.Name == "MFC02")
        {
          sNumValve = MFC02.Name;
          sSetVal = MainViewModel.MfcSetValue[1].ToString();
          sCurVal = MainViewModel.MfcCurValue[1].ToString();
        }

        else
        {
          sNumValve = MFC02.Name;
          sSetVal = MainViewModel.MfcSetValue[3].ToString();
          sCurVal = MainViewModel.MfcCurValue[3].ToString();
        }

      }

    }
  }
}
