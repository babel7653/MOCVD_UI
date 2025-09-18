using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeEditViewModel : ViewModelBase
    {
        public partial class TabDataGridViewModel: ObservableObject
        {
            public enum State {  NoneSelected = 0, Selected, Copied };

            public TabDataGridViewModel(RecipeEditViewModel upperLevelViewModel)
            {
                Selected = null;
                CurrentState = State.NoneSelected;
                RecipeViewModel = upperLevelViewModel;
            }

            public void reset()
            {
                deselect();
            }
            private void deselect()
            {
                CurrentState = State.NoneSelected;
                Selected = null;
                copied = null;
                RecipeViewModel.cleanupNewlyAdded();
            }

            private void select(IList selected)
            {
                RecipeViewModel.cleanupNewlyAdded();
                Selected = selected;

                if (0 < Selected.Count && 0 < selected.Count)
                {
                    if (CurrentState == State.NoneSelected)
                    {
                        CurrentState = State.Selected;
                    }
                }
                else
                {
                    deselect();
                }
            }

            private void setCopied(IList toCopy)
            {
                if (CurrentState != State.NoneSelected)
                {
                    copied = new List<Recipe>();
                    List<object> noneRecipes = new List<object>();
                    foreach(object item in toCopy)
                    {
                        if(item is Recipe)
                        { 
                            copied.Add((Recipe)item);
                        }
                        else
                        {
                            noneRecipes.Add(item);
                        }
                    }
                    CurrentState = State.Copied;

                    if (0 < noneRecipes.Count)
                    {
                        throw new Exception("There is none recipe item of selected items.\r\n" + noneRecipes.ToString());
                    }
                }
            }

            private Recipe insert(int index)
            {
                Recipe added = new Recipe() { RTime = 1, HTime = 1 };
                RecipeViewModel.Recipes!.Insert(index, added);
                recipeAddedPublishser.Publish(new List<Recipe>() { added });
                added.Foreground = Brushes.LightPink;
                RecipeViewModel.newlyAddedForMarking.Add(added);

                return added;
            }

            static private int selectedIndex(IList selected)
            {
                return ((Recipe)selected[selected.Count - 1]!).No - 1;
            }

            [ObservableProperty]
            private State _currentState;
            [ObservableProperty]
            private IList? _selected;
            private IList<Recipe>? copied = null;
            public RecipeEditViewModel RecipeViewModel { get; set; }
            private ObservableManager<IList<Recipe>>.Publisher recipeAddedPublishser = ObservableManager<IList<Recipe>>.Get("RecipeEdit.TabDataGrid.RecipeAdded");

            public IRelayCommand SelectionChangedCommand => new RelayCommand<object?>((object? args) =>
            {
                DataGrid? dataGrid = (args as SelectionChangedEventArgs)?.Source as DataGrid;
                if (dataGrid != null)
                {
                    foreach(object item in dataGrid.SelectedItems)
                    {
                        if(item is not Recipe)
                        {
                            deselect();
                            dataGrid.Dispatcher.InvokeAsync(() =>
                            {
                                dataGrid.UpdateLayout();
                                dataGrid.UnselectAll();
                            });
                            
                            return;
                        }
                    }
                    select(dataGrid.SelectedItems);
                    if (0 < dataGrid.SelectedItems.Count)
                    {
                        Recipe? selected = dataGrid.SelectedItems[0] as Recipe;
                        if (selected != null)
                        {
                            RecipeViewModel.onRecipeSelected(selected);
                        }
                    }
                    else
                    {
                        RecipeViewModel.onRecipeSelected(null);
                    }
                }
            });
            public IRelayCommand CopyCommand => new RelayCommand(() => {
                setCopied(Selected!);
            });
            public IRelayCommand PasteCommand => new RelayCommand(() =>
            {
                if(CurrentState == State.Copied)
                {
                    if (RecipeViewModel.Recipes != null)
                    {
                        int insert = selectedIndex(Selected!);
                        int copyCount = copied!.Count;
                        
                        IList<Recipe> added = RecipeViewModel.Recipes.CopyInsertRange(insert, copied!);
                        foreach(var recipe in added)
                        {
                            recipe.Foreground = Brushes.LightPink;
                        }
                        recipeAddedPublishser.Publish(added);

                        RecipeViewModel.newlyAddedForMarking.AddRange(added);
                    }
                }
            });
            public IRelayCommand DeleteStepCommand => new RelayCommand(() =>
            {
                if(State.Selected <= CurrentState)
                {
                    RecipeViewModel.Recipes!.RemoveAt((IList)this.Selected!);
                }
            });
            public IRelayCommand InsertStepCommand => new RelayCommand(() =>
            {
                if (State.Selected <= CurrentState)
                {
                    insert(selectedIndex(Selected!));
                }
            });
            public IRelayCommand AddStepCommand => new RelayCommand<object?>((object? args) =>
            {
                if (RecipeViewModel.Recipes != null)
                {
                    Recipe recipe = insert(RecipeViewModel.Recipes.Count);

                    DataGrid? dataGrid = args as DataGrid;
                    if (dataGrid != null)
                    {
                        dataGrid.Dispatcher.InvokeAsync(() =>
                        {
                            dataGrid.UpdateLayout();
                            dataGrid.ScrollIntoView(recipe);
                        });
                    }
                }
                else
                {
                    throw new Exception("TabDataGridViewModel in RecipeViewModel: Add Step is executed without initializing RecipeViewModel.Recipes");
                }
            });
        }
    }
}
