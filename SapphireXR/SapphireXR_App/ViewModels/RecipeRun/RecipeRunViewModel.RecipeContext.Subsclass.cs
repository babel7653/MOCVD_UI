using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.IO;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel
    {
        public partial class RecipeContext : ObservableObject, IDisposable
        {
            private class TemperatureCurrentValueSubscriber : IObserver<float>
            {
                internal TemperatureCurrentValueSubscriber(RecipeContext rc)
                {
                    recipeContext = rc;
                }

                void IObserver<float>.OnCompleted()
                {
                    throw new NotImplementedException();
                }

                void IObserver<float>.OnError(Exception error)
                {
                    throw new NotImplementedException();
                }

                void IObserver<float>.OnNext(float value)
                {
                    if (prevValue == null || prevValue != value)
                    {
                        recipeContext.CurrentWaitTemp = (int)value;
                        prevValue = recipeContext.CurrentWaitTemp;
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
                    if (prevValue == null || prevValue != value)
                    {
                        onNext(value);
                        prevValue = value;
                    }
                }

                private int? prevValue = null;
                private readonly Action<int> onNext;
            }

            public class Logger : IDisposable
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
                        csvWriter!.WriteRecord(new RecipeLog((recipeContext.Recipes as List<Recipe>)!));
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

            private class RecipeRunElapsedTimeSubscriber : IObserver<(int, PLCService.RecipeRunETMode)>
            {
                internal RecipeRunElapsedTimeSubscriber(RecipeContext rc)
                {
                    recipeContext = rc;
                }
                void IObserver<(int, PLCService.RecipeRunETMode)>.OnCompleted()
                {
                    throw new NotImplementedException();
                }

                void IObserver<(int, PLCService.RecipeRunETMode)>.OnError(Exception error)
                {
                    throw new NotImplementedException();
                }

                void IObserver<(int, PLCService.RecipeRunETMode)>.OnNext((int, PLCService.RecipeRunETMode) value)
                {
                    if(prevValue != value)
                    {
                        switch (value.Item2)
                        {
                            case PLCService.RecipeRunETMode.None:
                                recipeContext.CurrentRampTime = null;
                                recipeContext.CurrentHoldTime = null;
                                break;

                            case PLCService.RecipeRunETMode.Ramp:
                                recipeContext.CurrentRampTime = value.Item1;
                                recipeContext.CurrentHoldTime = null;
                                break;

                            case PLCService.RecipeRunETMode.Hold:
                                recipeContext.CurrentRampTime = null;
                                recipeContext.CurrentHoldTime = value.Item1;
                                break;
                        }
                        prevValue = value;
                    }
                }

                (int, PLCService.RecipeRunETMode)? prevValue = null;
                private RecipeContext recipeContext;
            }
        }
    }
}