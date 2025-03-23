using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SapphireXR_App.Models;
using System.Globalization;
using System.IO;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        public ReportViewModel() 
        {
            FlowControlLivePlot.Axes.Add(new LinearAxis
            {
                Title = "Data Value",
                Position = AxisPosition.Left,
                IsPanEnabled = true,
                IsZoomEnabled = true
            });
            FlowControlLivePlot.Axes.Add(new DateTimeAxis
            {
                Title = "Time Stamp",
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm:ss",
                IntervalLength = 60,
                IsPanEnabled = true,
                IsZoomEnabled = true,
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
            });
            FlowControlLivePlot.Legends.Add(new Legend() { Key= "CurrentTargetValue" });
        }

        [ObservableProperty]
        private string _log1FilePath = "";

        [ObservableProperty]
        private string _log2FilePath = "";

        private (IList<RecipeLog>?, string?) openLogFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "csv 파일(*.csv)|*.csv";
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == false)
            {
                return (null, null);
            }

            try
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    using (StreamReader streamReder = new StreamReader(fileStream))
                    {
                        using(CsvReader csvReader = new CsvReader(streamReder, Config))
                        {
                            return (csvReader.GetRecords<RecipeLog>().ToList(), openFileDialog.FileName);
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show(openFileDialog.FileName + "를 여는데 실패하였습니다. 올바른 로그 파일이 아닐 수 있습니다. 자세한 오류는 다음과 같습니다: " + exception.Message);
            }

            return (null, null);
        }

        [RelayCommand]
        public void OpenLog1File()
        {
        }

        [RelayCommand]
        public void OpenLog2File()
        {

        }

        [RelayCommand]
        public void ShowSeriesSelectionView()
        {
            if(reportSeriesSelectionViewModel == null)
            {
                reportSeriesSelectionViewModel = new ReportSeriesSelectionViewModel();
            }
            ReportSeriesSelectionEx.Show(reportSeriesSelectionViewModel);
        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

        public PlotModel FlowControlLivePlot { get; set; } = new PlotModel();
        private ReportSeriesSelectionViewModel? reportSeriesSelectionViewModel = null;
    }
}
