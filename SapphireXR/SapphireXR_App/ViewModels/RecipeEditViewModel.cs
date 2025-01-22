using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Bases;
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
                    RecipeService.PLCLoad(Recipes, 10);
                }
                else
                {
                    RecipeService.PLCLoad(Recipes, 0);
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
                    break;

                case nameof(RecipeFilePath):
                    RecipeSaveCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        private void RecipeOpen()
        {
            (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config);
            if(result == true)
            {
                RecipeFilePath = recipeFilePath!;
                Recipes = new RecipeObservableCollection(recipes!);
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

        public List<Recipe> newlyAddedForMarking = new List<Recipe>();
    }
}
