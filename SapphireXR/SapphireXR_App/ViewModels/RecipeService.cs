using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public static class RecipeService
    {
        internal class OpenRecipeFileException: Exception
        {
            internal OpenRecipeFileException(string  message) : base(message) { }    
        }

        public static (bool, string?, List<Recipe>?) OpenRecipe(CsvHelper.Configuration.CsvConfiguration config)
        {
            try
            {
                OpenFileDialog openFile = new();
                openFile.Multiselect = false;
                openFile.Filter = "csv 파일(*.csv)|*.csv";
                string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                int path_length = appBasePath.Length;
                openFile.InitialDirectory = appBasePath.Substring(0, path_length - 25) + "Data\\Recipes\\";

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
            catch(Exception exception)
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
