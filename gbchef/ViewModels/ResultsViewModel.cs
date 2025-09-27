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
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class ResultsViewModel
    {
        public ObservableCollection<Recipe> Recipes { get; } = new();
        private string _searchText = string.Empty;

        public ResultsViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes) {
                Recipes.Add(recipe);
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
}
