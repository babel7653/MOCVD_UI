using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using OxyPlot;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Specialized;
using System.Text;

namespace SapphireXR_App.ViewModels
{
  public partial class RecipeViewModel : ViewModelBase
  {
    //private IList<Recipe> _recipes;
    //public IList<Recipe> Recipes
    //{
    //  get { return _recipes; }
    //  set { SetProperty(ref _recipes, value); }
    //}
    //public bool RecipeStart { get; set; }
    //public short RecipeOperationState { get; set; }

    //public IRelayCommand RecipeOpenCommand { get; set; }

    //public RecipeViewModel()
    //{
    //  Init();
    //}
    //private void RecipeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  switch (e.PropertyName)
    //  {
    //    case nameof(Recipes):
    //      RecipeOpenCommand.NotifyCanExecuteChanged();
    //      break;
    //  }
    //}

    //private void RecipeOpen()
    //{
    //  OpenFileDialog openFile = new OpenFileDialog();
    //  openFile.Multiselect = false;
    //  openFile.Filter = "csv 파일(*.csv)|*.csv";
    //  string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
    //  int path_length = appBasePath.Length;
    //  openFile.InitialDirectory = appBasePath.Substring(0, path_length - 25) + "Data\\Recipes\\";

    //  if (openFile.ShowDialog() != true) return;
    //  string filepath = openFile.FileName;

    //  var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
    //  {
    //    Delimiter = ",",
    //    HasHeaderRecord = true
    //  };

    //  using (StreamReader streamReader = new StreamReader(filepath))
    //  {
    //    using (var csvReader = new CsvReader(streamReader, config))
    //    {

    //      Recipes = csvReader.GetRecords<Recipe>().ToList();
    //      //dtgRecipes.ItemsSource = Recipes;
    //      short iRcpTotalStep = (short)Recipes.Count;
    //      PlcRecipe[] aRecipePLC = new PlcRecipe[iRcpTotalStep];
    //      int i = 0;
    //      foreach (Recipe iRecipeRow in Recipes)
    //      {
    //        PlcRecipe RecipeRow = RecipeDataConverter(iRecipeRow);
    //        aRecipePLC[i] = RecipeRow;
    //        i += 1;
    //      };
    //      RecipeStart = false;
    //      RecipeOperationState = 4;
    //      try
    //      {
    //        uint hRcp = MainViewModel.Ads.CreateVariableHandle("RCP.aRecipe");
    //        uint hRcpTotalStep = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpTotalStep");
    //        uint hRcpStart = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpStart");
    //        uint hRcpState = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");


    //        MainViewModel.Ads.WriteAny(hRcp, aRecipePLC);
    //        MainViewModel.Ads.WriteAny(hRcpTotalStep, iRcpTotalStep);
    //        MainViewModel.Ads.WriteAny(hRcpStart, RecipeStart);
    //        MainViewModel.Ads.WriteAny(hRcpState, RecipeOperationState);

    //        MainViewModel.Ads.DeleteVariableHandle(hRcp);
    //        MainViewModel.Ads.DeleteVariableHandle(hRcpTotalStep);
    //        MainViewModel.Ads.DeleteVariableHandle(hRcpStart);
    //        MainViewModel.Ads.DeleteVariableHandle(hRcpState);
    //      }

