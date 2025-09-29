using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class MainViewModel
    {
        public bool? ShowPartiallySatisfiedRecipes = false;
        public bool? ShowAllRecipes = false;
        public bool? ShowAutoSelected = false;

        public ObservableCollection<SelectableIngredient> Vegetables { get; set; }
        public ObservableCollection<SelectableIngredient> Fruits { get; set; }
        public ObservableCollection<SelectableIngredient> Products { get; set; }
        public ObservableCollection<SelectableIngredient> Forageables { get; set; }
        public ObservableCollection<SelectableIngredient> Fish { get; set; }
        public ObservableCollection<SelectableIngredient> Others { get; set; }
        // This is recipes that are also an ingredient
        public ObservableCollection<SelectableIngredient> Recipes { get; set; }

        public CollectionViewSource ViewSource { get; } = new();

        public MainViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes) {
                recipe.PropertyChanged += HandleRecipeChanged;
            }

            ViewSource.Source = recipes;

            ApplyFilter();
        }

        public void ApplyFilter()
        {
            ViewSource.View.Filter = item =>
            {
                if (ShowAllRecipes == true)
                {
                    return true;
                }
                var recipe = item as Recipe;
                var show = ShowPartiallySatisfiedRecipes == true ? recipe.IsPartiallySatisfied : recipe.IsSatisfied;
                UpdateAutoSelection(recipe, show);
                return show;
            };

            ViewSource.View.Refresh();
        }

        private void UpdateAutoSelection(Recipe? recipe, bool show)
        {
            var recipeAsIngredient = recipe?.AsIngredient;
            if (recipeAsIngredient != null) {
                recipeAsIngredient.IsAutoSelected = ShowAutoSelected == true && show;
            }
        }

        private void HandleRecipeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Recipe)
            {
                throw new InvalidOperationException("Handle Recipes only.");
            }
            
            ApplyFilter();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
