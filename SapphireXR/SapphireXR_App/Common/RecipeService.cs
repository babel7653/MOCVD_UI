using System.IO;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Models;

namespace SapphireXR_App.Common
{
    public static class RecipeService
    {
        internal class OpenRecipeFileException : Exception
        {
            internal OpenRecipeFileException(string message) : base(message) { }
        }

        public static (bool, string?, List<Recipe>?) OpenRecipe(CsvHelper.Configuration.CsvConfiguration config, string? initialDirectory)
        {
            try
            {
                OpenFileDialog openFile = new();
                openFile.Multiselect = false;
                openFile.Filter = "csv 파일(*.csv)|*.csv";

                if(Path.Exists(initialDirectory) == false)
                {
                    initialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Recipe";
                    if (Path.Exists(initialDirectory) == false)
                    {
                        Directory.CreateDirectory(initialDirectory);
                    }
                }
                openFile.InitialDirectory = initialDirectory;

                if (openFile.ShowDialog() != true) return (false, null, null);
                string recipeFilePath = openFile.FileName;

                using (StreamReader streamReader = new StreamReader(recipeFilePath))
                {
                    using (var csvReader = new CsvReader(streamReader, config))
                    {
                        return (true, recipeFilePath, csvReader.GetRecords<Recipe>().ToList());
                    }
                }
            }
            catch (Exception exception)
            {
                throw new OpenRecipeFileException(exception.Message);
            }
        }

        public static void PLCLoad(IList<Recipe> recipes)
        {
            try
            {
                PlcRecipe[] aRecipePLC = new PlcRecipe[recipes.Count];
                int i = 0;
                foreach (Recipe iRecipeRow in recipes)
                {
                    aRecipePLC[i] = new PlcRecipe(iRecipeRow);
                    i += 1;
                };
                PLCService.WriteRecipe(aRecipePLC);
                PLCService.WriteTotalStep((short)aRecipePLC.Length);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
