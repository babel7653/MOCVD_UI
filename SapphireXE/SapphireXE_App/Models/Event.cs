using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXE_App.Models
{
  public class Event : ObservableObject
  {
    public string State { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string Checked { get; set; }

  }
}
