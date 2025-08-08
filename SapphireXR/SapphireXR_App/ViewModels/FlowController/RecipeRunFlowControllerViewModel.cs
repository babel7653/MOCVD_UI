using SapphireXR_App.Common;

namespace SapphireXR_App.ViewModels.FlowController
{
    public class RecipeRunFlowControllerViewModel : HomeFlowControllerViewModel
    {
        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            selectedThis = ObservableManager<string>.Get("FlowControl.Selected.CurrentPLCState.RecipeRun");
        }

        protected override void onClicked(object[]? args)
        {
            selectedThis?.Publish(ControllerID);
        }

        private ObservableManager<string>.Publisher? selectedThis;
    }
}
