using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel
    {
        public partial class RecipeContext : ObservableObject, IDisposable
        {
         
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

            public class Logger: IDisposable
            {
                internal Logger(RecipeContext recipeContextObj)
                {
                    try
                    {
                        fileStream = new FileStream(recipeContextObj.LogFilePath, FileMode.Create);
                        streamWriter = new StreamWriter(fileStream);
                        csvWriter = new CsvWriter(streamWriter, Config);

                        csvWriter.WriteHeader<RecipeLog>();
                        csvWriter.NextRecord();
                    }
                    catch (Exception)
                    {
                        csvWriter?.Dispose();
                        streamWriter?.Close();
                        fileStream?.Close();

                        throw;
                    }

                    recipeContext = recipeContextObj;

                    logTimer = new DispatcherTimer();
                    logTimer.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * AppSetting.LogIntervalInRecipeRunInMS);
                    logTimer.Tick += log;
                }

                private void log(object? sender, EventArgs args)
                {
                    if (recipeContext != null && recipeContext.currentRecipe != null)
                    {
                        csvWriter!.WriteRecord(new RecipeLog(recipeContext.currentRecipe));
                        csvWriter!.NextRecord();
                    }
                }

                public void pause()
                {
                    logTimer?.Stop();
                }

                public void start()
                {
                    logTimer?.Start();
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!disposedValue)
                    {
                        if (disposing)
                        {
                          
                        }

                        if (logTimer != null)
                        {
                            logTimer.Stop();
                            logTimer.Tick -= log;
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
                   
                        disposedValue = true;
                    }
                }

                public void Dispose()
                {
                    // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                    Dispose(disposing: true);
                    GC.SuppressFinalize(this);
                }

                public void dispose()
                {
                    Dispose(true);
                }

                private DispatcherTimer? logTimer = null;
                private FileStream? fileStream = null;
                private StreamWriter? streamWriter = null;
                private CsvWriter? csvWriter = null;
                private RecipeContext? recipeContext = null;

                private bool disposedValue = false;
            }


            public RecipeContext() { }
            public RecipeContext(string recipeFilePath, IList<Recipe> recipes)
            {
                RecipeFilePath = recipeFilePath;
                Recipes = recipes;

                int totalRecipeTime = 0;
                foreach (Recipe recipe in Recipes)
                {
                    totalRecipeTime += recipe.RTime;
                    totalRecipeTime += recipe.HTime;
                }
                TotalRecipeTime = totalRecipeTime;
                TotalStep = Recipes.Count;

                PropertyChanging += (object? sender, PropertyChangingEventArgs args) =>
                {
                    switch(args.PropertyName)
                    {
                        case nameof(FileLogger):
                            FileLogger?.dispose();
                            break;
                    }
                };

                initialized = true;
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
                            temperatureControlValueUnsubscriber ??= ObservableManager<int>.Subscribe("FlowControl.Temperature.ControlValue", temperatureControlValueSubscriber!);
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

            public void startLog()
            {
                if(initialized == false)
                {
                    return;
                }

                if (FileLogger == null)
                {
                    string fileName = Path.GetFileName(RecipeFilePath);
                    int fileNameEnd = fileName.LastIndexOf('.');
                    if(fileNameEnd == -1)
                    {
                        fileNameEnd = fileName.Length;
                    }
                    if(Path.Exists(AppSetting.LogFileDirectory) == false)
                    {
                        try
                        {
                            Directory.CreateDirectory(AppSetting.LogFileDirectory);
                        }
                        catch(Exception exception)
                        {
                            MessageBox.Show("로그 디렉토리을 생성하는데 실패했습니다. 로그 기능은 작동하지 않은 채로 동작합니다. 원인은 다음과 같습니다: " + exception.Message);
                        }
                }
                    LogFilePath = AppSetting.LogFileDirectory + "\\" + fileName.Substring(0, fileNameEnd) + "_" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + fileName.Substring(fileNameEnd);
                    try
                    {
                        FileLogger = new Logger(this);
                    }
                    catch (Exception exception)
                    {
                        FileLogger = null;
                        MessageBox.Show("로그 파일을 여는데 실패했습니다. 로그 기능은 작동하지 않은 채로 동작합니다. 원인은 다음과 같습니다: " + exception.Message);
                    }
                }
                FileLogger?.start();
            }

            public void pauseLog()
            {
                if (initialized == false)
                {
                    return;
                }
                FileLogger?.pause();
            }

            public void stopLog()
            {
                if (initialized == false)
                {
                    return;
                }
                FileLogger = null;
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

            private void disposeSubscribe()
            {
                temperatureControlValueUnsubscriber?.Dispose();
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
                disposeResource();
                temperatureControlValueUnsubscriber = null;
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
            private bool disposedValue = false;

            private TemperatureControlValueSubscriber? temperatureControlValueSubscriber;
            private IDisposable? temperatureControlValueUnsubscriber = null;
            private RecipeTimeSubscriber? recipeControlHoldTimeSubscriber = null;
            private RecipeTimeSubscriber? recipeControlPauseTimeSubscriber = null;
            private RecipeTimeSubscriber? recipeControlRampTimeSubscriber = null;
            private IDisposable? recipeControlHoldTimeUnsubscriber = null;
            private IDisposable? recipeControlPauseTimeUnsubscriber = null;
            private IDisposable? recipeControlRampTimeUnsubscriber = null;

            [ObservableProperty]
            private Logger? _fileLogger = null;

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
                    disposeSubscribe();
                    disposeResource();
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

            public void disposeResource()
            {
                FileLogger?.dispose();
            }
        }
    }
}
