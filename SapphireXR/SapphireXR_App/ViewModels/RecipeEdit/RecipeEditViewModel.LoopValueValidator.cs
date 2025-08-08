using SapphireXR_App.Bases;
using SapphireXR_App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SapphireXR_App.ViewModels.RecipeEdit
{
    public partial class RecipeEditViewModel : ViewModelBase
    {
        internal class LoopValueValidator
        {
            internal LoopValueValidator(ObservableCollection<Recipe> recipes) 
            {
                foreach (var recipe in recipes)
                {
                    recipe.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
                    {
                        switch(args.PropertyName)
                        {
                            case nameof(Recipe.LoopEndStep):
                                short jumpIndex = (short)(recipe.LoopEndStep - 1);
                                short curIndex = (short)recipes.IndexOf(recipe);
                                bool validJumpValue = true;
                                if (curIndex < jumpIndex && jumpIndex < recipes.Count)
                                {
                                    for (short rIndex = (short)(curIndex + 1); rIndex <= jumpIndex; ++rIndex)
                                    {
                                        if (recipes[rIndex].LoopEndStep != 0)
                                        {
                                            validJumpValue = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    validJumpValue = false;
                                }
                                if (validJumpValue == false)
                                {

                                }
                                break;
                        }
                    };
                }
            }
        }
    }
}
