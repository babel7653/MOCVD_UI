using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SapphireXE_App.Models;
using TwinCAT.Ads;

namespace SapphireXE_App.ViewModels
{
  public partial class MainViewModel : ViewModelBase
  {
    public List<int> tbInt { get; set; }
    public List<float> tbReal { get; set; }
    public List<bool> tbBool { get; set; }

    public void WriteRecipeVariable()
    {
      uint[] hPlcArray = { 0, 0, 0, 0, 0, 0, 0 };
      bool[] tcPlcArray = { true, true, false, true, true, true, true };
      uint[] hRcpInt = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      Int16[] tcRcpInt = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      uint[] hRcpReal = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      float[] tcRcpReal = { 100, 100, 100, 100, 100, 100, 100, 100, 100 };
      uint[] hRcpBool = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      bool[] tcRcpBool = { true, true, true, true, true, true, true, true };

      try
      {
        for (int i = 0; i < tcPlcArray.Length; i++)
        {
          hPlcArray[i] = tcClient.CreateVariableHandle($"MAIN.plc_array2[{i}]");
          tcClient.WriteAny(hPlcArray[i], tcPlcArray[i]);
          Console.WriteLine($"tcPlcArray[{i}] = {tcPlcArray[i]}");
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }

      try
      {
        for (int i = 0; i < hRcpInt.Length; i++)
        {
          hRcpInt[i] = tcClient.CreateVariableHandle($"GVL.gArray1.IntArr[{i}]");
          tcClient.WriteAny(hRcpInt[i], tcRcpInt[i]);
          Console.WriteLine($"GVL.gArray1.IntArr[{i}] = {tcRcpInt[i]}");
        }
        for (int i = 0; i < hRcpReal.Length; i++)
        {
          hRcpReal[i] = tcClient.CreateVariableHandle($"GVL.gArray1.RealArr[{i}]");
          tcClient.WriteAny(hRcpReal[i], tcRcpReal[i]);
          Console.WriteLine($"GVL.gArray1.RealArr[{i}] = {tcRcpReal[i]}");
        }
        for (int i = 0; i < hRcpBool.Length; i++)
        {
          hRcpBool[i] = tcClient.CreateVariableHandle($"GVL.gArray1.BoolArr[{i}]");
          tcClient.WriteAny(hRcpBool[i], tcRcpBool[i]);
          Console.WriteLine($"GVL.gArray1.BoolArr[{i}] = {tcRcpBool[i]}");
        }

      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }

    }

    // Recipe variable reading
    public void ReadRecipeVariable()
    {
      uint[] hPlcArray = { 0, 0, 0, 0, 0, 0, 0 };
      bool[] tcPlcArray = { false, false, false, false, false, false, false };
      uint[] hRcpInt = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      int[] tcRcpInt = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      uint[] hRcpReal = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      float[] tcRcpReal = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      uint[] hRcpBool = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      bool[] tcRcpBool = { false, false, false, false, false, false, false, false };

      try
      {
        for (int i = 0; i < tcPlcArray.Length; i++)
        {
          hPlcArray[i] = tcClient.CreateVariableHandle($"MAIN.plc_array[{i}]");
          tcPlcArray[i] = (bool)tcClient.ReadAny(hPlcArray[i], typeof(bool));
          Console.WriteLine($"tcPlcArray[{i}] = {tcPlcArray[i]}");
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
      try
      {
        for (int i = 0; i < hRcpInt.Length; i++)
        {
          hRcpInt[i] = tcClient.CreateVariableHandle($"GVL.gArray1.IntArr[{i}]");
          tcRcpInt[i] = (int)tcClient.ReadAny(hRcpInt[i], typeof(int));
          Console.WriteLine($"tcRecipe[{i}] = {tcRcpInt[i]}");
        }
        for (int i = 0; i < hRcpReal.Length; i++)
        {
          hRcpReal[i] = tcClient.CreateVariableHandle($"GVL.gArray1.RealArr[{i}]");
          tcRcpReal[i] = (float)tcClient.ReadAny(hRcpReal[i], typeof(float));
          Console.WriteLine($"tcRecipe[{i}] = {tcRcpReal[i]}");
        }
        for (int i = 0; i < hRcpBool.Length; i++)
        {
          hRcpBool[i] = tcClient.CreateVariableHandle($"GVL.gArray1.BoolArr[{i}]");
          tcRcpBool[i] = (bool)tcClient.ReadAny(hRcpBool[i], typeof(bool));
          Console.WriteLine($"tcRecipe[{i}] = {tcRcpBool[i]}");
        }

      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }


  }
}
