using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;
using System.Collections;
using System.Globalization;
using System.IO;

namespace SapphireXR_App.ViewModels
{
    public class RecipeViewModel : ViewModelBase
    {
        private IList<Recipe>? _recipes;
        public IList<Recipe>? Recipes
        {
            get { return _recipes; }
            set { SetProperty(ref _recipes, value); }
        }
        public short nRecipeOperationState { get; set; }

        // Static Variable
        public static bool bRecipeStart { get; set; }

        public IRelayCommand RecipeOpenCommand { get; set; }
        public IRelayCommand RecipeStartCommand { get; set; }
        public IRelayCommand RecipeStopCommand { get; set; }
        public IRelayCommand NavigateCommand {  get; set; }

        // 네비게이션 소스
        private string? _navigationSource;
        public string? NavigationSource
        {
            get { return _navigationSource; }
            set { SetProperty(ref _navigationSource, value); }
        }

        public RecipeViewModel()
        {
            Init();
        }
        private void RecipeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Recipes):
                    RecipeOpenCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        private void RecipeOpen()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.Filter = "csv 파일(*.csv)|*.csv";
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            int path_length = appBasePath.Length;
            openFile.InitialDirectory = appBasePath.Substring(0, path_length - 25) + "Data\\Recipes\\";

            if (openFile.ShowDialog() != true) return;
            string filepath = openFile.FileName;

            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            using (StreamReader streamReader = new StreamReader(filepath))
            {
                using (var csvReader = new CsvReader(streamReader, config))
                {

                    Recipes = csvReader.GetRecords<Recipe>().ToList();
                    //dtgRecipes.ItemsSource = Recipes;
                    short iRcpTotalStep = (short)Recipes.Count;
                    PlcRecipe[] aRecipePLC = new PlcRecipe[iRcpTotalStep];
                    int i = 0;
                    foreach (Recipe iRecipeRow in Recipes)
                    {
                        PlcRecipe RecipeRow = RecipeDataConverter(iRecipeRow);
                        aRecipePLC[i] = RecipeRow;
                        i += 1;
                    };
                    bRecipeStart= false;
                    nRecipeOperationState = 0;
                    try
                    {
                        uint hRcp = MainViewModel.Ads.CreateVariableHandle("RCP.aRecipe");
                        uint hRcpTotalStep = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpTotalStep");
                        uint hRcpStart = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpStart");
                        uint hRcpState = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");


                        MainViewModel.Ads.WriteAny(hRcp, aRecipePLC);
                        MainViewModel.Ads.WriteAny(hRcpTotalStep, iRcpTotalStep);
                        MainViewModel.Ads.WriteAny(hRcpStart, bRecipeStart);
                        MainViewModel.Ads.WriteAny(hRcpState, nRecipeOperationState);

                        MainViewModel.Ads.DeleteVariableHandle(hRcp);
                        MainViewModel.Ads.DeleteVariableHandle(hRcpTotalStep);
                        MainViewModel.Ads.DeleteVariableHandle(hRcpStart);
                        MainViewModel.Ads.DeleteVariableHandle(hRcpState);
                    }

                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                }
                //OnPropertyChanged(nameof(Recipes)); //추가 검토
                // Recipe Initial State
            }
        }
        private void RecipeStart()
        {
            bRecipeStart = true;
            nRecipeOperationState = 10;
            try
            {
                uint hRcpStart = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpStart");
                uint hRcpState = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");

                MainViewModel.Ads.WriteAny(hRcpStart, bRecipeStart);
                MainViewModel.Ads.WriteAny(hRcpState, nRecipeOperationState);

                MainViewModel.Ads.DeleteVariableHandle(hRcpStart);
                MainViewModel.Ads.DeleteVariableHandle(hRcpState);
            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        private void RecipeStop()
        {
            bRecipeStart= false;
            nRecipeOperationState = 40;
            try
            {
                uint hRcpStart = MainViewModel.Ads.CreateVariableHandle("RCP.bRcpStart");
                uint hRcpState = MainViewModel.Ads.CreateVariableHandle("RCP.iRcpOperationState");

                MainViewModel.Ads.WriteAny(hRcpStart, bRecipeStart);
                MainViewModel.Ads.WriteAny(hRcpState, nRecipeOperationState);

                MainViewModel.Ads.DeleteVariableHandle(hRcpStart);
                MainViewModel.Ads.DeleteVariableHandle(hRcpState);
            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private PlcRecipe RecipeDataConverter(Recipe iRecipeRow)
        {
            PlcRecipe PlcRecipeRow = new PlcRecipe();
            //Short Type Array
            PlcRecipeRow.aRecipeShort[0] = iRecipeRow.No;
            PlcRecipeRow.aRecipeShort[1] = iRecipeRow.rTime;
            PlcRecipeRow.aRecipeShort[2] = iRecipeRow.hTime;
            PlcRecipeRow.aRecipeShort[3] = iRecipeRow.sTemp;
            PlcRecipeRow.aRecipeShort[4] = iRecipeRow.rPress;
            PlcRecipeRow.aRecipeShort[5] = iRecipeRow.sRotation;
            PlcRecipeRow.aRecipeShort[6] = iRecipeRow.cTemp;
            PlcRecipeRow.aRecipeShort[7] = iRecipeRow.Loop;
            PlcRecipeRow.aRecipeShort[8] = iRecipeRow.Jump;
            //Float Type Array
            PlcRecipeRow.aRecipeFloat[0] = iRecipeRow.M01;
            PlcRecipeRow.aRecipeFloat[1] = iRecipeRow.M02;
            PlcRecipeRow.aRecipeFloat[2] = iRecipeRow.M03;
            PlcRecipeRow.aRecipeFloat[3] = iRecipeRow.M04;
            PlcRecipeRow.aRecipeFloat[4] = iRecipeRow.M05;
            PlcRecipeRow.aRecipeFloat[5] = iRecipeRow.M06;
            PlcRecipeRow.aRecipeFloat[6] = iRecipeRow.M07;
            PlcRecipeRow.aRecipeFloat[7] = iRecipeRow.M08;
            PlcRecipeRow.aRecipeFloat[8] = iRecipeRow.M09;
            PlcRecipeRow.aRecipeFloat[9] = iRecipeRow.M10;
            PlcRecipeRow.aRecipeFloat[10] = iRecipeRow.M11;
            PlcRecipeRow.aRecipeFloat[11] = iRecipeRow.M12;
            PlcRecipeRow.aRecipeFloat[12] = iRecipeRow.M13;
            PlcRecipeRow.aRecipeFloat[13] = iRecipeRow.M14;
            PlcRecipeRow.aRecipeFloat[14] = iRecipeRow.M15;
            PlcRecipeRow.aRecipeFloat[15] = iRecipeRow.M16;
            PlcRecipeRow.aRecipeFloat[16] = iRecipeRow.M17;
            PlcRecipeRow.aRecipeFloat[17] = iRecipeRow.M18;
            PlcRecipeRow.aRecipeFloat[18] = iRecipeRow.M19;
            PlcRecipeRow.aRecipeFloat[19] = iRecipeRow.E01;
            PlcRecipeRow.aRecipeFloat[20] = iRecipeRow.E02;
            PlcRecipeRow.aRecipeFloat[21] = iRecipeRow.E03;
            PlcRecipeRow.aRecipeFloat[22] = iRecipeRow.E04;
            PlcRecipeRow.aRecipeFloat[23] = iRecipeRow.E05;
            PlcRecipeRow.aRecipeFloat[24] = iRecipeRow.E06;
            PlcRecipeRow.aRecipeFloat[25] = iRecipeRow.E07;
            //BitArray from Valve Data
            BitArray aRecipeBit = new BitArray(32);
            aRecipeBit[0] = iRecipeRow.V01 ? true : false;
            aRecipeBit[1] = iRecipeRow.V02 ? true : false;
            aRecipeBit[2] = iRecipeRow.V03 ? true : false;
            aRecipeBit[3] = iRecipeRow.V04 ? true : false;
            aRecipeBit[4] = iRecipeRow.V05 ? true : false;
            aRecipeBit[5] = iRecipeRow.V07 ? true : false;
            aRecipeBit[6] = iRecipeRow.V08 ? true : false;
            aRecipeBit[7] = iRecipeRow.V10 ? true : false;
            aRecipeBit[8] = iRecipeRow.V11 ? true : false;
            aRecipeBit[9] = iRecipeRow.V13 ? true : false;
            aRecipeBit[10] = iRecipeRow.V14 ? true : false;
            aRecipeBit[11] = iRecipeRow.V16 ? true : false;
            aRecipeBit[12] = iRecipeRow.V17 ? true : false;
            aRecipeBit[13] = iRecipeRow.V19 ? true : false;
            aRecipeBit[14] = iRecipeRow.V20 ? true : false;
            aRecipeBit[15] = iRecipeRow.V22 ? true : false;
            aRecipeBit[16] = iRecipeRow.V23 ? true : false;
            aRecipeBit[17] = iRecipeRow.V24 ? true : false;
            aRecipeBit[18] = iRecipeRow.V25 ? true : false;
            aRecipeBit[19] = iRecipeRow.V26 ? true : false;
            aRecipeBit[20] = iRecipeRow.V27 ? true : false;
            aRecipeBit[21] = iRecipeRow.V28 ? true : false;
            aRecipeBit[22] = iRecipeRow.V29 ? true : false;
            aRecipeBit[23] = iRecipeRow.V30 ? true : false;
            aRecipeBit[24] = iRecipeRow.V31 ? true : false;
            aRecipeBit[25] = iRecipeRow.V32 ? true : false;

            PlcRecipeRow.sName = iRecipeRow.Name;

            if (aRecipeBit.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");
            int[] aValve = new int[1];
            aRecipeBit.CopyTo(aValve, 0);
            PlcRecipeRow.iValve = aValve[0];

            return PlcRecipeRow;
        }

        private void Init()
        {
            //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";

            RecipeOpenCommand = new RelayCommand(RecipeOpen);
            RecipeStartCommand = new RelayCommand(RecipeStart);
            RecipeStopCommand = new RelayCommand(RecipeStop);

            NavigateCommand = new RelayCommand<string?>(OnNavigate);

            //네비게이션 메시지 수신 등록
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);

            PropertyChanged += RecipeViewModel_PropertyChanged;
        }

        private void OnNavigationMessage(object recipient, NavigationMessage message)
        {
            NavigationSource = message.Value;
        }

        private void OnNavigate(string pageUri)
        {
            NavigationSource = pageUri;
        }
    }
}
