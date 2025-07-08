using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;

namespace SapphireXR_App.Models
{
    internal partial class PLCConnectionState : ObservableObject, IObserver<PLCConnection>
    {
        public static PLCConnectionState Instance { get => Singleton; }
        private static PLCConnectionState Singleton = new PLCConnectionState();

        private PLCConnectionState()
        {
            ObservableManager<PLCConnection>.Subscribe("PLCService.Connected", this);
            Online = PLCService.Connected == PLCConnection.Connected ? true : false;
        }

        [ObservableProperty]
        private bool _online = false;

        void IObserver<PLCConnection>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCConnection>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCConnection>.OnNext(PLCConnection value)
        {
            Online = value == PLCConnection.Connected ? true : false;
        }
    }
}
