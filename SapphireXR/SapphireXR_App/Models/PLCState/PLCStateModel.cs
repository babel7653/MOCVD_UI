using SapphireXR_App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Models.PLCState
{
    internal abstract class PLCStateModel
    {
        internal class ValveStateRequestListener : IObserver<bool>
        {
            internal ValveStateRequestListener(string valveID, string valveStateIssuePrefix, Func<string, bool> onRequest)
            {
                valveStateIssuer = ObservableManager<bool>.Get("ValveState." + valveID + "." + valveStateIssuePrefix);
                onNext = onRequest;
                ID = valveID; 
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
                valveStateIssuer.Issue(onNext(ID));
            }

            ObservableManager<bool>.DataIssuer valveStateIssuer;
            Func<string, bool> onNext;
            string ID;
        }

        protected PLCStateModel(Func<string, bool> onValveStateRequest, string valveStateIssuePrefix) 
        {
            valveStateRequestListeners = new Dictionary<string, ValveStateRequestListener>();
            foreach ((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx1)
            {
                valveStateRequestListeners[valveID] = new ValveStateRequestListener(valveID, valveStateIssuePrefix, onValveStateRequest);
            }
            foreach ((string valveID, int index) in PLCService.ValveIDtoOutputSolValveIdx2)
            {
                valveStateRequestListeners[valveID] = new ValveStateRequestListener(valveID, valveStateIssuePrefix, onValveStateRequest);
            }
        }

        protected Dictionary<string, ValveStateRequestListener> valveStateRequestListeners;
    }
}
