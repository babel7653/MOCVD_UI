using SapphireXR_App.Models;

namespace SapphireXR_App.Common
{
    internal static class RecipeValidator
    {
        public static (bool, string) Validate(IList<Recipe> recipe)
        {
            if (0 < recipe.Count)
            {
                Recipe first = recipe[0];
                if (first.M01 == null || first.M02 == null || first.M03 == null || first.M04 == null || first.M05 == null || first.M06 == null || first.M07 == null || first.M08 == null ||
                    first.M09 == null || first.M10 == null || first.M11 == null || first.M12 == null || first.M13 == null || first.M14 == null || first.M15 == null || first.M16 == null ||
                    first.M17 == null || first.M18 == null || first.M19 == null || first.E01 == null || first.E02 == null || first.E03 == null || first.E04 == null || first.E05 == null ||
                    first.E06 == null || first.E07 == null || first.RPress == null || first.SRotation == null || first.STemp == null || first.CTemp == null)
                {
                    return (false, "첫 번째 스텝에는 모든 Analaog Device값이 설정되어 있어야 합니다");
                }
            }

            return (true, "");
        }
    }
}
