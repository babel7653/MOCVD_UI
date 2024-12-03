using System.ComponentModel;
using System.Runtime.CompilerServices;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
  public class ViewModelBase : INotifyPropertyChanged
	{
    public static List<GasDO>? S_GasDOs;

    public event PropertyChangedEventHandler? PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
		{
			if (object.Equals(storage, value)) return false;

			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

	}
}
