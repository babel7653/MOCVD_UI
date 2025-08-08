namespace SapphireXR_App.Common
{
    static class ObservableManager<T>
    {
        internal sealed class Unsubscriber : IDisposable
        {
            private IList<IObserver<T>> _observers;
            private IObserver<T> _observer;

            internal Unsubscriber(IList<IObserver<T>> observers, IObserver<T> observer) => (_observers, _observer) = (observers, observer);

            public void Dispose() => _observers.Remove(_observer);
        }

        public class Publisher : IObservable<T>
        {
            public IDisposable Subscribe(IObserver<T> observer)
            {
                observers.Add(observer);
                return new Unsubscriber(observers, observer);
            }

            public void Publish(T data)
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(data);
                }
            }

            private List<IObserver<T>> observers = new List<IObserver<T>>();
        }


        public static Publisher Get(string name)
        {
            Publisher found;
            if (observables.TryGetValue(name, out found!) == false)
            {
                Publisher issuer = new Publisher();
                observables.Add(name, issuer);
                return issuer;
            }
            else
            {
                return found;
            }
        }

        public static IDisposable Subscribe(string name, IObserver<T> observer)
        {
            Publisher found;
            if (observables.TryGetValue(name, out found!) == false)
            {
                found = Get(name);
            }

            return found.Subscribe(observer);
           
        }

        static readonly Dictionary<string, Publisher> observables = new Dictionary<string, Publisher>();
    }
}
