using System.ComponentModel;
using System.Windows;
using SapphireXR_App.Controls;
using SapphireXR_App.Views;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Reactive;
using System.Windows.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using Caliburn.Micro;

namespace SapphireXR_App.ViewModels.FlowController
{
    public class RecipeRunFlowControllerViewModel : HomeFlowControllerViewModel
    {
        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            selectedThis = ObservableManager<string>.Get("FlowControl.Selected.CurrentRecipeStep");
        }

        protected override void onClicked(object[]? args)
        {
            selectedThis?.Issue(ControllerID);
        }

        private ObservableManager<string>.DataIssuer? selectedThis;
    }
}
