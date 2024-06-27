using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphireXE_App.ViewModels;
using SapphireXE_App.Views;

namespace SapphireXE_App.Models
{
  class RecipeData
  {
    public List<Recipe> recipeData = new List<Recipe>();

    public RecipeData()
    {
      //래시피 DataGrid 데이터 내용
      recipeData.Add(new Recipe { RecipeStep = 1, RecipeName = "Evacuation", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 2, RecipeName = "N2 Filling", RampingTime = 90, HoldingTime = 10, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 3, RecipeName = "Evacuation", RampingTime = 90, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 4, RecipeName = "H2 Flow Set", RampingTime = 30, HoldingTime = 10, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 5, RecipeName = "Temp Up Pre", RampingTime = 60, HoldingTime = 10, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 6, RecipeName = "Temp Up to T Etching 1", RampingTime = 250, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 7, RecipeName = "Temp Up to T Etching 2", RampingTime = 60, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 8, RecipeName = "Thermal Etching", RampingTime = 1, HoldingTime = 300, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 9, RecipeName = "Temp Dowin to Buffer", RampingTime = 200, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 10, RecipeName = "Wait to Stable", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 11, RecipeName = "Thermal Etching", RampingTime = 1, HoldingTime = 300, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 12, RecipeName = "Temp Dowin to Buffer", RampingTime = 200, HoldingTime = 1, RecipeLoop = false, RecipeJump = false });
      recipeData.Add(new Recipe { RecipeStep = 13, RecipeName = "Wait to Stable", RampingTime = 180, HoldingTime = 30, RecipeLoop = false, RecipeJump = false });

    }
  }
}
