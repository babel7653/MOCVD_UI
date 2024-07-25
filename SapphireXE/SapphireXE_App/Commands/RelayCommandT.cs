using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SapphireXE_App.Commands
{
  public class RelayCommandT<T> : ICommand
  {
    private readonly Action<T>? _execute;
    private readonly Predicate<T>? _canExecute;

    public RelayCommandT(Action<T>? execute, Predicate<T>? canExecute = null)
    {
      this._execute = execute;
      this._canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
      return _canExecute?.Invoke((T)parameter!) ?? true;
    }

    public void Execute(object? parameter)
    {
      _execute?.Invoke((T)parameter!);
    }

  }
}
