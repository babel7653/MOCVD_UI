using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using SapphireXR_App.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel
    {
        public class RecipeContext : ObservableObject, IDisposable
        {
            public RecipeContext() { }
            public RecipeContext(string recipeFilePath, IList<Recipe> recipes)
            {
                RecipeFilePath = recipeFilePath;
                LogFilePath = RecipeFilePath.Replace(".csv", "_log.csv");
                Recipes = recipes;

                try
                {
                    fileStream = new FileStream(LogFilePath, FileMode.Create);
                    streamWriter = new StreamWriter(fileStream);
                    csvWriter = new CsvWriter(streamWriter, Config);
                }
                catch (Exception)
                {
                    csvWriter?.Dispose();
                    streamWriter?.Close();
                    fileStream?.Close();

                    MessageBox.Show(LogFilePath + "을 현재 이용할 수가 없습니다.\r\n해당 파일이 현재 사용중이거나 용량 부족 등이 문제일 수 있습니다.");
                }
                csvWriter!.WriteHeader<RecipeLog>();
                csvWriter!.NextRecord();

                initialized = true;
            }

            ~RecipeContext()
            {
                Dispose(disposing: false);
            }

            public void onStart()
            {
                if (initialized == false)
                {
                    return;
                }

                if (firstStart == true)
                {
                    RecipeService.PLCLoad(Recipes, 0);
                    firstStart = false;
                }
                else
                {
                    PLCService.WriteStart(true);
                    PLCService.WriteOperationState(10);
                }
            }

            public Recipe? markCurrent(short index)
            {
                if (initialized == false)
                {
                    return null;
                }

                if (0 < index)
                {
                    index -= 1;
                    if (index < Recipes.Count)
                    {
                        Recipe next = Recipes[index];
                        if (next != currentRecipe)
                        {
                            if (currentRecipe != null)
                            {
                                currentRecipe.Background = Brushes.White;
                            }
                            currentRecipe = next;
                            currentRecipeIndex = index;
                            currentRecipe.Background = Brushes.LightGoldenrodYellow;

                            return currentRecipe;
                        }
                    }
                }

                return null;
            }

            public void loadPLCSubRangeOfRecipes()
            {
                if(initialized == false)
                {
                    return;
                }

                if (currentRecipe != null)
                {
                    PlcRecipe[] plcRecipes = modifiedRecipeIndice.Where((int recipeIndex) => currentRecipeIndex < recipeIndex).Select((int recipeIndex) => new PlcRecipe(Recipes[recipeIndex])).ToArray();
                    if (0 < plcRecipes.Length)
                    {
                        PLCService.WriteRecipe(plcRecipes);
                        modifiedRecipeIndice.Clear();
                    }
                }
            }

            public void addModfiedRecipe(Recipe recipe)
            {
                if (initialized == false)
                {
                    return;
                }

                int recipeIndex = Recipes.IndexOf(recipe);
                if(recipeIndex != -1)
                {
                    modifiedRecipeIndice.Add(recipeIndex);
                }
            }

            public void log()
            {
                if(initialized == false)
                {
                    return;
                }

                if (currentRecipe != null)
                {
                    csvWriter!.WriteRecord<RecipeLog>(new RecipeLog(currentRecipe));
                    csvWriter!.NextRecord();
                }
            }

            private bool initialized = false;
            private bool firstStart = true;
            private HashSet<int> modifiedRecipeIndice = new HashSet<int>();

            private IList<Recipe> _recipes = new List<Recipe>();
            public IList<Recipe> Recipes
            {
                get { return _recipes; }
                set { SetProperty(ref _recipes, value); }
            }

            private string _recipeFilePath = string.Empty;
            public string RecipeFilePath
            {
                get { return _recipeFilePath; }
                set { SetProperty(ref _recipeFilePath, value); }
            }

            private string _logFilePath = string.Empty;
            public string LogFilePath
            {
                get { return _logFilePath; }
                set { SetProperty(ref _logFilePath, value); }
            }

            public Recipe? currentRecipe = null;
            public int currentRecipeIndex = -1;
            private FileStream? fileStream = null;
            private StreamWriter? streamWriter = null;
            private CsvWriter? csvWriter = null;
            private bool disposedValue = false;

            private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {

                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    DisposeResource();
                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public void DisposeResource()
            {
                if(initialized == false)
                {
                    return;
                }

                csvWriter!.Flush();
                csvWriter!.Dispose();
                streamWriter!.Close();
                fileStream!.Close();
            }
        }
    }
}
