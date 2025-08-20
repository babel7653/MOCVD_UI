using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using System.Collections;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel
    {
        public class LogIntervalInRecipeRunListener : IObserver<int>
        {
            public LogIntervalInRecipeRunListener(RecipeRunViewModel viewModel, int currentLogIntervalInRecipeRun)
            {
                recipeRunViewModel = viewModel;
                currentValue = currentLogIntervalInRecipeRun;
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
                if (currentValue != value)
                {
                    recipeRunViewModel.resetLogTimer(value);
                    currentValue = value;
                }
            }
            RecipeRunViewModel recipeRunViewModel;
            int currentValue;
        }

        private class LoadFromRecipeEditSubscriber : IObserver<(string, IList<Recipe>)>
        {
            internal LoadFromRecipeEditSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }

            void IObserver<(string, IList<Recipe>)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, IList<Recipe>)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, IList<Recipe>)>.OnNext((string, IList<Recipe>) value)
            {
                if (0 < value.Item2.Count)
                {
                    recipeRunViewModel.CurrentRecipe = new RecipeContext(value.Item1 != "" ? value.Item1 : "recipe_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv", value.Item2);
                }
            }

            private RecipeRunViewModel recipeRunViewModel;
        }

        internal class RecipeEndedSubscriber : IObserver<bool>
        {
            internal RecipeEndedSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }

            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                recipeRunViewModel.switchState(RecipeUserState.Ended);
            }

            private RecipeRunViewModel recipeRunViewModel;
        }

        private class LogicalInterlockStateSubscriber : IObserver<BitArray>
        {
            public LogicalInterlockStateSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }

            void IObserver<BitArray>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<BitArray>.OnNext(BitArray value)
            {
                if (value[10] != prevValue)
                {
                    recipeRunViewModel.RecipeStartAvailableInterlock = value[10];
                    prevValue = value[10];
                }
            }

            bool? prevValue = null;
            RecipeRunViewModel recipeRunViewModel;
        }

        private class ControlModeChangingSubscriber : IObserver<PLCService.ControlMode>
        {
            public ControlModeChangingSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }
            void IObserver<PLCService.ControlMode>.OnCompleted()
            {
                throw new NotImplementedException();
            }
            void IObserver<PLCService.ControlMode>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }
            void IObserver<PLCService.ControlMode>.OnNext(PLCService.ControlMode value)
            {
                if (value != PLCService.ControlMode.Recipe)
                {
                    if (recipeRunViewModel.recipeRunning() == true)
                    {
                        recipeRunViewModel.RecipeStop();
                    }
                }
            }
            private RecipeRunViewModel recipeRunViewModel;
        }
    }
}
