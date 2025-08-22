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

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeEditViewModel : ViewModelBase
    {
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

            loadToRecipeRunPublisher = ObservableManager<(string, IList<Recipe>)>.Get("RecipeEdit.LoadToRecipeRun");
            switchTabToDataRunPublisher = ObservableManager<int>.Get("SwitchTab");

            RecipePLCLoadCommand = new RelayCommand(() =>
            {
                loadToRecipeRunPublisher.Publish((RecipeFilePath ?? "", new RecipeObservableCollection(Recipes.Select(recipe => new Recipe(recipe)))));
                switchTabToDataRunPublisher.Publish(1);
            },
            () => Recipes != null && 0 < Recipes.Count);
            var recipeSave = (string filePath) =>
            {
                if (Recipes != null)
                {
                    using (StreamWriter streamWriter = new StreamWriter(filePath))
                    {
                        using (CsvWriter csvWriter = new CsvWriter(streamWriter, Config))
                        {
                            try
                            {
                                csvWriter.WriteRecords<Recipe>(Recipes);
                                MessageBox.Show(filePath + "로의 저장이 완료되었습니다.");
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("Recipe를 저장하는데 실패하였습니다. 원인은 다음과 같습니다.\r\n" + exception.Message);
                            }
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
                saveFileDialog.InitialDirectory = AppSetting.RecipeEditRecipeInitialPath;
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
            PropertyChanging += (object? sender, PropertyChangingEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(CurrentRecipeInformationViewModel):
                        CurrentRecipeInformationViewModel?.dispose();
                        break;
                }
            };
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
                    Recipes.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) =>
                    {
                        RecipePLCLoadCommand.NotifyCanExecuteChanged();
                        Recipes.RefreshNo();
                    };
                    recipeStateUpdater?.clean();
                    recipeStateUpdater = null;
                    ControlUIEnabled = false;
                    CurrentRecipeInformationViewModel = new RecipeInformationViewModel(Recipes);
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
                (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config, AppSetting.RecipeEditRecipeInitialPath);
                if (result == true)
                {
                    RecipeFilePath = recipeFilePath!;
                    AppSetting.RecipeEditRecipeInitialPath = Path.GetDirectoryName(recipeFilePath);
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
                recipe.Foreground = Recipe.DefaultForeground;
            }
            newlyAddedForMarking.Clear();
        }

        public void onRecipeSelected(Recipe? recipe)
        {
            if(recipe != null)
            {
                ControlUIEnabled = true;
                if (recipeStateUpdater == null)
                {
                    recipeStateUpdater = new RecipeStateUpader();
                }
                recipeStateUpdater.setSelectedRecipeStep(recipe);
            }
            else
            {
                ControlUIEnabled = false;
                recipeStateUpdater?.clean();
                recipeStateUpdater = null;
            }
            CurrentRecipeInformationViewModel?.setCurrentRecipe(recipe);

        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

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

        private string? _recipeFilePath = null;
        public string? RecipeFilePath
        {
            get { return _recipeFilePath; }
            set { SetProperty(ref _recipeFilePath, value); }
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

        public List<Recipe> newlyAddedForMarking = new List<Recipe>();
        private RecipeStateUpader? recipeStateUpdater;
        private ObservableManager<(string, IList<Recipe>)>.Publisher loadToRecipeRunPublisher;
        private ObservableManager<int>.Publisher switchTabToDataRunPublisher;


        [ObservableProperty]
        private RecipeInformationViewModel? _currentRecipeInformationViewModel;
    }
}
