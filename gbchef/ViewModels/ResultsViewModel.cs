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

namespace gbchef.ViewModels
{
    public class ResultsViewModel
    {
        private readonly ObservableCollection<Recipe> _recipes = new();
        private readonly CollectionViewSource _recipeViewSource;
        private string _searchText = string.Empty;

        public ResultsViewModel()
        {
            _recipeViewSource = new CollectionViewSource { Source = _recipes };
            _recipeViewSource.Filter += RecipeFilter;
        }

        public ICollectionView Recipes => _recipeViewSource.View;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    _recipeViewSource.View.Refresh();
                }
            }
        }

        private void RecipeFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Recipe recipe))
            {
                e.Accepted = false;
                return;
            }

            var selectedIngredients = GetSelectedIngredients();
            e.Accepted = recipe.Ingredients.Any(i => selectedIngredients.Contains(i));
        }

        private HashSet<string> GetSelectedIngredients()
        {
            var vm = (ItemsViewModel)Application.Current.MainWindow.DataContext;
            return new HashSet<string>(
                vm.Vegetables.Where(i => i.IsSelected).Select(i => i.Name)
                .Concat(vm.Fruits.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.Products.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.Foragables.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.Fish.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(vm.Others.Where(i => i.IsSelected).Select(i => i.Name))
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<string> Ingredients { get; set; }
        public string Instructions { get; set; }
    }
}
