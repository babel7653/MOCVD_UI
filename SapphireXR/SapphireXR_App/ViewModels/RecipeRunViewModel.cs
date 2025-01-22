using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel: ViewModelBase, IObserver<short>
    {
        public class RecipeContext: ObservableObject, IDisposable
        {
            public RecipeContext()
            {
            }
            public RecipeContext(string recipeFilePath, IList<Recipe> recipes)
            {
                RecipeFilePath = recipeFilePath;
                LogFilePath = RecipeFilePath.Replace(".csv", "_log.csv");
                Recipes = recipes;

                fileStream = new FileStream(LogFilePath, FileMode.Create);
                streamWriter = new StreamWriter(fileStream);
            }

            public void onStart()
            {
                if(firstStart == true)
                {
                    RecipeService.PLCLoad(Recipes, 0);
                    firstStart = false;
                }
                else
                {
                    PLCService.WriteStart(true);
                    PLCService.WriteOperationState(10);
                }
            }

            public Recipe? markCurrent(short index)
            {
                if (0 < index)
                {
                    index -= 1;
                    if (index < Recipes.Count)
                    {
                        Recipe next = Recipes[index];
                        if (next != currentRecipe)
                        {
                            if (currentRecipe != null)
                            {
                                currentRecipe.Background = Brushes.White;
                            }
                            currentRecipe = next;
                            currentRecipe.Background = Brushes.LightGoldenrodYellow;

                            return currentRecipe;
                        }
                    }
                }

                return null;
            }

            private bool firstStart = true;
            private IList<Recipe> _recipes = new List<Recipe>();
            public IList<Recipe> Recipes
            {
                get { return _recipes; }
                set { SetProperty(ref _recipes, value); }
            }

            private string _recipeFilePath = string.Empty;
            public string RecipeFilePath
            {
                get { return _recipeFilePath; }
                set { SetProperty(ref _recipeFilePath, value); }
            }

            private string _logFilePath = string.Empty;
            public string LogFilePath
            {
                get { return _logFilePath; }
                set { SetProperty(ref _logFilePath, value); }
            }

            public Recipe? currentRecipe = null;
            private FileStream fileStream;
            private StreamWriter streamWriter;
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        DisposeResource();
                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public void DisposeResource()
            {
                streamWriter.Close();
                fileStream.Close();
            }
        }

        private static RecipeContext EmptyRecipeContext = new RecipeContext();

        [ObservableProperty]
        private RecipeContext _currentRecipe = EmptyRecipeContext;

        [ObservableProperty]
        string _startText = "Start";

        bool _recipeStarted = false;
        bool RecipeStarted
        {
            get { return _recipeStarted; }
            set { SetProperty(ref _recipeStarted, value); }
        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

        [RelayCommand]
        private void RecipeOpen()
        {
            (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config);
            if (result == true)
            {
                if (0 < recipes!.Count)
                {
                    CurrentRecipe?.DisposeResource();
                    CurrentRecipe = new RecipeContext(recipeFilePath!, recipes!);
                }
                else
                {
                    MessageBox.Show(recipeFilePath + "은 빈 파일입니다.");
                }
            }
        }

      
        [RelayCommand(CanExecute =nameof(CurrentRecipeActive))]
        private void RecipeStart()
        {
            RecipeStarted = !RecipeStarted;
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        void RecipeSkip()
        {
            PLCService.WriteOperationState(60);
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        void RecipeRefresh()
        {

        }

        [RelayCommand]
        void ValveDataGridLoaded(object? args)
        {
            valveDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand]
        void FlowDataGridLoaded(object? args)
        {
            flowDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand]
        void ReactorDataGridLoaded(object? args)
        {
            reactorDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        private void RecipeStop()
        {
            PLCService.WriteStart(false);
            PLCService.WriteOperationState(40);
        }

        public RecipeRunViewModel()
        {
            ObservableManager<short>.Subscribe("RecipeRun.CurrentActiveRecipe", this);
            PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
            {
                var recipeStart = () =>
                {
                    CurrentRecipe.onStart();
                };
                var recipePause = () => 
                {
                    PLCService.WriteOperationState(20);
                }; 
                switch (e.PropertyName)
                {
                    case nameof(RecipeStarted):
                        if(RecipeStarted == true)
                        {
                            recipeStart();
                            StartText = "Pause";

                        }
                        else
                        {
                            recipePause();
                            StartText = "Start";
                        }
                        break;

                    case nameof(CurrentRecipe):
                        RecipeStartCommand.NotifyCanExecuteChanged();
                        RecipeStopCommand.NotifyCanExecuteChanged();
                        RecipeRefreshCommand.NotifyCanExecuteChanged();
                        RecipeSkipCommand.NotifyCanExecuteChanged();
                        break;
                }
            };
        }

        bool CurrentRecipeActive()
        {
            return CurrentRecipe != EmptyRecipeContext && CurrentRecipe != null;
        }

        void IObserver<short>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<short>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<short>.OnNext(short value)
        {
            Recipe? currentRecipe = CurrentRecipe.markCurrent(value);
            if (currentRecipe != null)
            {
                scrollIntoViewCurrentRecipe(reactorDataGrid, currentRecipe);
                scrollIntoViewCurrentRecipe(flowDataGrid, currentRecipe);
                scrollIntoViewCurrentRecipe(valveDataGrid, currentRecipe);
            }
        }

        private static void scrollIntoViewCurrentRecipe(DataGrid? dataGrid, Recipe recipe)
        {
            dataGrid?.Dispatcher.InvokeAsync(() =>
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(recipe);
            });
        }

        private DataGrid? reactorDataGrid;
        private DataGrid? flowDataGrid;
        private DataGrid? valveDataGrid;
    }
}
