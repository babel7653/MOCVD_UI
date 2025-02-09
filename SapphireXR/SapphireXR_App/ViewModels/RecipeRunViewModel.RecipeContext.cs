using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using SapphireXR_App.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel
    {
        public partial class RecipeContext : ObservableObject, IDisposable
        {
            internal class RecipeContextCreationException: Exception
            {
                internal RecipeContextCreationException(string message): base(message) {  }
            }

            public RecipeContext() { }
            public RecipeContext(string recipeFilePath, IList<Recipe> recipes)
            {
                RecipeFilePath = recipeFilePath;
                DirectoryInfo directoryInfo = new DirectoryInfo(recipeFilePath);
                
                LogFilePath = RecipeFilePath.Replace(".csv", "_log.csv");
                Recipes = recipes;

                try
                {
                    fileStream = new FileStream(LogFilePath, FileMode.Create);
                    streamWriter = new StreamWriter(fileStream);
                    csvWriter = new CsvWriter(streamWriter, Config);

                    csvWriter!.WriteHeader<RecipeLog>();
                    csvWriter!.NextRecord();
                    initialized = true;
                }
                catch (Exception exception)
                {
                    csvWriter?.Dispose();
                    streamWriter?.Close();
                    fileStream?.Close();
                    
                    throw new RecipeContextCreationException(exception.Message);
                }
            }

            ~RecipeContext()
            {
                Dispose(disposing: false);
            }

            public Recipe? markCurrent(short index)
            {
                if (initialized == false)
                {
                    return null;
                }

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
                            currentRecipeIndex = index;
                            currentRecipe.Background = Brushes.LightGoldenrodYellow;

                            return currentRecipe;
                        }
                    }
                }

                return null;
            }

            public void loadPLCSubRangeOfRecipes()
            {
                if(initialized == false)
                {
                    return;
                }

                if (currentRecipe != null)
                {
                    PlcRecipe[] plcRecipes = modifiedRecipeIndice.Where((int recipeIndex) => currentRecipeIndex < recipeIndex).Select((int recipeIndex) => new PlcRecipe(Recipes[recipeIndex])).ToArray();
                    if (0 < plcRecipes.Length)
                    {
                        PLCService.RefreshRecipe(plcRecipes);
                        modifiedRecipeIndice.Clear();
                    }
                }
            }

            public void addModfiedRecipe(Recipe recipe)
            {
                if (initialized == false)
                {
                    return;
                }

                int recipeIndex = Recipes.IndexOf(recipe);
                if(recipeIndex != -1)
                {
                    modifiedRecipeIndice.Add(recipeIndex);
                }
            }

            public void log()
            {
                if(initialized == false)
                {
                    return;
                }

                if (currentRecipe != null)
                {
                    csvWriter!.WriteRecord<RecipeLog>(new RecipeLog(currentRecipe));
                    csvWriter!.NextRecord();
                }
            }

            public void toLoadedFromFileState()
            {
                if (currentRecipe != null)
                {
                    currentRecipe.Background = Brushes.White;
                }
                currentRecipe = null;
                currentRecipeIndex = -1;
                modifiedRecipeIndice.Clear();
            }

            private readonly bool initialized = false;
            private HashSet<int> modifiedRecipeIndice = new HashSet<int>();

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

            [ObservableProperty]
            private int _currentRecipeTime;
            [ObservableProperty]
            private int _totalRecipeTime;
            [ObservableProperty]
            private int _currentStep;
            [ObservableProperty]
            private int _totalStep;
            [ObservableProperty]
            private int _stepName;
            [ObservableProperty]
            private int _currentRampTime;
            [ObservableProperty]
            private int _totalRampTime;
            [ObservableProperty]
            private int _currentHoldTime;
            [ObservableProperty]
            private int _totalHoldTime;
            [ObservableProperty]
            private int _pauseTime;
            [ObservableProperty]
            private int _currentLoopStep;
            [ObservableProperty]
            private int _totalLoopStep;
            [ObservableProperty]
            private int _currentLoopNumber;
            [ObservableProperty]
            private int _totalLoopNumber;
            [ObservableProperty]
            private int _currentWaitTemp;
            [ObservableProperty]
            private int _totalWaitTemp;

            public Recipe? currentRecipe = null;
            public int currentRecipeIndex = -1;
            private readonly FileStream? fileStream = null;
            private readonly StreamWriter? streamWriter = null;
            private readonly CsvWriter? csvWriter = null;
            private bool disposedValue = false;

            private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    DisposeResource();
                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public void dispose()
            {
                Dispose(disposing: false);
            }

            public void DisposeResource()
            {
                if(initialized == false)
                {
                    return;
                }

                try
                {
                    csvWriter!.Flush();
                    csvWriter!.Dispose();
                    streamWriter!.Close();
                    fileStream!.Close();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
