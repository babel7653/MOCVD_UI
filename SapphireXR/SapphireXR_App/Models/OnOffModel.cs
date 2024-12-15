using SapphireXR_App.Common;

namespace SapphireXR_App.Models
{
    public class OnOffModel : IObserver<bool>
    {
        public OnOffModel(string vid)
        {
            try
            {
                dataIssuer = ObservableManager<bool>.Get(vid + ".IsOpen.Read");
            }
            catch (ObservableManager<bool>.DataIssuerBaseCreateException)
            {

            }
            ObservableManager<bool>.Subscribe(vid + ".IsOpen.Write", this);
        }

        private ObservableManager<bool>.DataIssuerBase? dataIssuer;

        //PLC에서 Bool값을 읽어와서 이 메소드를 호출하면, UI가 변경 됨.
        public void ReadValue(bool value)
        {
            dataIssuer?.Issue(value);
        }

        void IObserver<bool>.OnCompleted() { }

        void IObserver<bool>.OnError(Exception error) { }

        //UI에서 IsOpen 값을 변경하면 이 메소드가 호출 됨.
        void IObserver<bool>.OnNext(bool value)
        {
            //여기에 PLC로 값을 쓰는 코드를 넣음.
        }
    }
}
