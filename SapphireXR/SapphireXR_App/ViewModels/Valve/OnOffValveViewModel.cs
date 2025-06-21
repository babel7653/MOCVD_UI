using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using System.Windows;

namespace SapphireXR_App.ViewModels.Valve
{
    public class OnOffValveViewModel: ValveViewModel
    {
        protected override void Init(string valveID, Controls.Valve.UpdateTarget target)
        {
            base.Init(valveID, target); 
            if (valveID != null)
            {
                valveStateUpdater = CreateValveStateUpdater(target, this);
                Name = Util.GetValveName(valveID);
            }
            else
            {
                throw new Exception("ValveID is null, cannot set/get valve value from PLC\r\nCheck the SwitchValve ValveID value");
            }
        }

        protected override void OnClicked()
        {
            valveStateUpdater?.OnValveClicked();
        }

        private ValveStateUpdater? valveStateUpdater;
        public string? Name
        {
            get { return (string)GetValue(NameProperty); }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(OnOffValveViewModel), new PropertyMetadata(default));
    }
}