    //      catch (Exception err)
    //      {
    //        Console.WriteLine(err.Message);
    //      }
    //    }
    //    //OnPropertyChanged(nameof(Recipes)); //추가 검토
    //    // Recipe Initial State
    //  }
    //}
    //private PlcRecipe RecipeDataConverter(Recipe iRecipeRow)
    //{
    //  PlcRecipe PlcRecipeRow = new PlcRecipe();
    //  //Short Type Array
    //  PlcRecipeRow.aRecipeShort[0] = iRecipeRow.No;
    //  PlcRecipeRow.aRecipeShort[1] = iRecipeRow.rTime;
    //  PlcRecipeRow.aRecipeShort[2] = iRecipeRow.hTime;
    //  PlcRecipeRow.aRecipeShort[3] = iRecipeRow.sTemp;
    //  PlcRecipeRow.aRecipeShort[4] = iRecipeRow.rPress;
    //  PlcRecipeRow.aRecipeShort[5] = iRecipeRow.sRotation;
    //  PlcRecipeRow.aRecipeShort[6] = iRecipeRow.cTemp;
    //  PlcRecipeRow.aRecipeShort[7] = iRecipeRow.Loop;
    //  PlcRecipeRow.aRecipeShort[8] = iRecipeRow.Jump;
    //  //Float Type Array
    //  PlcRecipeRow.aRecipeFloat[0] = iRecipeRow.M01;
    //  PlcRecipeRow.aRecipeFloat[1] = iRecipeRow.M02;
    //  PlcRecipeRow.aRecipeFloat[2] = iRecipeRow.M03;
    //  PlcRecipeRow.aRecipeFloat[3] = iRecipeRow.M04;
    //  PlcRecipeRow.aRecipeFloat[4] = iRecipeRow.M05;
    //  PlcRecipeRow.aRecipeFloat[5] = iRecipeRow.M06;
    //  PlcRecipeRow.aRecipeFloat[6] = iRecipeRow.M07;
    //  PlcRecipeRow.aRecipeFloat[7] = iRecipeRow.M08;
    //  PlcRecipeRow.aRecipeFloat[8] = iRecipeRow.M09;
    //  PlcRecipeRow.aRecipeFloat[9] = iRecipeRow.M10;
    //  PlcRecipeRow.aRecipeFloat[10] = iRecipeRow.M11;
    //  PlcRecipeRow.aRecipeFloat[11] = iRecipeRow.M12;
    //  PlcRecipeRow.aRecipeFloat[12] = iRecipeRow.M13;
    //  PlcRecipeRow.aRecipeFloat[13] = iRecipeRow.M14;
    //  PlcRecipeRow.aRecipeFloat[14] = iRecipeRow.M15;
    //  PlcRecipeRow.aRecipeFloat[15] = iRecipeRow.M16;
    //  PlcRecipeRow.aRecipeFloat[16] = iRecipeRow.M17;
    //  PlcRecipeRow.aRecipeFloat[17] = iRecipeRow.M18;
    //  PlcRecipeRow.aRecipeFloat[18] = iRecipeRow.M19;
    //  PlcRecipeRow.aRecipeFloat[19] = iRecipeRow.E01;
    //  PlcRecipeRow.aRecipeFloat[20] = iRecipeRow.E02;
    //  PlcRecipeRow.aRecipeFloat[21] = iRecipeRow.E03;
    //  PlcRecipeRow.aRecipeFloat[22] = iRecipeRow.E04;
    //  PlcRecipeRow.aRecipeFloat[23] = iRecipeRow.E05;
    //  PlcRecipeRow.aRecipeFloat[24] = iRecipeRow.E06;
    //  PlcRecipeRow.aRecipeFloat[25] = iRecipeRow.E07;
    //  //BitArray from Valve Data
    //  BitArray aRecipeBit = new BitArray(32);
    //  aRecipeBit[0] = iRecipeRow.V01 ? true : false;
    //  aRecipeBit[1] = iRecipeRow.V02 ? true : false;
    //  aRecipeBit[2] = iRecipeRow.V03 ? true : false;
    //  aRecipeBit[3] = iRecipeRow.V04 ? true : false;
    //  aRecipeBit[4] = iRecipeRow.V05 ? true : false;
    //  aRecipeBit[5] = iRecipeRow.V07 ? true : false;
    //  aRecipeBit[6] = iRecipeRow.V08 ? true : false;
    //  aRecipeBit[7] = iRecipeRow.V10 ? true : false;
    //  aRecipeBit[8] = iRecipeRow.V11 ? true : false;
    //  aRecipeBit[9] = iRecipeRow.V13 ? true : false;
    //  aRecipeBit[10] = iRecipeRow.V14 ? true : false;
    //  aRecipeBit[11] = iRecipeRow.V16 ? true : false;
    //  aRecipeBit[12] = iRecipeRow.V17 ? true : false;
    //  aRecipeBit[13] = iRecipeRow.V19 ? true : false;
    //  aRecipeBit[14] = iRecipeRow.V20 ? true : false;
    //  aRecipeBit[15] = iRecipeRow.V22 ? true : false;
    //  aRecipeBit[16] = iRecipeRow.V23 ? true : false;
    //  aRecipeBit[17] = iRecipeRow.V24 ? true : false;
    //  aRecipeBit[18] = iRecipeRow.V25 ? true : false;
    //  aRecipeBit[19] = iRecipeRow.V26 ? true : false;
    //  aRecipeBit[20] = iRecipeRow.V27 ? true : false;
    //  aRecipeBit[21] = iRecipeRow.V28 ? true : false;
    //  aRecipeBit[22] = iRecipeRow.V29 ? true : false;
    //  aRecipeBit[23] = iRecipeRow.V30 ? true : false;
    //  aRecipeBit[24] = iRecipeRow.V31 ? true : false;
    //  aRecipeBit[25] = iRecipeRow.V32 ? true : false;

