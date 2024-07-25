using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using SapphireXE_App.Models;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Input;
using SapphireXE_App.Commands;
using TwinCAT.Ads;

namespace SapphireXE_App.ViewModels
{
  public class RecipeControlViewModel : ViewModelBase
  {
    private List<Recipe> recipes;
    public List<Recipe> Recipes
    {
      get { return recipes; }
      set { SetProperty(ref recipes, value); }
    }

    public RecipeControlViewModel()
    {
    }

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
      SaveFileDialog saveFile = new SaveFileDialog();
      saveFile.Filter = "모든파일(*.*)|*.*";
      saveFile.Title = "New File";

      if (saveFile.ShowDialog() == true)
      {
        string filepath = saveFile.FileName;
        string[] pathArray = filepath.Split('\\');
        string filename = pathArray[pathArray.Count() - 1];
        MessageBox.Show(filename, $"Create new file");

      }
    }

    private void FileOpen()
    {
      OpenFileDialog openFile = new();
      openFile.Multiselect = false;
      openFile.Filter = "모든파일(*.*)|*.*";

      if (openFile.ShowDialog() != true) return;
      string filepath = openFile.FileName;

      recipes = new();
      // load csv data and test 

      using StreamReader streamReader = new StreamReader(filepath);
      using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
      _ = csvReader.Read();
      while (csvReader.Read())
      {
        Recipe r = new()
        {
          No = csvReader.GetField<int>(0),
          Name = csvReader.GetField<string>(1),
          rTime = csvReader.GetField<int>(2),
          hTime = csvReader.GetField<int>(3),
          rTemp = csvReader.GetField<int>(4),
          rPressure = csvReader.GetField<int>(5),
          sRotation = csvReader.GetField<int>(6),
          cTemp = csvReader.GetField<int>(7),
          Loop = csvReader.GetField<int>(8),
          Jump = csvReader.GetField<int>(9),
          M01 = csvReader.GetField<int>(10),
          M02 = csvReader.GetField<int>(11),
          M03 = csvReader.GetField<int>(12),
          M04 = csvReader.GetField<int>(13),
        };
        recipes.Add(r);

        //var objResult = new Dictionary<string, string>();
        //for (int i = 0; i < 6; i++)
        //  objResult.Add(header[i], csvReader.GetField<string>(i));
        //listObjResult.Add(objResult);
      }
      OnPropertyChanged(nameof(Recipes));

      //return JsonConvert.SerializeObject(listObjResult);
    }

    private void FileSave()
    {
      SaveFileDialog saveFile = new SaveFileDialog();
      saveFile.Filter = "Csv file|*.csv";

      if (saveFile.ShowDialog() != true) return;
      string filepath = saveFile.FileName;
      MessageBox.Show(filepath, "Save file");
    }

    private void FileSaveas()
    {
      SaveFileDialog saveAsFile = new SaveFileDialog();
      saveAsFile.Filter = "Csv file|*.csv";
      saveAsFile.Title = "Save an Excel File";

      if (saveAsFile.ShowDialog() != true) return;
      string filepath = saveAsFile.FileName;
      MessageBox.Show(filepath, "Save as file");
    }

    private void FileRefresh()
    {
      MessageBox.Show("파일을 다시 로드합니다. 저장하지 않은 데이터는 저장되지 않습니다.", "File refresh");

    }

    private void RecipeStart()
    {
      MessageBox.Show("Recipe start");
    }

    private void RecipePause()
    {
      MessageBox.Show("Recipe pause");
    }

    private void RecipeRestart()
    {
      MessageBox.Show("Recipe restart");
    }

    private void RecipeSkip()
    {
      MessageBox.Show("Recipe skip");
    }



    #endregion


    public string CsvJson()
    {
      string[] header = { "RecipeStep", "RecipeName", "RampingTime", "HoldingTime", "RecipeLoop", "RecipeJump" };

      string path = "..\\..\\..\\data\\recipe\\Test240621_InGaN_MQW(5p).csv";
      var csv = new List<string[]>();
      var lines = File.ReadAllLines(path);

      foreach (string line in lines)
        csv.Add(line.Split(','));

      var listObjResult = new List<Dictionary<string, string>>();

      for (int i = 1; i < lines.Length; i++)
      {
        var objResult = new Dictionary<string, string>();
        for (int j = 0; j < header.Length; j++)
          objResult.Add(header[j], csv[i][j]);

        listObjResult.Add(objResult);
      }

      return JsonConvert.SerializeObject(listObjResult);
    }

  }
}
