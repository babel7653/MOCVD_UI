using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.Models
{
  public class FlowSetData : ObservableObject
  {
    public string? Name { get; set; }
    public int TargetVal { get; set; }
    public int RampTime { get; set; }
    public int Deviation { get; set; }
    public int CurVal { get; set; }
    public int ConVal { get; set; }
    public int MaxVal { get; set; }
  }
}
