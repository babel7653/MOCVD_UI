using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SapphireXE_App.Commands
{
	public class RelayCommand : ICommand
	{
    private readonly Action _execute;

    public RelayCommand(Action execute)
    {
      this._execute = execute;
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      _execute?.Invoke();
    }
  }
}
