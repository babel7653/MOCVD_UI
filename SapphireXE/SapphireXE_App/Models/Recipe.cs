using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXE_App.Models
{
	class Recipe
	{
    public int RecipeStep { get; set; }
    public string RecipeName { get; set; }
    public int RampingTime { get; set; }
    public int HoldingTime { get; set; }
    public bool RecipeLoop { get; set; }
    public bool RecipeJump { get; set; }

  }
}