    //  PlcRecipeRow.sName = iRecipeRow.Name;

    //  if (aRecipeBit.Length > 32)
    //    throw new ArgumentException("Argument length shall be at most 32 bits.");
    //  int[] aValve = new int[1];
    //  aRecipeBit.CopyTo(aValve, 0);
    //  PlcRecipeRow.iValve = aValve[0];

    //  return PlcRecipeRow;
    //}

    //private void Init()
    //{

    //  RecipeOpenCommand = new RelayCommand(RecipeOpen);

    //  PropertyChanged += RecipeViewModel_PropertyChanged;
    //}

    public RecipeViewModel()
    {
      ValveStateUpdate();

    }

    public static string RecipeCsvFile = "";

    public int CurrRcpNum { get; set; } = 0;
    public List<GasAIO> GasSets { get; set; }


    // TcRecipes (for debugging) : plc recipe data to be loaded from csv file
    public List<Recipe>? Recipes { get; set; }
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
    public static RecipePlc[]? RecipeArr;


    // RelayCommand
    public ICommand RcFileNewCommand => new RelayCommand(FileNew);
    public ICommand RcFileOpenCommand => new RelayCommand(FileOpen);
    public ICommand RcFileSaveCommand => new RelayCommand(FileSave);
    public ICommand RcFileSaveasCommand => new RelayCommand(FileSaveas);
    public ICommand RcFileRefreshCommand => new RelayCommand(FileRefresh);
    public ICommand RecipeStartCommand => new RelayCommand(RecipeStart);
    public ICommand RecipePauseCommand => new RelayCommand(RecipePause);
    public ICommand RecipeRestartCommand => new RelayCommand(RecipeRestart);
    public ICommand RecipeSkipCommand => new RelayCommand(RecipeSkip);



    #region control event
    private void FileNew()
    {
      SaveFileDialog saveFile = new()
      {
        Filter = "모든파일(*.*)|*.*",
        Title = "New File"
      };

      if (saveFile.ShowDialog() == true)
      {
        string filepath = saveFile.FileName;
        string[] pathArray = filepath.Split('\\');
        string filename = pathArray[pathArray.Count() - 1];
        MessageBox.Show(filename, $"Create new file");

      }


    }

    // Open Recipe file
    private void FileOpen()
    {
      OpenFileDialog openFile = new()
      {
        Multiselect = false,
        Filter = "csv 파일(*.csv)|*.csv"
      };
      string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
      int path_length = appBasePath.Length;
      openFile.InitialDirectory = appBasePath.Substring(0, path_length - 25) + "data\\recipe\\";

      if (openFile.ShowDialog() != true) return;
      RecipeCsvFile = openFile.FileName;

      var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
      {
        Delimiter = ",",
        HasHeaderRecord = true
      };

      using (StreamReader streamReader = new(RecipeCsvFile))
      {
        using var csvReader = new CsvReader(streamReader, config);

        Recipes = csvReader.GetRecords<Recipe>().ToList();
        int Count = Recipes.Count;

        RecipePlc[] aRecipePLC = new RecipePlc[Count];
        int i = 0;
        foreach (Recipe rRecipe in Recipes)
        {
          RecipePlc RecipeRow = RecipeConvert(rRecipe);
          aRecipePLC[i] = RecipeRow;
          i += 1;
        };
        try
        {
          WritePc2Plc();
          uint hRcp = MainViewModel.Ads.CreateVariableHandle("RCP.aRecipe");
          uint hRcp1 = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpTotalStep");
          MainViewModel.Ads.WriteAny(hRcp, aRecipePLC);
          MainViewModel.Ads.WriteAny(hRcp1, (short)Count);
          MainViewModel.Ads.DeleteVariableHandle(hRcp);
          MainViewModel.Ads.DeleteVariableHandle(hRcp1);
        }

        catch (Exception err)
        {
          Console.WriteLine(err.Message);
        }
      }

      OnPropertyChanged(nameof(Recipes));
    }


    private void FileSave()
    {
      SaveFileDialog saveFile = new()
      {
        Filter = "Csv file|*.csv"
      };

      if (saveFile.ShowDialog() != true) return;
      string filepath = saveFile.FileName;
      MessageBox.Show(filepath, "Save file");
    }

    private void FileSaveas()
    {
      SaveFileDialog saveAsFile = new()
      {
        Filter = "Csv file|*.csv",
        Title = "Save an Excel File"
      };

      if (saveAsFile.ShowDialog() != true) return;
      string filepath = saveAsFile.FileName;
      MessageBox.Show(filepath, "Save as file");
    }

