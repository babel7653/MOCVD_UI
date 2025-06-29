using System.Windows.Controls;

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
