using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXE_App.Models
{
  public class DeviceE : ObservableObject
  {
    public string ID { get; set; }
    public string Event { get; set; }
    public bool Alarm { get; set; }
    public bool Warning { get; set; }
    public double Delay { get; set; }
  }
}