    private void FileRefresh()
    {
      ReadPlcVariables();
    }

    private void RecipeStart()
    {
      if (Recipes == null)
      {
        MessageBox.Show("Recipe 파일을 로드하세요.");
        return;
      }

      InitPlcRecipe();
      RunPlcRecipe();


      BottomViewModel.ChartStartSensing();

    }



    private void RecipePause()
    {
      PausePlcRecipe();
      BottomViewModel.ChartPauseSensing();

    }

    private void RecipeRestart()
    {
      RunPlcRecipe();
      BottomViewModel.ChartStartSensing();
    }

    private void RecipeSkip()
    {
      // test : Recipe variable reading 
      ReadRcpCurrNum();

    }

    #endregion



    /// <summary>
		/// 테스트용
    /// Datagrid에 이벤트 데이터 전달
    /// 이전 log 파일에서 log 데이터 임의 추출하여 보여주는 함수.
    /// </summary>


    private void SaveCsvDataLog()
    {

      string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
      int path_length = appBasePath.Length;
      string initFilepath = appBasePath.Substring(0, path_length - 25) + "data\\recipe\\";
      string dataLogFile = "data_log.csv";

      var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
      {
        Delimiter = ",",
        HasHeaderRecord = true
      };
    }



    /// <summary>
    /// 경로를 검증한다.
    /// true : 옳바른 경로이다. false : 부적절한 경로이다.
    /// </summary>
    private bool ValidateCsvFilePath(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
      {
        return false;
      }
      return true;
    }

    public List<int> tbInt { get; set; }
    public List<float> tbReal { get; set; }
    public List<bool> tbBool { get; set; }

    // Load all Recipes to PLC

    private int ReadRcpCurrNum()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("P50_RecipeControl.RcpIndex");
        int selectedItem = (int)MainViewModel.Ads.ReadAny(hVar, typeof(int));
        MainViewModel.Ads.DeleteVariableHandle(hVar);

        return selectedItem - 1;
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
      return 0;

    }

    // Run PLC Recipe process
    public void RunPlcRecipe()
    {
      try
      {
        byte var = 10;
        uint hVar = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");
        MainViewModel.Ads.WriteAny(hVar, var);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }

    // Pause PLC Recipe process
    public void PausePlcRecipe()
    {
      try
      {
        byte var = 20;
        uint hVar = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");
        MainViewModel.Ads.WriteAny(hVar, var);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }


    /// <summary>
    /// Write control command to PLC
    /// </summary>
    public void WritePc2Plc()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("Test_Recipe_Run.bPc2Plc");
        MainViewModel.Ads.WriteAny(hVar, true);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }

    public void InitPlcRecipe()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpInit");
        uint hVar1 = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpStart");
        MainViewModel.Ads.WriteAny(hVar, true);
        MainViewModel.Ads.WriteAny(hVar1, true);
        MainViewModel.Ads.DeleteVariableHandle(hVar);
        MainViewModel.Ads.DeleteVariableHandle(hVar1);
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }


