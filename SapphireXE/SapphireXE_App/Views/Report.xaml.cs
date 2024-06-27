using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using CsvHelper;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SapphireXE_App.Models;
using SapphireXE_App.ViewModels;
using SkiaSharp;

namespace SapphireXE_App.Views
{
  public partial class Report : Page
  {
    public Report()
    {
      InitializeComponent();
      DataContext = new ReportViewModel();
    }


    private void PlotLogFileOpen_1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ReportViewModel RVM = new();

      RVM.CsvData();
    }

  }
}
