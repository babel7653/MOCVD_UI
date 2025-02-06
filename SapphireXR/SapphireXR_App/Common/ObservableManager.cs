using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using SapphireXR_App.Models;

namespace SapphireXR_App.Common
{
    static class ObservableManager<T>
    {
        internal sealed class Unsubscriber : IDisposable
        {
            private IList<IObserver<T>> _observers;
            private IObserver<T> _observer;

            internal Unsubscriber(
                IList<IObserver<T>> observers,
                IObserver<T> observer) => (_observers, _observer) = (observers, observer);

            public void Dispose() =>_observers.Remove(_observer);
        }

        public class DataIssuer : IObservable<T>
        {
            public IDisposable Subscribe(IObserver<T> observer)
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


        public static DataIssuer Get(string name)
        {
            DataIssuer found;
            if (observables.TryGetValue(name, out found!) == false)
            {
                DataIssuer issuer = new DataIssuer();
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
            DataIssuer found;
            if (observables.TryGetValue(name, out found!) == false)
            {
                found = Get(name);
            }

            return found.Subscribe(observer);
           
        }

        static readonly Dictionary<string, DataIssuer> observables = new Dictionary<string, DataIssuer>();
    }
}
