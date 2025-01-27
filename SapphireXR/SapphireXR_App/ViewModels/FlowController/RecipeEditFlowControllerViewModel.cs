using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.ViewModels.FlowController
{
    internal class RecipeEditFlowControllerViewModel: FlowControllerViewModelBase, IObserver<float>
    {       
        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            ObservableManager<float>.Subscribe("FlowControl." + controllerID + ".CurrentValue.CurrentRecipeStage", this);
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
            ControlValue = ((int)value).ToString();
        }
    }
}
