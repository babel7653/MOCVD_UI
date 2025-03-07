namespace SapphireXR_App.ViewModels.Valve
{
    public class OnOffValveViewModel: ValveViewModel
    {
        protected override void Init(string valveID, SapphireXR_App.Controls.Valve.UpdateTarget target)
        {
            base.Init(valveID, target); 
            if (valveID != null)
            {
                valveStateUpdater = CreateValveStateUpdater(target, this);
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
    }
}
