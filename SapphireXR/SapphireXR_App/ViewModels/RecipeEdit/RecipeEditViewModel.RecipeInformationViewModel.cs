using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeEditViewModel: ViewModelBase
    {
        public partial class RecipeInformationViewModel : ObservableObject, IObserver<IList<Recipe>>, IDisposable
        {
            public RecipeInformationViewModel(RecipeObservableCollection recipeList)
            {
                recipes = recipeList;
                recipes.CollectionChanged += recipeCollectionChanged;
                unsubscriber = ObservableManager<IList<Recipe>>.Subscribe("RecipeEdit.TabDataGrid.RecipeAdded", this);
                foreach (Recipe recipe in recipes)
                {
                    recipe.PropertyChanged += recipePropertyChanged;
                }
                refreshTotal();
            }

            private void recipeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
            {
                refreshTotal();
            }

            private void recipePropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                switch (args.PropertyName)
                {
                    case nameof(Recipe.RTime):
                        if (sender == currentStep || sender == prevStep)
                        {
                            refreshRampingRateTemp();
                            refreshRampingRatePress();
                        }
                        refreshTotal();
                        break;

                    case nameof(Recipe.HTime):
                        refreshTotal();
                        break;

                    case nameof(Recipe.STemp):
                        if (sender == currentStep || sender == prevStep)
                        {
                            refreshRampingRateTemp();
                        }
                        break;

                    case nameof(Recipe.RPress):
                        if (sender == currentStep || sender == prevStep)
                        {
                            refreshRampingRatePress();
                        }
                        break;

                    case nameof(Recipe.M01):
                    case nameof(Recipe.M02):
                    case nameof(Recipe.M03):
                    case nameof(Recipe.M04):
                    case nameof(Recipe.M07):
                    case nameof(Recipe.M08):
                    case nameof(Recipe.M09):
                    case nameof(Recipe.M10):
                    case nameof(Recipe.M11):
                    case nameof(Recipe.M14):
                    case nameof(Recipe.M15):
                    case nameof(Recipe.M16):
                    case nameof(Recipe.M17):
                    case nameof(Recipe.V30):
                    case nameof(Recipe.V29):
                    case nameof(Recipe.V31):
                    case nameof(Recipe.V23):
                    case nameof(Recipe.V24):
                    case nameof(Recipe.V25):
                    case nameof(Recipe.V26):
                    case nameof(Recipe.V27):
                    case nameof(Recipe.V28):
                        refreshTotalFlowRate();
                        break;
                }
            }

            private void refreshTotal()
            {
                if (0 < recipes.Count)
                {
                    int totalTime = 0;
                    foreach (Recipe recipe in recipes)
                    {
                        totalTime += (recipe.RTime + recipe.HTime);
                    }
                    TotalRecipeTime = totalTime;
                    TotalStepNumber = recipes.Count;
                }
                else
                {
                    TotalRecipeTime = null;
                    TotalStepNumber = null;
                }
            }

            private T? findDefaultValue<T>(Recipe current, Func<Recipe, T?> selector) where T : struct
            {
                Recipe? recipe = recipes.Where(recipe => recipe.No < current.No).Reverse().FirstOrDefault(recipe => selector(recipe) != null);
                if (recipe != null)
                {
                    return selector(recipe);
                }
                else
                {
                    return null;
                }
            }

            private void refreshRampingRateTemp()
            {
                if (currentStep != null)
                {
                    short? sTempDiff = currentStep.STemp ?? findDefaultValue(currentStep, recipe => recipe.STemp);
                    if (sTempDiff != null)
                    {
                        if (prevStep != null)
                        {
                            short? prevSTemp = prevStep.STemp ?? findDefaultValue(prevStep, recipe => recipe.STemp);
                            if (prevSTemp != null)
                            {
                                RampingRateTemp = Math.Abs(sTempDiff.Value - prevSTemp.Value) / currentStep.RTime;
                                return;
                            }
                        }
                        else
                        {
                            RampingRateTemp = sTempDiff / currentStep.RTime;
                            return;
                        }
                    }
                }
                RampingRateTemp = null;
            }

            private void refreshRampingRatePress()
            {
                if (currentStep != null)
                {
                    short? rPressDiff = currentStep.RPress ?? findDefaultValue(currentStep, recipe => recipe.RPress);
                    if (rPressDiff != null)
                    {
                        if (prevStep != null)
                        {
                            short? prevRPress = prevStep.RPress ?? findDefaultValue(prevStep, recipe => recipe.RPress);
                            if (prevRPress != null)
                            {
                                RampingRatePress = Math.Abs(rPressDiff.Value - prevRPress.Value) / currentStep.RTime;
                                return;
                            }
                        }
                        else
                        {
                            RampingRatePress = rPressDiff / currentStep.RTime;
                            return;
                        }
                    }
                }
                RampingRatePress = null;
            }

            private void refreshTotalFlowRate()
            {
                if(currentStep == null)
                {
                    return;
                }

                var update = (Func<Recipe, float?> selector, string mfcName, float? totalFlowRate) =>
                {
                    float? currentValue = selector(currentStep);
                    if (currentValue != null)
                    {
                        return currentValue + totalFlowRate;
                    }
                    else
                    {
                        float? defaultValue = findDefaultValue(currentStep, selector);
                        if (defaultValue != null)
                        {
                            return defaultValue + totalFlowRate;

                        }
                        else
                        {
                            return null;
                        }
                    }
                };

                float? totalFlowRate = 0;
                if ((totalFlowRate = update(recipe => recipe.M01, "M01", totalFlowRate)) == null)
                {
                    TotalFlowRate = null;
                    return;
                }
                if ((totalFlowRate = update(recipe => recipe.M02, "M02", totalFlowRate)) == null)
                {
                    TotalFlowRate = null;
                    return;
                }
                if (currentStep.V30 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M03, "M03", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V29 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M04, "M04", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V31 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M07, "M07", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V23 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M08, "M08", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V24 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M09, "M09", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V25 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M10, "M10", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V26 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M11, "M11", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V27 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M14, "M14", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if (currentStep.V28 == true)
                {
                    if ((totalFlowRate = update(recipe => recipe.M15, "M15", totalFlowRate)) == null)
                    {
                        TotalFlowRate = null;
                        return;
                    }
                }
                if ((totalFlowRate = update(recipe => recipe.M16, "M16", totalFlowRate)) == null)
                {
                    TotalFlowRate = null;
                    return;
                }
                if ((totalFlowRate = update(recipe => recipe.M17, "M17", totalFlowRate)) == null)
                {
                    TotalFlowRate = null;
                    return;
                }

                TotalFlowRate = (int?)totalFlowRate;
            }

            public void setCurrentRecipe(Recipe? recipe)
            {
                if (recipe != null)
                {
                    currentStep = recipe;
                    int prevStepIndex = recipe.No - 2;
                    if (0 <= prevStepIndex && prevStepIndex < recipes.Count)
                    {
                        prevStep = recipes[prevStepIndex];
                    }
                    else
                    {
                        prevStep = null;
                    }

                    refreshRampingRatePress();
                    refreshRampingRateTemp();
                    refreshTotalFlowRate();
                }
                else
                {
                    currentStep = null;
                    prevStep = null;

                    RampingRatePress = null;
                    RampingRateTemp = null;
                    TotalFlowRate = null;
                }
            }

            void IObserver<IList<Recipe>>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<IList<Recipe>>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<IList<Recipe>>.OnNext(IList<Recipe> newlyAdded)
            {
                foreach (Recipe recipe in newlyAdded)
                {
                    recipe.PropertyChanged += recipePropertyChanged;
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        unsubscriber.Dispose();
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

            public void dispose()
            {
                Dispose(disposing: true);
            }

            [ObservableProperty]
            private int? _totalRecipeTime = null;
            [ObservableProperty]
            private int? _totalStepNumber = null;
            [ObservableProperty]
            private int? _rampingRateTemp = null;
            [ObservableProperty]
            private int? _rampingRatePress = null;
            [ObservableProperty]
            private int? _totalFlowRate = null;

            private Recipe? prevStep = null;
            private Recipe? currentStep = null;

            private RecipeObservableCollection recipes;
            private IDisposable unsubscriber;
            private bool disposedValue = false;
        }
    }
}
