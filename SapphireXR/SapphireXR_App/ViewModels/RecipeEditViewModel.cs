using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeEditViewModel : ViewModelBase
    {
        public partial class RecipeInformationViewModel : ObservableObject, IDisposable
        {
            public RecipeInformationViewModel(RecipeObservableCollection recipeList)
            {
                recipes = recipeList;
                recipeList.CollectionChanged += recipeCollectionChanged;
                
            }

            private void recipeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
            {
                int totalTime = 0;
                foreach (Recipe recipe in recipes)
                {
                    totalTime += (recipe.rTime + recipe.hTime);
                }
                TotalRecipeTime = totalTime;
                TotalStepNumber = recipes.Count;
            }

            private void setCurrentRecipe(Recipe recipe)
            {
                
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        
                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    disposedValue = true;
                }
            }

            // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
            // ~RecipeInformationViewModel()
            // {
            //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            //     Dispose(disposing: false);
            // }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            [ObservableProperty]
            private int _totalRecipeTime;
            [ObservableProperty]
            private int _totalStepNumber;
            [ObservableProperty]
            private int _rampingRateTemp;
            [ObservableProperty]
            private int _rampingRatePress;
            [ObservableProperty]
            private int _totalFlowRate;

            IList<Recipe> recipes;
            private bool disposedValue = false;
        }

        private RecipeObservableCollection _recipes;
        public RecipeObservableCollection Recipes
        {
            get { return _recipes; }
            set 
            { 
                SetProperty(ref _recipes, value);
            }
        }

        public IRelayCommand RecipeNewCommand => new RelayCommand(() =>
        {
            Recipes = new RecipeObservableCollection();
            RecipeFilePath = null;
        });
        public IRelayCommand RecipeOpenCommand => new RelayCommand(RecipeOpen);
        public IRelayCommand NavigateCommand => new RelayCommand<string?>(OnNavigate);
        public IRelayCommand RecipePLCLoadCommand { get; set; }
        public IRelayCommand RecipeSaveCommand { get; set; }
        public IRelayCommand RecipeSaveAsCommand { get; set; }

        private bool initialPLCLoadCommand = true;
        private string? _recipeFilePath = null;
        public string? RecipeFilePath
        {
            get { return _recipeFilePath; }
            set {  SetProperty(ref _recipeFilePath, value); }
        }

        [ObservableProperty]
        private Visibility _showCopyMenu;
        [ObservableProperty]
        private Visibility _showPasteMenu;
        [ObservableProperty]
        private bool _controlUIEnabled = false;

        public TabDataGridViewModel ReactorDataGridContext { get; set; }
        public TabDataGridViewModel FlowDataGridContext { get; set; }
        public TabDataGridViewModel ValveDataGridContext { get; set; }

        // 네비게이션 소스
        private string? _navigationSource;
        public string? NavigationSource
        {
            get { return _navigationSource; }
            set { SetProperty(ref _navigationSource, value); }
        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

#pragma warning disable CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        public RecipeEditViewModel()
#pragma warning restore CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        {
            //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";
            ShowCopyMenu = Visibility.Hidden;
            ShowPasteMenu = Visibility.Hidden;

            ReactorDataGridContext = new TabDataGridViewModel(this);
            FlowDataGridContext = new TabDataGridViewModel(this);
            ValveDataGridContext = new TabDataGridViewModel(this);

            RecipePLCLoadCommand = new RelayCommand(() =>
            {
                if(initialPLCLoadCommand == false)
                {
                    PLCService.WriteRCPOperationCommand(10);
                    RecipeService.PLCLoad(Recipes);
                }
                else
                {
                    PLCService.WriteRCPOperationCommand(0);
                    RecipeService.PLCLoad(Recipes);
                    initialPLCLoadCommand = false;
                }
            },
             () => Recipes != null && 0 < Recipes.Count
             );
            var recipeSave = (string filePath) =>
            {
                if (Recipes != null)
                {
                    using (StreamWriter streamWriter = new StreamWriter(filePath))
                    {
                        using (CsvWriter csvWriter = new CsvWriter(streamWriter, Config))
                        {
                            csvWriter.WriteRecords<Recipe>(Recipes);
                            MessageBox.Show(filePath + "로의 저장이 완료되었습니다.");
                        }
                    }
                }
            };
            RecipeSaveCommand = new RelayCommand(() =>
            {
                if (RecipeFilePath != null)
                {
                    recipeSave(RecipeFilePath);
                }
            },
            () => RecipeFilePath != null);
            RecipeSaveAsCommand = new RelayCommand(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "csv 파일(*.csv)|*.csv";
                RecipeFilePath = saveFileDialog.FileName = DateTime.Today.ToString("yyyyMMdd_");
                saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 25) + "Data\\Recipes\\";
                if (saveFileDialog.ShowDialog() == true)
                {
                    recipeSave(saveFileDialog.FileName);
                }
            },
            () => Recipes != null
            );

            //네비게이션 메시지 수신 등록
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);

            PropertyChanged += RecipeViewModel_PropertyChanged;
            Recipes = new RecipeObservableCollection();
        }

        private void RecipeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Recipes):
                    RecipeOpenCommand.NotifyCanExecuteChanged();
                    RecipePLCLoadCommand.NotifyCanExecuteChanged();
                    RecipeSaveAsCommand.NotifyCanExecuteChanged();
                    cleanupNewlyAdded();
                    ReactorDataGridContext.reset();
                    FlowDataGridContext.reset();
                    ValveDataGridContext.reset();
                    initialPLCLoadCommand = true;
                    Recipes.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) =>
                    {
                        RecipePLCLoadCommand.NotifyCanExecuteChanged();
                    };
                    recipeStateUpdater?.clean();
                    recipeStateUpdater = new RecipeStateUpader();
                    ControlUIEnabled = false;
                    
                    break;

                case nameof(RecipeFilePath):
                    RecipeSaveCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        private void RecipeOpen()
        {
            try
            {
                (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config);
                if (result == true)
                {
                    RecipeFilePath = recipeFilePath!;
                    Recipes = new RecipeObservableCollection(recipes!);
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show("Recipe를 로드하는데 실패하였습니다. 원인은 다음과 같습니다.\r\n" + exception.Message);
            }
        }

        private void OnNavigationMessage(object recipient, NavigationMessage message)
        {
            NavigationSource = message.Value;
        }

        private void OnNavigate(string? pageUri)
        {
            NavigationSource = pageUri;
        }

        private void cleanupNewlyAdded()
        {
            foreach (var recipe in newlyAddedForMarking)
            {
                recipe.Background = Brushes.White;
            }
            newlyAddedForMarking.Clear();
        }

        public void setSelectedRecipeStep(Recipe recipe)
        {
            ControlUIEnabled = true;
            recipeStateUpdater?.setSelectedRecipeStep(recipe);
        }

        public List<Recipe> newlyAddedForMarking = new List<Recipe>();
        private RecipeStateUpader? recipeStateUpdater;

        [ObservableProperty]
        private RecipeInformationViewModel _currentRecipeInformationViewModel;
    }
}
