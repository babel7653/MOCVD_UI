using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using TwinCAT.Ads;
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;
using LiveCharts;
using LiveCharts.Wpf;

namespace SapphireXE_App
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //Create an instance of the the TcAdsClient
        TcAdsClient tcClient;
        //Live Chart용 코드
        public SeriesCollection SeriesData { get; private set; }
        public string[] XLabel { get; set; }
        public Func<int, string> Values { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //Live Chart 테스트 코드
            SeriesData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "2020",
                    Values = new ChartValues<int>{3,5,7,4,7 }
                },
                new LineSeries
                {
                    Title ="2021",
                    Values = new ChartValues<int>{5,6,2,7,8}
                },
                new LineSeries
                {
                    Title = "2022",
                    Values = new ChartValues<int> { 8, 7, 6, 9, 7}
                }
            };
            XLabel = new string[] { "Kang", "Kim", "Cho", "Ko", "Song" };
            Values = value => value.ToString("N");

            DataContext = this;

            //래시피 DataGrid 데이터 내용
            List<RecipeDataRow> recipeDataRow = new List<RecipeDataRow>();
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 1, RecipeName = "Evacuation", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 2, RecipeName = "N2 Filling", RampingTime = 90, HoldingTime = 10, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 3, RecipeName = "Evacuation", RampingTime = 90, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 4, RecipeName = "H2 Flow Set", RampingTime = 30, HoldingTime = 10, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 5, RecipeName = "Temp Up Pre", RampingTime = 60, HoldingTime =10, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 6, RecipeName = "Temp Up to T Etching 1", RampingTime = 250, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 7, RecipeName = "Temp Up to T Etching 2", RampingTime = 60, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 8, RecipeName = "Thermal Etching", RampingTime = 1, HoldingTime = 300, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 9, RecipeName = "Temp Dowin to Buffer", RampingTime = 200, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 10, RecipeName = "Wait to Stable", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 11, RecipeName = "Thermal Etching", RampingTime = 1, HoldingTime = 300, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 12, RecipeName = "Temp Dowin to Buffer", RampingTime = 200, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
            recipeDataRow.Add(new RecipeDataRow { RecipeStep = 13, RecipeName = "Wait to Stable", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
            RecipeStepReactor.ItemsSource = recipeDataRow;
            RecipeStepMFC.ItemsSource = recipeDataRow;
            RecipeStepValve.ItemsSource = recipeDataRow;

        }
        // 래시피 DataGrid 데이터 클래스, struct data형식과 비교 검토예정
        public class RecipeDataRow
        {
            public int RecipeStep { get; set; }
            public string RecipeName { get; set; }
            public int RampingTime { get; set; } // 시간(초) int로
            public int HoldingTime { get; set; } // 시간(초) int로
            public bool RecipeLoop { get; set; } // Loop 데이터형태 변경
            public bool RecipeJump { get; set; } // Loop와 연관된 Jump형태
           
        }

        private void HydridCarrirerChange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("밸르를 열까요?", "밸브Open", MessageBoxButton.YesNoCancel);
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(e.VerticalChange != 0.0f)
            {
                ScrollViewer sv1 = null;
                ScrollViewer sv2 = null;
                try
                {
                    if (sender.Equals(RecipeStepReactor))
                    {
                        Type t = RecipeStepReactor.GetType();
                        sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepMFC, null) as ScrollViewer;
                        sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepValve, null) as ScrollViewer;

                    }
                    else if (sender.Equals(RecipeStepMFC))
                    {
                        Type t = RecipeStepMFC.GetType();
                        sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepReactor, null) as ScrollViewer;
                        sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepValve, null) as ScrollViewer;
                    }
                    else
                    {
                        Type t = RecipeStepValve.GetType();
                        sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepReactor, null) as ScrollViewer;
                        sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.GetProperty, null, RecipeStepMFC, null) as ScrollViewer;
                    }
                    sv1.ScrollToVerticalOffset(e.VerticalOffset);
                    sv2.ScrollToVerticalOffset(e.VerticalOffset);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
    }
}
