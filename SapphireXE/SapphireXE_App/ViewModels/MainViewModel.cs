using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SapphireXE_App.Commands;
using SapphireXE_App.Commons;
using SapphireXE_App.Models;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace SapphireXE_App.ViewModels
{
  public partial class MainViewModel : ViewModelBase
  {
    // UI binding properties
    public string plcAddress { get; set; } = "PLC Address : ";
    public string plcMode { get; set; } = "System Mode : ";

    // twincat plc variables setting
    AdsClient tcClient = new();
    AmsNetId amsNetId = new("10.10.10.10.1.1");
    public bool tcPlcState { get; set; }
    public byte tcDeviceArray { get; set; }

    public int tcAI3054 { get; set; }
    public byte tcDO2088 { get; set; }
    public int tcAO4024 { get; set; }

    uint hPlcState = 0;
    uint hDeviceArray = 0;

    uint hAI3054 = 0;
    uint hDO2088 = 0;
    uint hAO4024 = 0;


    MemoryStream dataStream = new(1);
    Random rand = new();


    public MainViewModel()
    {
      ConnectPLC();
      GetPlcDatetime();
      ReadWritePlcOpenTypes();

      AlarmSettingLoad();


      RecipeLiveData = new BindableCollection<RecipeControlData>();
      RecipeLiveData.CollectionChanged += RecipeLiveData_CollectionChanged;

      RecipeProgressData = new BindableCollection<RecipeControlData>();
      RecipeProgressData.CollectionChanged += RecipeProgressData_CollectionChanged;

    }

    private void ConnectPLC()
    {
      try
      {
        tcClient.Connect(AmsNetId.Local, 851);
        //tcClient.Connect(amsNetId, 851);
        if (tcClient.IsConnected)
        {
          hPlcState = tcClient.CreateVariableHandle("MAIN.plc_state");
          tcPlcState = (bool)tcClient.ReadAny(hPlcState, typeof(bool));
          plcAddress = $"PLC Address : {tcClient.Address}";
          plcMode = "System Mode : Ready";

          Console.WriteLine("Connected");

          // PLC initial test
          hDeviceArray = tcClient.CreateVariableHandle("MAIN.plc_array");
          tcDeviceArray = (byte)tcClient.ReadAny(hDeviceArray, typeof(byte));

          hDO2088 = tcClient.CreateVariableHandle("MAIN.el2088_out");
          tcDO2088 = (byte)tcClient.ReadAny(hDO2088, typeof(byte));
          hAO4024 = tcClient.CreateVariableHandle("MAIN.el4024_out");
          tcAO4024 = (int)tcClient.ReadAny(hAO4024, typeof(int));
          hAI3054 = tcClient.CreateVariableHandle("MAIN.el3054_in");
          tcAI3054 = (int)tcClient.ReadAny(hAI3054, typeof(int));
          Console.WriteLine($"tcDO2088={tcDO2088}, tcAO4024={tcAO4024}, tcAI3054={tcAI3054}");
        }
      }
      catch
      {
        MessageBox.Show("TwinCAT이 연결되지 않았습니다.");
        plcAddress = "PLC Address : ";
        plcMode = "System Mode : Not Connected";

      }
    }


    /// <summary>
    /// 경로를 검증한다.
    /// true : 옳바른 경로이다. false : 부적절한 경로이다.
    /// </summary>




  }
}
