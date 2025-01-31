using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.Common;
using System.ComponentModel;

namespace SapphireXR_App.Controls
{
    public class Valve : UserControl
    {
        public enum UpdateTarget { CurrentPLCState, CurrentRecipeStep };
        public string ValveID
        {
            get;
            set;
        } = "";

        public UpdateTarget Target { get; set; } = UpdateTarget.CurrentPLCState;
    }
}
