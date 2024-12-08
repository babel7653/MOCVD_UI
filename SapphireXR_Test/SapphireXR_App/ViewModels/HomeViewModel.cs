using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
  public class HomeViewModel : ViewModelBase
  {
    public static DispatcherTimer? _timer;
    public List<GasAIO> GasSets { get; set; }

    #region Gs Data property 
    public int GsH2Value { get; set; }
    public int GsN2Value { get; set; }
    public int GsNH3Value { get; set; }
    public int GsSiH4Value { get; set; }
    #endregion

    #region Fc Data property 
    public float FcSetVal01 { get; set; }
    public float FcSetVal02 { get; set; }
    public float FcSetVal03 { get; set; }
    public float FcSetVal04 { get; set; }
    public float FcSetVal05 { get; set; }
    public float FcSetVal06 { get; set; }
    public float FcSetVal07 { get; set; }
    public float FcSetVal08 { get; set; }
    public float FcSetVal09 { get; set; }
    public float FcSetVal10 { get; set; }
    public float FcSetVal11 { get; set; }
    public float FcSetVal12 { get; set; }
    public float FcSetVal13 { get; set; }
    public float FcSetVal14 { get; set; }
    public float FcSetVal15 { get; set; }
    public float FcSetVal16 { get; set; }
    public float FcSetVal17 { get; set; }
    public float FcSetVal18 { get; set; }
    public float FcSetVal19 { get; set; }
    public float FcSetVal20 { get; set; }
    public float FcSetVal21 { get; set; }
    public float FcSetVal22 { get; set; }
    public float FcSetVal23 { get; set; }
    public float FcSetVal24 { get; set; }
    public float FcSetVal25 { get; set; }
    public float FcSetVal26 { get; set; }

    public float FcCurVal01 { get; set; }
    public float FcCurVal02 { get; set; }
    public float FcCurVal03 { get; set; }
    public float FcCurVal04 { get; set; }
    public float FcCurVal05 { get; set; }
    public float FcCurVal06 { get; set; }
    public float FcCurVal07 { get; set; }
    public float FcCurVal08 { get; set; }
    public float FcCurVal09 { get; set; }
    public float FcCurVal10 { get; set; }
    public float FcCurVal11 { get; set; }
    public float FcCurVal12 { get; set; }
    public float FcCurVal13 { get; set; }
    public float FcCurVal14 { get; set; }
    public float FcCurVal15 { get; set; }
    public float FcCurVal16 { get; set; }
    public float FcCurVal17 { get; set; }
    public float FcCurVal18 { get; set; }
    public float FcCurVal19 { get; set; }
    public float FcCurVal20 { get; set; }
    public float FcCurVal21 { get; set; }
    public float FcCurVal22 { get; set; }
    public float FcCurVal23 { get; set; }
    public float FcCurVal24 { get; set; }
    public float FcCurVal25 { get; set; }
    public float FcCurVal26 { get; set; }
    #endregion

    // Valve Test 
    private void ValveOperation()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("GVL_IO.aDigitalInputIO");
        MainViewModel.Ads.WriteAny(hVar, 255);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }

    private void ValveStateUpdate()
    {
      if (_timer is null)
      {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        _timer.Tick += ValveStateData;
      }
      _timer.Start();
    }

    private void ValveStateData(object? sender, EventArgs e)
    {
      GasSets = SettingViewModel.sGasAIO;

      foreach (var gasSet in GasSets)
      {
        switch (gasSet.ID)
        {
          case "M01":
            FcSetVal01 = gasSet.TargetValue;
            FcCurVal01 = gasSet.CurrentValue;
            break;
          case "M02":
            FcSetVal02 = gasSet.TargetValue;
            FcCurVal02 = gasSet.CurrentValue;
            break;
          case "M03":
            FcSetVal03 = gasSet.TargetValue;
            FcCurVal03 = gasSet.CurrentValue;
            break;
          case "M04":
            FcSetVal04 = gasSet.TargetValue;
            FcCurVal04 = gasSet.CurrentValue;
            break;
          case "M05":
            FcSetVal05 = gasSet.TargetValue;
            FcCurVal05 = gasSet.CurrentValue;
            break;
          case "M06":
            FcSetVal06 = gasSet.TargetValue;
            FcCurVal06 = gasSet.CurrentValue;
            break;
          case "M07":
            FcSetVal07 = gasSet.TargetValue;
            FcCurVal07 = gasSet.CurrentValue;
            break;
          case "M08":
            FcSetVal08 = gasSet.TargetValue;
            FcCurVal08 = gasSet.CurrentValue;
            break;
          case "M09":
            FcSetVal09 = gasSet.TargetValue;
            FcCurVal09 = gasSet.CurrentValue;
            break;
          case "M10":
            FcSetVal10 = gasSet.TargetValue;
            FcCurVal10 = gasSet.CurrentValue;
            break;
          case "M11":
            FcSetVal11 = gasSet.TargetValue;
            FcCurVal11 = gasSet.CurrentValue;
            break;
          case "M12":
            FcSetVal12 = gasSet.TargetValue;
            FcCurVal12 = gasSet.CurrentValue;
            break;
          case "M13":
            FcSetVal13 = gasSet.TargetValue;
            FcCurVal13 = gasSet.CurrentValue;
            break;
          case "M14":
            FcSetVal14 = gasSet.TargetValue;
            FcCurVal14 = gasSet.CurrentValue;
            break;
          case "M15":
            FcSetVal15 = gasSet.TargetValue;
            FcCurVal15 = gasSet.CurrentValue;
            break;
          case "M16":
            FcSetVal16 = gasSet.TargetValue;
            FcCurVal16 = gasSet.CurrentValue;
            break;
          case "M17":
            FcSetVal17 = gasSet.TargetValue;
            FcCurVal17 = gasSet.CurrentValue;
            break;
          case "M18":
            FcSetVal18 = gasSet.TargetValue;
            FcCurVal18 = gasSet.CurrentValue;
            break;
          case "M19":
            FcSetVal19 = gasSet.TargetValue;
            FcCurVal19 = gasSet.CurrentValue;
            break;
          case "E01":
            FcSetVal20 = gasSet.TargetValue;
            FcCurVal20 = gasSet.CurrentValue;
            break;
          case "E02":
            FcSetVal21 = gasSet.TargetValue;
            FcCurVal21 = gasSet.CurrentValue;
            break;
          case "E03":
            FcSetVal22 = gasSet.TargetValue;
            FcCurVal22 = gasSet.CurrentValue;
            break;
          case "E04":
            FcSetVal23 = gasSet.TargetValue;
            FcCurVal23 = gasSet.CurrentValue;
            break;
          case "E05":
            FcSetVal24 = gasSet.TargetValue;
            FcCurVal24 = gasSet.CurrentValue;
            break;
          case "E06":
            FcSetVal25 = gasSet.TargetValue;
            FcCurVal25 = gasSet.CurrentValue;
            break;
          case "E07":
            FcSetVal26 = gasSet.TargetValue;
            FcCurVal26 = gasSet.CurrentValue;
            break;
          default:
            break;
        }
      }

      GsH2Value += 20;
      GsN2Value += 22;
      GsNH3Value += 24;
      GsSiH4Value += 26;


      #region onpropertyChanged
      OnPropertyChanged(nameof(GsH2Value));
      OnPropertyChanged(nameof(GsN2Value));
      OnPropertyChanged(nameof(GsNH3Value));
      OnPropertyChanged(nameof(GsSiH4Value));

      OnPropertyChanged(nameof(FcSetVal01));
      OnPropertyChanged(nameof(FcSetVal02));
      OnPropertyChanged(nameof(FcSetVal03));
      OnPropertyChanged(nameof(FcSetVal04));
      OnPropertyChanged(nameof(FcSetVal05));
      OnPropertyChanged(nameof(FcSetVal06));
      OnPropertyChanged(nameof(FcSetVal07));
      OnPropertyChanged(nameof(FcSetVal08));
      OnPropertyChanged(nameof(FcSetVal09));
      OnPropertyChanged(nameof(FcSetVal10));
      OnPropertyChanged(nameof(FcSetVal11));
      OnPropertyChanged(nameof(FcSetVal12));
      OnPropertyChanged(nameof(FcSetVal13));
      OnPropertyChanged(nameof(FcSetVal14));
      OnPropertyChanged(nameof(FcSetVal15));
      OnPropertyChanged(nameof(FcSetVal16));
      OnPropertyChanged(nameof(FcSetVal17));
      OnPropertyChanged(nameof(FcSetVal18));
      OnPropertyChanged(nameof(FcSetVal19));
      OnPropertyChanged(nameof(FcSetVal20));
      OnPropertyChanged(nameof(FcSetVal21));
      OnPropertyChanged(nameof(FcSetVal22));
      OnPropertyChanged(nameof(FcSetVal23));
      OnPropertyChanged(nameof(FcSetVal24));
      OnPropertyChanged(nameof(FcSetVal25));
      OnPropertyChanged(nameof(FcSetVal26));

      OnPropertyChanged(nameof(FcCurVal01));
      OnPropertyChanged(nameof(FcCurVal02));
      OnPropertyChanged(nameof(FcCurVal03));
      OnPropertyChanged(nameof(FcCurVal04));
      OnPropertyChanged(nameof(FcCurVal05));
      OnPropertyChanged(nameof(FcCurVal06));
      OnPropertyChanged(nameof(FcCurVal07));
      OnPropertyChanged(nameof(FcCurVal08));
      OnPropertyChanged(nameof(FcCurVal09));
      OnPropertyChanged(nameof(FcCurVal10));
      OnPropertyChanged(nameof(FcCurVal11));
      OnPropertyChanged(nameof(FcCurVal12));
      OnPropertyChanged(nameof(FcCurVal13));
      OnPropertyChanged(nameof(FcCurVal14));
      OnPropertyChanged(nameof(FcCurVal15));
      OnPropertyChanged(nameof(FcCurVal16));
      OnPropertyChanged(nameof(FcCurVal17));
      OnPropertyChanged(nameof(FcCurVal18));
      OnPropertyChanged(nameof(FcCurVal19));
      OnPropertyChanged(nameof(FcCurVal20));
      OnPropertyChanged(nameof(FcCurVal21));
      OnPropertyChanged(nameof(FcCurVal22));
      OnPropertyChanged(nameof(FcCurVal23));
      OnPropertyChanged(nameof(FcCurVal24));
      OnPropertyChanged(nameof(FcCurVal25));
      OnPropertyChanged(nameof(FcCurVal26));
      #endregion
    }

    public HomeViewModel()
    {
      ValveStateUpdate();
    }

  }
}
