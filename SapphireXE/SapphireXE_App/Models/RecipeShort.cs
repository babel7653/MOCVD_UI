using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXE_App.Models
{
  class RecipeShort : ObservableObject
  {
    public bool V01 { get; set; }
    public bool V02 { get; set; }
    public bool V03 { get; set; }
    public bool V04 { get; set; }
    public bool V05 { get; set; }
    public bool V07 { get; set; }
    public bool V08 { get; set; }

  }
}   // 데이터 전송 테스트를 위해 만든 class. 테스트후 삭제할 것.
