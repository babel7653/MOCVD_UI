using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows;
using SapphireXR_App.Views;
using Microsoft.Xaml.Behaviors;
using Caliburn.Micro;
using System.Reflection.Metadata;
using System.Windows.Data;
using System.Windows.Media.TextFormatting;
using System.Windows.Controls;

namespace SapphireXR_App.ViewModels
{
  public class AnalogDeviceControlViewModel : ViewModelBase
  {
    public int TbValveNum { get; set; }
    public string TbValveState { get; set; }

    //public ICommand TestWindowShowCommand => new RelayCommand(BtnTestWindowShow);
    public ICommand MfcOkCommand => new RelayCommand<object>(MfcOk);
    public ICommand MfcCancelCommand => new RelayCommand<object>(MfcCancel);


    public AnalogDeviceControlViewModel()
    {
      TbValveNum = HomeViewModel.MfcNo;
      OnPropertyChanged(nameof(TbValveNum));

    }


    private void MfcOk(object? parameter)
    {
      TbValveState = parameter as string;
      OnPropertyChanged(nameof(TbValveState));
      MessageBoxResult re = MessageBox.Show($"MFC {TbValveNum}번을 " + TbValveState + " 으로 설정했습니다.", "MFC1", MessageBoxButton.OKCancel);
      if (re == MessageBoxResult.OK)
      {
        if (parameter is System.Windows.Window)
        {
          //CurrWinClose(parameter);
        }

      }
    }

    private void MfcCancel(object? parameter)
    {
      MessageBox.Show("밸브 1번을 닫았습니다.");
      CurrWinClose(parameter);
    }

    private void CurrWinClose(object? param)
    {
      if (param is System.Windows.Window)
      {
        ((System.Windows.Window)param).Close();
      }
    }


  }
}
