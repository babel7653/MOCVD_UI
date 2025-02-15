using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Media;
using static SapphireXR_App.ViewModels.RecipeRunViewModel;

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

            private class TemperatureControlValueSubscriber : IObserver<int>
            {
                internal TemperatureControlValueSubscriber(RecipeContext rc)
                {
                    recipeContext = rc;
                }

                void IObserver<int>.OnCompleted()
                {
                    throw new NotImplementedException();
                }

                void IObserver<int>.OnError(Exception error)
                {
                    throw new NotImplementedException();
                }

                void IObserver<int>.OnNext(int value)
                {
                    if(prevValue == null || prevValue != value)
                    {
                        recipeContext.CurrentWaitTemp = value;
                        prevValue = value;
                    }
                }

                private RecipeContext recipeContext;
                private int? prevValue;
            }

            private class RecipeTimeSubscriber : IObserver<int>
            {
                internal RecipeTimeSubscriber(Action<int> onNextAc)
                {
                    onNext = onNextAc;
                }

                void IObserver<int>.OnCompleted()
                {
                    throw new NotImplementedException();
                }

                void IObserver<int>.OnError(Exception error)
                {
                    throw new NotImplementedException();
                }

                void IObserver<int>.OnNext(int value)
                {
                    if(prevValue == null || prevValue != value)
                    {
                        onNext(value);
                        prevValue = value;
                    }
                }

                private int? prevValue = null;
                private readonly Action<int> onNext;
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

                    int totalRecipeTime = 0;
                    int totalCTtemp = 0;
                    foreach(Recipe recipe in recipes)
                    {
                        totalRecipeTime += recipe.RTime;
                        totalRecipeTime += recipe.HTime;
                        totalCTtemp += recipe.cTemp;
                    }
                    TotalRecipeTime = totalRecipeTime;
                    TotalStep = recipes.Count;

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
                                currentRecipe.Foreground = Brushes.Black;
                            }
                            currentRecipe = next;
                            currentRecipeIndex = index;
                            currentRecipe.Background = Brushes.LightGoldenrodYellow;
                            currentRecipe.Foreground = Brushes.Red;
                            currentRecipe.IsEnabled = false;

                            temperatureControlValueSubscriber ??= new TemperatureControlValueSubscriber(this);
                            unsubscriber ??= ObservableManager<int>.Subscribe("FlowControl.Temperature.ControlValue", temperatureControlValueSubscriber!);
                            recipeControlHoldTimeSubscriber ??= new RecipeTimeSubscriber((int value) => { CurrentHoldTime = value; });
                            recipeControlHoldTimeUnsubscriber ??= ObservableManager<int>.Subscribe("RecipeControlTime.Hold", recipeControlHoldTimeSubscriber);
                            recipeControlPauseTimeSubscriber ??= new RecipeTimeSubscriber((int value) => { PauseTime = value; });
                            recipeControlPauseTimeUnsubscriber ??= ObservableManager<int>.Subscribe("RecipeControlTime.Pause", recipeControlPauseTimeSubscriber);
                            recipeControlRampTimeSubscriber ??= new RecipeTimeSubscriber((int value) => { CurrentRampTime = value; });
                            recipeControlRampTimeUnsubscriber ??= ObservableManager<int>.Subscribe("RecipeControlTime.Ramp", recipeControlRampTimeSubscriber);
                            CurrentRecipeTime ??= 0;
                            CurrentRecipeTime += currentRecipe.RTime;
                            CurrentRecipeTime += currentRecipe.HTime;
                            CurrentStep = currentRecipe.No;
                            StepName = currentRecipe.Name;
                            TotalRampTime = currentRecipe.RTime;
                            TotalHoldTime = currentRecipe.HTime;

                            if (0 < currentRecipe.cTemp)
                            {
                                TotalWaitTemp = currentRecipe.cTemp;
                            }
                            else
                            {
                                TotalWaitTemp = null;
                                CurrentWaitTemp = null;
                            }

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

            private void disposeSubscribe()
            {
                unsubscriber?.Dispose();
                recipeControlHoldTimeUnsubscriber?.Dispose();
                recipeControlRampTimeUnsubscriber?.Dispose();
                recipeControlPauseTimeUnsubscriber?.Dispose();
            }

            public void toLoadedFromFileState()
            {
                if (currentRecipe != null)
                {
                    currentRecipe.Background = Brushes.White;
                    currentRecipe.Foreground = Brushes.Black;

                }
                foreach(Recipe recipe in Recipes)
                {
                    recipe.IsEnabled = true;
                }
                currentRecipe = null;
                currentRecipeIndex = -1;
                modifiedRecipeIndice.Clear();
                disposeSubscribe();
                unsubscriber = null;
                temperatureControlValueSubscriber = null;
                recipeControlHoldTimeUnsubscriber = null;
                recipeControlRampTimeUnsubscriber = null;
                recipeControlPauseTimeUnsubscriber = null;
                recipeControlHoldTimeSubscriber = null;
                recipeControlRampTimeSubscriber = null;
                recipeControlPauseTimeSubscriber = null;

                CurrentRecipeTime = null;
                TotalRecipeTime = null;
                CurrentStep = null;
                TotalStep = null;
                StepName = "";
                CurrentRampTime = null;
                TotalRampTime = null;
                CurrentHoldTime = null;
                TotalHoldTime = null;
                PauseTime = null;
                CurrentLoopStep = null;
                TotalLoopStep = null;
                CurrentLoopNumber = null;
                TotalLoopNumber = null;
                CurrentWaitTemp = null;
                TotalWaitTemp = null;
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
            private int? _currentRecipeTime = null;
            [ObservableProperty]
            private int? _totalRecipeTime = null;
            [ObservableProperty]
            private int? _currentStep = null;
            [ObservableProperty]
            private int? _totalStep = null;
            [ObservableProperty]
            private string _stepName = "";
            [ObservableProperty]
            private int? _currentRampTime = null;
            [ObservableProperty]
            private int? _totalRampTime = null;
            [ObservableProperty]
            private int? _currentHoldTime = null;
            [ObservableProperty]
            private int? _totalHoldTime = null;
            [ObservableProperty]
            private int? _pauseTime = null;
            [ObservableProperty]
            private int? _currentLoopStep = null;
            [ObservableProperty]
            private int? _totalLoopStep = null;
            [ObservableProperty]
            private int? _currentLoopNumber = null;
            [ObservableProperty]
            private int? _totalLoopNumber = null;
            [ObservableProperty]
            private int? _currentWaitTemp = null;
            [ObservableProperty]
            private int? _totalWaitTemp = null;

            public Recipe? currentRecipe = null;
            public int currentRecipeIndex = -1;
            private readonly FileStream? fileStream = null;
            private readonly StreamWriter? streamWriter = null;
            private readonly CsvWriter? csvWriter = null;
            private bool disposedValue = false;

            private TemperatureControlValueSubscriber? temperatureControlValueSubscriber;
            private IDisposable? unsubscriber = null;
            private RecipeTimeSubscriber? recipeControlHoldTimeSubscriber = null;
            private RecipeTimeSubscriber? recipeControlPauseTimeSubscriber = null;
            private RecipeTimeSubscriber? recipeControlRampTimeSubscriber = null;
            private IDisposable? recipeControlHoldTimeUnsubscriber = null;
            private IDisposable? recipeControlPauseTimeUnsubscriber = null;
            private IDisposable? recipeControlRampTimeUnsubscriber = null;


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
                        disposeSubscribe();
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
                Dispose(disposing: true);
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
