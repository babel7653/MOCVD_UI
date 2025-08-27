using System.IO;
using CsvHelper;
using Microsoft.Win32;
using SapphireXR_App.Models;

namespace SapphireXR_App.Common
{
    public static class RecipeService
    {
        internal class MaxValueExceedSubscriber : IObserver<string>
        {
            void IObserver<string>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<string>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<string>.OnNext(string value)
            {
                fcMaxValueExceeded.Add(value);
            }

            public HashSet<string> fcMaxValueExceeded { get; private set; } = new HashSet<string>();
        }

        internal class OpenRecipeFileException : Exception
        {
            internal OpenRecipeFileException(string message) : base(message) { }
        }

        public static (bool, string?, List<Recipe>?, HashSet<string>?) OpenRecipe(CsvHelper.Configuration.CsvConfiguration config, string? initialDirectory)
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

                if (openFile.ShowDialog() != true) return (false, null, null, null);
                string recipeFilePath = openFile.FileName;

                using (StreamReader streamReader = new StreamReader(recipeFilePath))
                {
                    MaxValueExceedSubscriber maxValueExceedSubscriber;
                    using (var csvReader = new CsvReader(streamReader, config))
                    using (var unsubscriber = ObservableManager<string>.Subscribe("Recipe.MaxValueExceed", maxValueExceedSubscriber = new MaxValueExceedSubscriber()))
                    {
                        List<Recipe> recipe = csvReader.GetRecords<Recipe>().ToList();
                        return (true, recipeFilePath, recipe, maxValueExceedSubscriber.fcMaxValueExceeded);
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
