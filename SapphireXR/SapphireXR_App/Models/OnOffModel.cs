using SapphireXR_App.Common;
using static SapphireXR_App.Models.PLCService;

namespace SapphireXR_App.Models
{
    public class OnOffModel : IObserver<bool>
    {
        public OnOffModel(string vid)
        {
            ValveID = vid;
            dataIssuer = ObservableManager<bool>.Get(vid + ".IsOpen.Read");
            ObservableManager<bool>.Subscribe(vid + ".IsOpen.Write", this);
            try
            {
                dataIssuer.Issue(PLCService.ReadValveState(vid));
            }
            catch (PLCService.ReadValveStateException)
            {
            }
        }

        private ObservableManager<bool>.DataIssuer dataIssuer;

        void IObserver<bool>.OnCompleted() { }

        void IObserver<bool>.OnError(Exception error) { }

        //UI에서 IsOpen 값을 변경하면 이 메소드가 호출 됨.
        void IObserver<bool>.OnNext(bool value)
        {
            PLCService.WriteValveState(ValveID, value);
        }

        public string ValveID {  get; set; }
    }
}
