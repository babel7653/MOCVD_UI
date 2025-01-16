using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using SapphireXR_App.Views;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeViewModel : ViewModelBase
    {
        public partial class TabDataGridViewModel: ObservableObject
        {
            public enum State {  NoneSelected = 0, Selected, Copied };

            public TabDataGridViewModel(string name, string[] gridNames, RecipeViewModel upperLevelViewModel)
            {
                Selected = null;
                CurrentState = State.NoneSelected;
                RecipeViewModel = upperLevelViewModel;
            }

            public void reset()
            {
                deselect();
            }

            private void rearangeNumber(int insert)
            {
                for(int index = insert; index < RecipeViewModel.Recipes!.Count; ++index)
                {
                    RecipeViewModel.Recipes[index].No = (short)(index + 1);
                }
            }

            private void deselect()
            {
                CurrentState = State.NoneSelected;
                Selected = null;
                copied = null;
                RecipeViewModel.cleanupNewlyAdded();
            }

            private void select(IList selected)
            {
                RecipeViewModel.cleanupNewlyAdded();
                Selected = selected;

                if (0 < Selected.Count && 0 < selected.Count)
                {
                    if (CurrentState == State.NoneSelected)
                    {
                        CurrentState = State.Selected;
                    }
                }
                else
                {
                    deselect();
                }
            }

            private void setCopied(IList toCopy)
            {
                if (CurrentState != State.NoneSelected)
                {
                    copied = new List<Recipe>();
                    List<object> noneRecipes = new List<object>();
                    foreach(object item in toCopy)
                    {
                        if(item is Recipe)
                        { 
                            copied.Add((Recipe)item);
                        }
                        else
                        {
                            noneRecipes.Add(item);
                        }
                    }
                    CurrentState = State.Copied;

                    if (0 < noneRecipes.Count)
                    {
                        throw new Exception("There is none recipe item of selected items.\r\n" + noneRecipes.ToString());
                    }
                }
            }

            private Recipe insert(int index)
            {
                Recipe added = new Recipe();
                RecipeViewModel.Recipes!.Insert(index, added);
                added.Background = Brushes.LightPink;
                RecipeViewModel.newlyAddedForMarking.Add(added);
                rearangeNumber(index);

                return added;
            }

            static private int selectedIndex(IList selected)
            {
                return ((Recipe)selected[selected.Count - 1]!).No - 1;
            }

            [ObservableProperty]
            private State _currentState;
            [ObservableProperty]
            private IList? _selected;
            private IList<Recipe>? copied = null;
            public RecipeViewModel RecipeViewModel { get; set; }

            public IRelayCommand SelectionChangedCommand => new RelayCommand<object?>((object? args) =>
            {
                DataGrid? dataGrid = (args as SelectionChangedEventArgs)?.Source as DataGrid;
                if (dataGrid != null)
                {
                    foreach(object item in dataGrid.SelectedItems)
                    {
                        if(item is not Recipe)
                        {
                            deselect();
                            dataGrid.Dispatcher.InvokeAsync(() =>
                            {
                                dataGrid.UpdateLayout();
                                dataGrid.UnselectAll();
                            });
                            
                            return;
                        }
                    }
                    select(dataGrid.SelectedItems);
                }
            });
            public IRelayCommand CopyCommand => new RelayCommand(() => {
                setCopied(Selected!);
            });
            public IRelayCommand PasteCommand => new RelayCommand(() =>
            {
                if(CurrentState == State.Copied)
                {
                    if (RecipeViewModel.Recipes != null)
                    {
                        int insert = selectedIndex(Selected!);
                        int copyCount = copied!.Count;
                        
                        IList<Recipe> added = RecipeViewModel.Recipes.CopyInsertRange(insert, copied!);
                        foreach(var recipe in added)
                        {
                            recipe.Background = Brushes.LightPink;
                        }
                        rearangeNumber(insert);

                        RecipeViewModel.newlyAddedForMarking.AddRange(added);
                       
                    }
                }
            });
            public IRelayCommand DeleteStepCommand => new RelayCommand(() =>
            {
                if(State.Selected <= CurrentState)
                {
                    RecipeViewModel.Recipes!.RemoveAt(Selected!);
                    rearangeNumber(0);
                }
            });
            public IRelayCommand InsertStepCommand => new RelayCommand(() =>
            {
                if (State.Selected <= CurrentState)
                {
                    insert(selectedIndex(Selected!));
                }
            });
            public IRelayCommand AddStepCommand => new RelayCommand<object?>((object? args) =>
            {
                if (RecipeViewModel.Recipes != null)
                {
                    Recipe recipe = insert(RecipeViewModel.Recipes.Count);

                    DataGrid? dataGrid = args as DataGrid;
                    if (dataGrid != null)
                    {
                        dataGrid.Dispatcher.InvokeAsync(() =>
                        {
                            dataGrid.UpdateLayout();
                            dataGrid.ScrollIntoView(recipe);
                        });
                    }
                }
                else
                {
                    throw new Exception("TabDataGridViewModel in RecipeViewModel: Add Step is executed without initializing RecipeViewModel.Recipes");
                }
            });
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
        public IRelayCommand RecipeStartCommand => new RelayCommand(RecipeStart);
        public IRelayCommand RecipeStopCommand => new RelayCommand(RecipeStop);
        public IRelayCommand NavigateCommand => new RelayCommand<string?>(OnNavigate);
        public IRelayCommand RecipePLCLoadCommand { get; set; }
        public IRelayCommand RecipeSaveCommand { get; set; }
        public IRelayCommand RecipeSaveAsCommand { get; set; }

        private bool initialPLCLoadCommand = true;
        public static bool bRecipeStart { get; set; }
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
        public RecipeViewModel()
#pragma warning restore CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
        {
            //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";
            ShowCopyMenu = Visibility.Hidden;
            ShowPasteMenu = Visibility.Hidden;

            ReactorDataGridContext = new TabDataGridViewModel(GridNames[0], GridNames, this);
            FlowDataGridContext = new TabDataGridViewModel(GridNames[1], GridNames, this);
            ValveDataGridContext = new TabDataGridViewModel(GridNames[2], GridNames, this);

            RecipePLCLoadCommand = new RelayCommand(() =>
            {
                try
                {
                    PlcRecipe[] aRecipePLC = new PlcRecipe[Recipes!.Count];
                    int i = 0;
                    foreach (Recipe iRecipeRow in Recipes!)
                    {
                        aRecipePLC[i] = new PlcRecipe(iRecipeRow);
                        i += 1;
                    };
                    PLCService.WriteRecipe(aRecipePLC);
                    PLCService.WriteTotalStep((short)aRecipePLC.Length);
                    bRecipeStart = false;
                    PLCService.WriteStart(bRecipeStart);
                    short operationState;
                    if (initialPLCLoadCommand == false)
                    {
                        operationState = 10;
                    }
                    else
                    {
                        operationState = 0;
                        initialPLCLoadCommand = false;
                    }
                    PLCService.WriteOperationState(operationState);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
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

        private void RecipeViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                    };
                    break;

                case nameof(RecipeFilePath):
                    RecipeSaveCommand.NotifyCanExecuteChanged();
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
            RecipeFilePath = openFile.FileName;

            using (StreamReader streamReader = new StreamReader(RecipeFilePath))
            {
                using (var csvReader = new CsvReader(streamReader, Config))
                {
                    Recipes = new RecipeObservableCollection(csvReader.GetRecords<Recipe>().ToList());
                    //dtgRecipes.ItemsSource = Recipes;
                }
                //OnPropertyChanged(nameof(Recipes)); //추가 검토
                // Recipe Initial State
            }
        }
        private void RecipeStart()
        {
            try
            {
                bRecipeStart = true;
                PLCService.WriteStart(bRecipeStart);
                PLCService.WriteOperationState(10);
            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        private void RecipeStop()
        {
            try
            {
                bRecipeStart = false;
                PLCService.WriteStart(bRecipeStart);
                PLCService.WriteOperationState(40);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
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

        private string[] GridNames = new string[3] { "reactorGridData", "flowGridData", "valveGridData" };
    }
}
