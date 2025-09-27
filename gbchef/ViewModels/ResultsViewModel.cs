using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class ResultsViewModel
    {

        public ObservableCollection<Recipe> Recipes { get; } = new();
        private HashSet<string> _selectedIngredients = new();
        private readonly ItemsViewModel _itemsViewModel;

        public ResultsViewModel(IEnumerable<Recipe> recipes, ItemsViewModel itemsViewModel)
        {
            foreach (var recipe in recipes) {
                Recipes.Add(recipe);
            }

            // setup ingredient change handlers
            _itemsViewModel = itemsViewModel;
            _itemsViewModel.PropertyChanged += HandleIngredientChange;

        }

        private void HandleIngredientChange(object sender, PropertyChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Recipes).Refresh();
        }

        public void RecipeFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Recipe recipe)) {
                e.Accepted = false;
                return;
            }
            var selectedIngredients = GetSelectedIngredients();

            // Check if all recipe ingredients are in the selected set
            var recipeIngredients = new[]
            {
                recipe.Ingredient1,
                recipe.Ingredient2,
                recipe.Ingredient3,
                recipe.Ingredient4
            };

            // Only accept recipes where ALL ingredients are selected
            e.Accepted = recipeIngredients.All(ingredient =>
                string.IsNullOrEmpty(ingredient) ||
                selectedIngredients.Contains(ingredient));
        }

        private HashSet<string> GetSelectedIngredients()
        {
            var vm = (CompositeViewModel)Application.Current.MainWindow.DataContext;
            return new HashSet<string>(
                vm.ItemsVM.Vegetables.Where(i => i.IsSelected).Select(i => i.Name)
                .Concat(vm.ItemsVM.Fruits.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.ItemsVM.Products.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.ItemsVM.Foragables.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.ItemsVM.Fish.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.ItemsVM.Others.Where(i => i.IsSelected).Select(i => i.Name))
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
