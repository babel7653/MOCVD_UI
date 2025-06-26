using SapphireXR_App.Common;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.ViewModels.FlowController
{
    internal partial class RecipeEditFlowControllerViewModel: FlowControllerViewModelBase, IObserver<float>
    {
        class ControlValueResetSubscriber : IObserver<bool>
        {
            public ControlValueResetSubscriber(RecipeEditFlowControllerViewModel vm)
            {
                recipeEditFlowControllerViewModel = vm;
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
                recipeEditFlowControllerViewModel.ControlValue = "";
                
            }

            private RecipeEditFlowControllerViewModel recipeEditFlowControllerViewModel;
        }

        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            string controlValuePubilshSubscribeTopic = "FlowControl." + controllerID + ".CurrentValue.CurrentRecipeStep";
            ObservableManager<float>.Subscribe(controlValuePubilshSubscribeTopic, this);
            controlValuePublisher = ObservableManager<float>.Get(controlValuePubilshSubscribeTopic);
            ObservableManager<bool>.Subscribe("Reset.CurrentRecipeStep", controlValueResetSubscriber = new ControlValueResetSubscriber(this));
            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(ControlValue):
                        try
                        {
                            if (ControlValue != "")
                            {
                                controlValuePublisher.Issue(float.Parse(ControlValue));
                            }
                        }
                        catch(ArgumentNullException)
                        {

                        }
                        catch (FormatException)
                        {

                        }
                        catch(OverflowException)
                        { 
                        }
                        break;
                }
            };
            int? redMaxValue = SettingViewModel.ReadMaxValue(controllerID);
            if (redMaxValue != null)
            {
                MaxValue = redMaxValue.Value;
            }
            else
            {
                throw new Exception("Faiure happend in reading max value for flow controller in recipe edit view. Logic error in RecipeEditFlowControllerViewModel.onLoaded in HomeFlowControllerViewModel: "
                       + "the value of \"controllerID\", the second argument of the onLoaded method \"" + controllerID + "\" is not valid flow controller ID");
            }
        }

        protected override void onClicked(object[]? args)
        {
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
            string controlValueStr = ((int)value).ToString();
            if (controlValueStr != ControlValue)
            {
                ControlValue = controlValueStr;
            }
        }

        [ObservableProperty]
        int _maxValue;

        private ObservableManager<float>.DataIssuer? controlValuePublisher;
        private ControlValueResetSubscriber? controlValueResetSubscriber;
    }
}
