using SapphireXR_App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Models
{
    internal class CurrentRecipeStep : IObserver<Recipe>
    {
        internal CurrentRecipeStep()
        {
            ObservableManager<Recipe>.Subscribe("CurrentSelectedRecipe", this);
            foreach((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx1)
            {
                valveStatePublishers[valveID] = ObservableManager<(object, bool)>.Get("Valve.OnOff." + valveID + ".CurrentRecipeStep");
            }
            foreach ((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx2)
            {
                valveStatePublishers[valveID] = ObservableManager<(object, bool) >.Get("Valve.OnOff." + valveID + ".CurrentRecipeStep");
            }
            foreach((string flowControllerID, int index) in PLCService.dIndexController)
            {
                flowValuePublishers[flowControllerID] = ObservableManager<float>.Get("FlowControl." + flowControllerID + ".CurrentValue.CurrentRecipeStage");
            }
        }

        void IObserver<Recipe>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<Recipe>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<Recipe>.OnNext(Recipe value)
        {
            valveStatePublishers["V01"].Issue((this, value.V01));
            valveStatePublishers["V02"].Issue((this, value.V02));
            valveStatePublishers["V03"].Issue((this, value.V03));
            valveStatePublishers["V04"].Issue((this, value.V04));
            valveStatePublishers["V05"].Issue((this, value.V05));
            valveStatePublishers["V07"].Issue((this, value.V07));
            valveStatePublishers["V08"].Issue((this, value.V08));
            valveStatePublishers["V10"].Issue((this, value.V10));
            valveStatePublishers["V11"].Issue((this, value.V11));
            valveStatePublishers["V13"].Issue((this, value.V13));
            valveStatePublishers["V14"].Issue((this, value.V14));
            valveStatePublishers["V16"].Issue((this, value.V16));
            valveStatePublishers["V17"].Issue((this, value.V17));
            valveStatePublishers["V19"].Issue((this, value.V19));
            valveStatePublishers["V20"].Issue((this, value.V20));
            valveStatePublishers["V22"].Issue((this, value.V22));
            valveStatePublishers["V23"].Issue((this, value.V23));
            valveStatePublishers["V24"].Issue((this, value.V24));
            valveStatePublishers["V25"].Issue((this, value.V25));
            valveStatePublishers["V26"].Issue((this, value.V26));
            valveStatePublishers["V27"].Issue((this, value.V27));
            valveStatePublishers["V28"].Issue((this, value.V28));
            valveStatePublishers["V29"].Issue((this, value.V29));
            valveStatePublishers["V30"].Issue((this, value.V30));
            valveStatePublishers["V31"].Issue((this, value.V31));
            valveStatePublishers["V32"].Issue((this, value.V32));

            flowValuePublishers["MFC01"].Issue(value.M01);
            flowValuePublishers["MFC02"].Issue(value.M02);
            flowValuePublishers["MFC03"].Issue(value.M03);
            flowValuePublishers["MFC04"].Issue(value.M04);
            flowValuePublishers["MFC05"].Issue(value.M05);
            flowValuePublishers["MFC06"].Issue(value.M06);
            flowValuePublishers["MFC07"].Issue(value.M07);
            flowValuePublishers["MFC08"].Issue(value.M08);
            flowValuePublishers["MFC09"].Issue(value.M09);
            flowValuePublishers["MFC10"].Issue(value.M10);
            flowValuePublishers["MFC11"].Issue(value.M11);
            flowValuePublishers["MFC12"].Issue(value.M12);
            flowValuePublishers["MFC13"].Issue(value.M13);
            flowValuePublishers["MFC14"].Issue(value.M14);
            flowValuePublishers["MFC15"].Issue(value.M15);
            flowValuePublishers["MFC16"].Issue(value.M16);
            flowValuePublishers["MFC17"].Issue(value.M17);
            flowValuePublishers["MFC18"].Issue(value.M18);
            flowValuePublishers["MFC19"].Issue(value.M19);
            flowValuePublishers["EPC01"].Issue(value.E01);
            flowValuePublishers["EPC02"].Issue(value.E02);
            flowValuePublishers["EPC03"].Issue(value.E03);
            flowValuePublishers["EPC04"].Issue(value.E04);
            flowValuePublishers["EPC05"].Issue(value.E05);
            flowValuePublishers["EPC06"].Issue(value.E06);
            flowValuePublishers["EPC07"].Issue(value.E07);
            flowValuePublishers["Temperature"].Issue(value.STemp);
            flowValuePublishers["Pressure"].Issue(value.RPress);
            flowValuePublishers["Rotation"].Issue(value.SRotation);
        }

        private Dictionary<string, ObservableManager<(object, bool)>.DataIssuer> valveStatePublishers = new Dictionary<string, ObservableManager<(object, bool)>.DataIssuer>();
        private Dictionary<string, ObservableManager<float>.DataIssuer> flowValuePublishers = new Dictionary<string, ObservableManager<float>.DataIssuer>();
    }
}
