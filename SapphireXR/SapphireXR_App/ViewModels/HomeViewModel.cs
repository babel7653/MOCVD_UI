using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
  public class HomeViewModel : ViewModelBase
  {
    private short nValveNo;
    public static short MfcNo; 

    //public ICommand TestWindowShowCommand => new RelayCommand(BtnTestWindowShow);
    public ICommand BtnMsgBoxShowCommand => new RelayCommand<string>(BtnMsgBoxShow);

    public void BtnMsgBoxShow(string ValveNo)
    {
      MessageBoxResult result = MessageBox.Show($"밸브{ValveNo}를 여시겠습니까?", $"밸브{ValveNo}", MessageBoxButton.OKCancel);
      if (result == MessageBoxResult.OK)
      {
        ValveOperation();
                

      }
      MfcNo = Convert.ToInt16(ValveNo);
    }


    public void ValveOperation()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("GVL_IO.aDigitalInputIO");
        MainViewModel.Ads.WriteAny(hVar, 1);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }


    public void BtnTestWindowShow()
    {
      AnalogDeviceControl TW = new();
      TW.Show();
    }

    public HomeViewModel()
    {
      Init();
    }

    private void Init()
    {
    }


  }
}