    // Read Recipe from PLC
    public void ReadPlcVariables()
    {
      try
      {
        uint hVar = MainViewModel.Ads.CreateVariableHandle("GVL_IO.stDeviceIO");
        DevicePlc plcDeviceIO = (DevicePlc)MainViewModel.Ads.ReadAny(hVar, typeof(DevicePlc));

        Console.WriteLine($"plcDeviceIO.aDigitalInputIO[1] = {plcDeviceIO.aDigitalInputIO[1]}");
        Console.WriteLine($"plcDeviceIO.aDigitalOutputIO[1] = {plcDeviceIO.aDigitalOutputIO[1]}");
        Console.WriteLine($"plcDeviceIO.aAnalogInputIO[1]  = {plcDeviceIO.aAnalogInputIO[1]}");
        Console.WriteLine($"plcDeviceIO.aAnalogOutputIO[1] = {plcDeviceIO.aAnalogOutputIO[1]}");
        Console.WriteLine($"plcDeviceIO.aAnalogInputIO2[1] = {plcDeviceIO.aAnalogInputIO2[1]}");
        Console.WriteLine($"plcDeviceIO.aOutputSolValve[1] = {plcDeviceIO.aOutputSolValve[1]}");
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }

    private RecipePlc RecipeConvert(Recipe rRecipe)
    {
      RecipePlc rRecipePLC = new();
      //Short Type Array
      rRecipePLC.sName = rRecipe.Name;
      rRecipePLC.aRecipeShort[0] = rRecipe.No;
      rRecipePLC.aRecipeShort[1] = rRecipe.rTime;
      rRecipePLC.aRecipeShort[2] = rRecipe.hTime;
      rRecipePLC.aRecipeShort[3] = rRecipe.sTemp;
      rRecipePLC.aRecipeShort[4] = rRecipe.rPress;
      rRecipePLC.aRecipeShort[5] = rRecipe.sRotation;
      rRecipePLC.aRecipeShort[6] = rRecipe.cTemp;
      rRecipePLC.aRecipeShort[7] = rRecipe.Loop;
      rRecipePLC.aRecipeShort[8] = rRecipe.Jump;
      //Float Type Array
      rRecipePLC.aRecipeFloat[0] = rRecipe.M01;
      rRecipePLC.aRecipeFloat[1] = rRecipe.M02;
      rRecipePLC.aRecipeFloat[2] = rRecipe.M03;
      rRecipePLC.aRecipeFloat[3] = rRecipe.M04;
      rRecipePLC.aRecipeFloat[4] = rRecipe.M05;
      rRecipePLC.aRecipeFloat[5] = rRecipe.M06;
      rRecipePLC.aRecipeFloat[6] = rRecipe.M07;
      rRecipePLC.aRecipeFloat[7] = rRecipe.M08;
      rRecipePLC.aRecipeFloat[8] = rRecipe.M09;
      rRecipePLC.aRecipeFloat[9] = rRecipe.M10;
      rRecipePLC.aRecipeFloat[10] = rRecipe.M11;
      rRecipePLC.aRecipeFloat[11] = rRecipe.M12;
      rRecipePLC.aRecipeFloat[12] = rRecipe.M13;
      rRecipePLC.aRecipeFloat[13] = rRecipe.M14;
      rRecipePLC.aRecipeFloat[14] = rRecipe.M15;
      rRecipePLC.aRecipeFloat[15] = rRecipe.M16;
      rRecipePLC.aRecipeFloat[16] = rRecipe.M17;
      rRecipePLC.aRecipeFloat[17] = rRecipe.M18;
      rRecipePLC.aRecipeFloat[18] = rRecipe.M19;
      rRecipePLC.aRecipeFloat[19] = rRecipe.E01;
      rRecipePLC.aRecipeFloat[20] = rRecipe.E02;
      rRecipePLC.aRecipeFloat[21] = rRecipe.E03;
      rRecipePLC.aRecipeFloat[22] = rRecipe.E04;
      rRecipePLC.aRecipeFloat[23] = rRecipe.E05;
      rRecipePLC.aRecipeFloat[24] = rRecipe.E06;
      rRecipePLC.aRecipeFloat[25] = rRecipe.E07;
      //BitArray from Valve Data
      BitArray aRecipeBit = new(32);
      aRecipeBit[0] = rRecipe.V01;
      aRecipeBit[1] = rRecipe.V02;
      aRecipeBit[2] = rRecipe.V03;
      aRecipeBit[3] = rRecipe.V04;
      aRecipeBit[4] = rRecipe.V05;
      aRecipeBit[5] = rRecipe.V07;
      aRecipeBit[6] = rRecipe.V08;
      aRecipeBit[7] = rRecipe.V10;
      aRecipeBit[8] = rRecipe.V11;
      aRecipeBit[9] = rRecipe.V13;
      aRecipeBit[10] = rRecipe.V14;
      aRecipeBit[11] = rRecipe.V16;
      aRecipeBit[12] = rRecipe.V17;
      aRecipeBit[13] = rRecipe.V19;
      aRecipeBit[14] = rRecipe.V20;
      aRecipeBit[15] = rRecipe.V22;
      aRecipeBit[16] = rRecipe.V23;
      aRecipeBit[17] = rRecipe.V24;
      aRecipeBit[18] = rRecipe.V25;
      aRecipeBit[19] = rRecipe.V26;
      aRecipeBit[20] = rRecipe.V27;
      aRecipeBit[21] = rRecipe.V28;
      aRecipeBit[22] = rRecipe.V29;
      aRecipeBit[23] = rRecipe.V30;
      aRecipeBit[24] = rRecipe.V31;
      aRecipeBit[25] = rRecipe.V32;

      if (aRecipeBit.Length > 32)
        throw new ArgumentException("Argument length shall be at most 32 bits.");
      int[] aValve = new int[1];
      aRecipeBit.CopyTo(aValve, 0);
      rRecipePLC.iValve = aValve[0];

      return rRecipePLC;
    }

    public static DispatcherTimer? _timer;
    public FlowConData FlowConData { get; set; }

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

      #region onpropertyChanged
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

  }
}
