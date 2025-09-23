using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Models;

namespace SapphireXR_App.Common
{
    internal static class RecipeValidator
    {
        public static (bool, string) ValidOnLoadedFromDisk(IList<Recipe> recipe)
        {
            if (0 < recipe.Count)
            {
                Recipe first = recipe[0];
                if (Valid(first) == false)
                {
                    return (false, "첫 번째 스텝에는 모든 Analaog Device값이 설정되어 있어야 합니다");
                }
            }

            return (true, "");
        }

        internal static bool Valid(Recipe recipe)
        {
            return !(recipe.M01 == null || recipe.M02 == null || recipe.M03 == null || recipe.M04 == null || recipe.M05 == null || recipe.M06 == null || recipe.M07 == null || recipe.M08 == null ||
                    recipe.M09 == null || recipe.M10 == null || recipe.M11 == null || recipe.M12 == null || recipe.M13 == null || recipe.M14 == null || recipe.M15 == null || recipe.M16 == null ||
                    recipe.M17 == null || recipe.M18 == null || recipe.M19 == null || recipe.E01 == null || recipe.E02 == null || recipe.E03 == null || recipe.E04 == null || recipe.E05 == null ||
                    recipe.E06 == null || recipe.E07 == null || recipe.RPress == null || recipe.SRotation == null || recipe.STemp == null || recipe.CTemp == null);
        }

        internal static bool Valid(IList<Recipe> recipes)
        {
            RecipeStepValidator? firstStepValidator = recipes.First().stepValidator;
            if (firstStepValidator != null)
            {
                return firstStepValidator.Valid;
            }
            else
            {
                throw new InvalidOperationException("첫번째 스텝의 RecipeStepValidator이 설정되지 안았습니다");
            }
        }
    }

    internal abstract partial class RecipeStepValidator: ObservableObject
    {
        internal RecipeStepValidator(bool initalValid)
        {
            valid = initalValid;
        }
        internal abstract string validate(Recipe recipe, string analogController);

        private bool valid;
        public bool Valid
        {
            get { return valid; }
            protected set { SetProperty(ref valid, value); }
        }
    }

    internal class FirstRecipeStepValidator: RecipeStepValidator
    {
        internal FirstRecipeStepValidator(bool initalValid) : base(initalValid) { }
        internal override string validate(Recipe recipe, string analogController)
        {
            string errorMessage = "첫번째 Step의 값은 빈값일 수 없습니다";
            switch (analogController)
            {
                case nameof(Recipe.M01):
                    if (recipe.M01 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M02):
                    if (recipe.M02 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M03):
                    if (recipe.M03 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M04):
                    if (recipe.M04 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M05):
                    if (recipe.M05 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M06):
                    if (recipe.M06 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M07):
                    if (recipe.M07 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M08):
                    if (recipe.M08 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M09):
                    if (recipe.M09 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M10):
                    if (recipe.M10 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M11):
                    if (recipe.M11 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M12):
                    if (recipe.M12 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M13):
                    if (recipe.M13 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M14):
                    if (recipe.M14 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M15):
                    if (recipe.M15 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M16):
                    if (recipe.M16 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M17):
                    if (recipe.M17 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M18):
                    if (recipe.M18 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.M19):
                    if (recipe.M19 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E01):
                    if (recipe.E01 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E02):
                    if (recipe.E02 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E03):
                    if (recipe.E03 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E04):
                    if (recipe.E04 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E05):
                    if (recipe.E05 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E06):
                    if (recipe.E06 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.E07):
                    if (recipe.E07 == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.STemp):
                    if (recipe.STemp == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.CTemp):
                    if (recipe.CTemp == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.RPress):
                    if (recipe.RPress == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;

                case nameof(Recipe.SRotation):
                    if (recipe.SRotation == null)
                    {
                        Valid = false;
                        return errorMessage;
                    }
                    else
                    {
                        Valid = RecipeValidator.Valid(recipe);
                    }
                    break;
            }

            return string.Empty;
        }
    }

    internal class NormalRecipeStepValidator : RecipeStepValidator
    {
        internal NormalRecipeStepValidator() : base(true) { }

        internal override string validate(Recipe recipe, string analogController)
        {
            return string.Empty;
        }
    }
}
