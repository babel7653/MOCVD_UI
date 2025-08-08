using SapphireXR_App.Common;
using System.Windows;

namespace SapphireXR_App.ViewModels.Valve
{
    public class OnOffValveViewModel: ValveViewModel
    {
        private class ValveLabelUpdater : IObserver<(string, string)>
        {
            public ValveLabelUpdater(OnOffValveViewModel vm)
            {
                onOffValveViewModel = vm;
            }

            void IObserver<(string, string)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, string)>.OnNext((string, string) value)
            {
                if(value.Item1 == onOffValveViewModel.ValveID)
                {
                    onOffValveViewModel.Name = value.Item2;
                }
            }

            private OnOffValveViewModel onOffValveViewModel;
        }

        public OnOffValveViewModel()
        {
            ObservableManager<(string, string)>.Subscribe("ValveIOLabelChanged", valveLabelUpdater = new ValveLabelUpdater(this));
        }

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
        private ValveLabelUpdater valveLabelUpdater;
    }
}
