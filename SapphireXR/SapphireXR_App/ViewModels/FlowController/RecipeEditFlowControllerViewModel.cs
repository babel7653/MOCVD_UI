using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
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
            string controlValuePubilshSubscribeTopic = "FlowControl." + controllerID + ".CurrentValue.CurrentRecipeStage";
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
            MaxValue = (int)PLCService.ReadMaxValue(controllerID);

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
