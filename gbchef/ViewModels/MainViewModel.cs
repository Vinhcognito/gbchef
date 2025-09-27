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
        public ObservableCollection<SelectableIngredient> Vegetables { get; set; } = new()
        {
            new SelectableIngredient("Radish",false),
            new SelectableIngredient("Tomato",false),
            new SelectableIngredient("Corn",false),
            new SelectableIngredient("Cucumber",false),
            new SelectableIngredient("Onion",false),
            new SelectableIngredient("Carrot",false),
            new SelectableIngredient("Avocado",false),
            new SelectableIngredient("1",false),
            new SelectableIngredient("2",false),
            new SelectableIngredient("3",false),
            new SelectableIngredient("4",false),
            new SelectableIngredient("5",false),

        };

        public ObservableCollection<SelectableIngredient> Fruits { get; } = new()
        {
            new SelectableIngredient("Apple", false),
            new SelectableIngredient("Orange", false),
            new SelectableIngredient("Peach", false),
        };

        public ObservableCollection<SelectableIngredient> Products { get; } = new()
        {
            new SelectableIngredient("Cheese", false),
            new SelectableIngredient("Milk", false),
            new SelectableIngredient("Butter", false),
            new SelectableIngredient("Mayonnaise", false),
        };

        public ObservableCollection<SelectableIngredient> Foragables { get; } = new()
        {
            new SelectableIngredient("Shiitake Mushroom", false),
            new SelectableIngredient("Honey", false),
            new SelectableIngredient("Enoki", false),
        };

        public ObservableCollection<SelectableIngredient> Fish { get; } = new()
        {
            new SelectableIngredient("Salmon", false),
            new SelectableIngredient("Yellow Perch", false),
            new SelectableIngredient("Snakehead", false),
        };

        public ObservableCollection<SelectableIngredient> Others { get; } = new()
        {
            new SelectableIngredient("Rice Flour", false),
            new SelectableIngredient("Salt", false),
            new SelectableIngredient("Oil", false),
        };


        private ObservableCollection<Recipe> _recipes { get; } = new();

        public CollectionViewSource ViewSource { get; } = new();

        public MainViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes) {
                recipe.PropertyChanged += HandleRecipeChanged;

                _recipes.Add(recipe);

            }

            ViewSource.Source = recipes;

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            ViewSource.View.Filter = item =>
            {
                var recipe = item as Recipe;
                return recipe.IsSatisfied;
            };
            ViewSource.View.Refresh();
        }

        private void HandleRecipeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Recipe)
            {
                throw new InvalidOperationException("Handle Recipes only.");
            }

            ApplyFilter();
        }

    }

}
