using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXE_App.Models
{
  public class RecipeTest : ObservableObject
  {
    public int[] IntArr { get; set; } = new int[9];
    public float[] RealArr { get; set; } = new float[26];
    public bool[] BoolArr { get; set; } = new bool[26];
  }
}
