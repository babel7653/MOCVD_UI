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
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.ViewModels
{
  public class FlowControlDialogViewModel : ViewModelBase
  {


    public int FsTarVal { get; set; } = 0;
    public int FsRampTime { get; set; } = 0;
    public int FsDeviation { get; set; } = 0;
    public int FsCurVal { get; set; } = 0;
    public int FsSetVal { get; set; } = 0;
    public int FsMaxVal { get; set; } = 0;

    public int TbValveNum { get; set; }
    public string TbValveState { get; set; }

    //public ICommand TestWindowShowCommand => new RelayCommand(BtnTestWindowShow);
    public ICommand FlowSettingOkCommand => new RelayCommand<object>(FlowSettingOk);
    public ICommand FlowSettingCancelCommand => new RelayCommand<object>(FlowSettingCancel);


    public FlowControlDialogViewModel(IServiceProvider service)
    {

    }


    private void FlowSettingOk(object win)
    {
      Window window = (Window)win;
      //TbValveState = parameter as string;
      //OnPropertyChanged(nameof(TbValveState));
      //MessageBoxResult re = MessageBox.Show($"MFC {TbValveNum}번을 " + TbValveState + " 으로 설정했습니다.", "MFC1", MessageBoxButton.OKCancel);
      //if (re == MessageBoxResult.OK)
      //{
      //  if (parameter is System.Windows.Window)
      //  {
      //    CurrWinClose(parameter);
      //  }

      //}
      MessageBox.Show("밸브 1번을 열었습니다.");

      window.Close();

    }

    private void FlowSettingCancel(object win)
    {
      Window window = (Window)win;

      MessageBox.Show("밸브 1번을 닫았습니다.");
      window.Close();
    }

  }
}
