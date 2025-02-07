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
        public static (bool, string?, List<Recipe>?) OpenRecipe(CsvHelper.Configuration.CsvConfiguration config)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.Filter = "csv 파일(*.csv)|*.csv";
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            int path_length = appBasePath.Length;
            openFile.InitialDirectory = appBasePath.Substring(0, path_length - 25) + "Data\\Recipes\\";

            if (openFile.ShowDialog() != true) return ( false, null, null );
            string recipeFilePath = openFile.FileName;

            using (StreamReader streamReader = new StreamReader(recipeFilePath))
            {
                using (var csvReader = new CsvReader(streamReader, config))
                {
                    return (true, recipeFilePath, csvReader.GetRecords<Recipe>().ToList());
                }
            }
        }

        public static void PLCLoad(IList<Recipe> recipes, short operationState)
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
                //PLCService.WriteStart(true);
                PLCService.WriteOperationState(operationState);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
