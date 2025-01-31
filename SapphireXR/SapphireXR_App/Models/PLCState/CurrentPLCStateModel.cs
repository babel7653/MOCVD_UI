using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Models.PLCState
{
    internal class CurrentPLCStateModel: PLCStateModel
    {
        internal CurrentPLCStateModel(): base((string valveID) => PLCService.ReadValveState(valveID), "CurrentPLCState") {  }
    }
}
