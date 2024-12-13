using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SapphireXR_App.Common
{
    static class ObservableManager<T>
    {
        public class DataIssuerBaseCreateException: Exception
        {
            public DataIssuerBaseCreateException(IObservable<T> alreadyExisted)
            {
                alreadyExistedName = alreadyExisted.GetType().ToString();
            }

            private string alreadyExistedName;
            public override string Message { get { return "The given name is already registered with different Observable Type" + alreadyExistedName;  } }
        }
        internal sealed class Unsubscriber : IDisposable
        {
            private readonly IList<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            internal Unsubscriber(
                IList<IObserver<T>> observers,
                IObserver<T> observer) => (_observers, _observer) = (observers, observer);

            public void Dispose() => _observers.Remove(_observer);
        }

        public class DataIssuerBase : IObservable<T>
        {
            IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
            {
                observers.Add(observer);
                return new Unsubscriber(observers, observer);
            }

            public void Issue(T data)
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(data);
                }
            }

            private List<IObserver<T>> observers = new List<IObserver<T>>();
        }

        public static DataIssuerBase Get(string name)
        {
            IObservable<T> found;
            if (observables.TryGetValue(name, out found!) == false)
            {
                DataIssuerBase issuer = new DataIssuerBase();
                DoRegister(name, issuer);
                return issuer;
            }
            else
            {
                if(found is DataIssuerBase)
                {
                    return (DataIssuerBase)found;
                }
                else
                {
                    throw new DataIssuerBaseCreateException(found);
                }
            }
        }

        private static void DoRegister(string name, IObservable<T> observable)
        {
            observables.Add(name, observable);
            List<IObserver<T>> deferredObservers;
            deferred.TryGetValue(name, out deferredObservers!);
            if (deferredObservers != null)
            {
                foreach (IObserver<T> observer in deferred[name])
                {
                    deferredUnsubscribers.Add(observer, observable.Subscribe(observer));
                }
                deferred[name].Clear();
            }
        }

        public static (bool, IObservable<T>?) Register(string name, IObservable<T> observable)
        {
            if (observable == null)
            {
                return (false, null);
            }
            else
            {
                IObservable<T> found;
                if (observables.TryGetValue(name, out found!) == false)
                {
                    DoRegister(name, observable);

                    return (true, observable);
                }
                else
                {
                    return (false, found);
                }
            }
        }

        public static void Subscribe(string name, IObserver<T> observer)
        {
            IObservable<T> found;
            if (observables.TryGetValue(name, out found!) == true)
            {
                observables[name].Subscribe(observer);
            }
            else
            {
                List<IObserver<T>> deferredObservers;
                if(deferred.TryGetValue(name, out deferredObservers!) == false)
                {
                    deferredObservers = new List<IObserver<T>>();
                    deferred[name] = deferredObservers;
                }
                deferredObservers.Add(observer);
            }
        }

        public static IDisposable? PopUnsubscriber(IObserver<T> observer)
        {
            IDisposable found;
            if (deferredUnsubscribers.TryGetValue(observer, out found!) == true)
            {
                deferredUnsubscribers.Remove(observer);
                return found;
            }
            else
            {
                return null;
            }
        }

        static readonly Dictionary<string, IObservable<T>> observables = new Dictionary<string, IObservable<T>>();
        static readonly Dictionary<string, List<IObserver<T>>> deferred = new Dictionary<string, List<IObserver<T>>>();
        static readonly Dictionary<IObserver<T>, IDisposable> deferredUnsubscribers = new Dictionary<IObserver<T>, IDisposable>();

    }
}
