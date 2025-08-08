using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace SapphireXR_App.Models
{
    public class RecipeObservableCollection : ObservableCollection<Recipe>
    {
        public RecipeObservableCollection() { }

        public RecipeObservableCollection(IEnumerable<Recipe> rhs) : base(rhs) { }
        public RecipeObservableCollection(RecipeObservableCollection rhs): base(rhs) { }

        public RecipeObservableCollection(List<Recipe> items) : base(items) { }
        public IList<Recipe> CopyInsertRange(int index, IEnumerable<Recipe> items)
        {
            CheckReentrancy();
            IList<Recipe> newlyAdded = new List<Recipe>();
            int insert = index;
            foreach (var recipe in items)
            {
                Recipe added = new Recipe(recipe);
                Items.Insert(insert++, added);
                newlyAdded.Add(added);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            return newlyAdded;
        }

        public void RemoveAt(IList recipe)
        {
            CheckReentrancy();
            foreach(object item in recipe)
            {
                Items.Remove((Recipe)item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Insert(int index, Recipe recipe)
        {
            CheckReentrancy();
            Items.Insert(index, recipe);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, recipe, index));
        }
    }
}
