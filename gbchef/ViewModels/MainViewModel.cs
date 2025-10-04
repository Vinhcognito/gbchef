using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class MainViewModel
    {
        public bool? ShowPartiallySatisfiedRecipes = true;
        public bool? ShowAllRecipes = false;
        public bool? ShowAutoSelected = true;

        public ObservableCollection<SelectableIngredient> Vegetables { get; } = [];
        public ObservableCollection<SelectableIngredient> Fruits { get; } = [];
        public ObservableCollection<SelectableIngredient> Products { get; } = [];
        public ObservableCollection<SelectableIngredient> Forageables { get; } = [];
        public ObservableCollection<SelectableIngredient> Fish { get; } = [];
        public ObservableCollection<SelectableIngredient> Others { get; } = [];
        // This is recipes that are also an ingredient
        public ObservableCollection<SelectableIngredient> Recipes { get; } = [];

        public IEnumerable<string> Effects { get; set; }
        public ObservableCollection<string> SelectedEffects { get; set; }
        public IEnumerable<string> Unlocks { get; set; }
        public ObservableCollection<string> SelectedUnlocks { get; set; }
        public IEnumerable<string> Festivals { get; set; }
        public ObservableCollection<string> SelectedFestivals { get; set; }

        public CollectionViewSource ViewSource { get; } = new();

        public MainViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                recipe.PropertyChanged += HandleRecipeChanged;
            }

            Effects = [.. recipes.Where(r => !string.IsNullOrWhiteSpace(r.Effect))
                .Select(r => r.Effect).Distinct().Order()];
            SelectedEffects = new ObservableCollection<string>(Effects);
            SelectedEffects.CollectionChanged += HandleOtherFilters;

            Unlocks = [.. recipes.Where(r => !string.IsNullOrWhiteSpace(r.Unlock))
                .Select(r => r.Unlock).Distinct().Order()];
            SelectedUnlocks = new ObservableCollection<string>(Unlocks);
            SelectedUnlocks.CollectionChanged += HandleOtherFilters;

            Festivals = [.. recipes.Where(r => !string.IsNullOrWhiteSpace(r.Festival))
                .Select(r => r.Festival).Distinct().Order()];
            SelectedFestivals = new ObservableCollection<string>(Festivals);
            SelectedFestivals.CollectionChanged += HandleOtherFilters;

            ViewSource.Source = recipes;
            EnableAutoSort();
            
        }

        public void EnableAutoSort()
        {
            ViewSource.SortDescriptions.Clear();
            
            ViewSource.SortDescriptions.Add(new SortDescription("IsSatisfied", ListSortDirection.Descending));  // true values first
            ViewSource.SortDescriptions.Add(new SortDescription("BaseValue", ListSortDirection.Descending));
            ApplyFilter();
        }

        private void HandleOtherFilters(object? sender, NotifyCollectionChangedEventArgs e)
        {
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
                bool show = ShowPartiallySatisfiedRecipes == true ? recipe.IsPartiallySatisfied : recipe.IsSatisfied;
                UpdateAutoSelection(recipe, show);
                if (show)
                {
                    if (!SelectedEffects.Any(x => x == recipe.Effect))
                    {
                        return false;
                    }

                    if (!SelectedUnlocks.Any(x => x == recipe.Unlock))
                    {
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(recipe.Festival) && !SelectedFestivals.Any(x => x == recipe.Festival))
                    {
                        return false;
                    }
                }

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

            if (e.PropertyName == nameof(Recipe.IsSatisfied) || e.PropertyName == nameof(Recipe.IsPartiallySatisfied))
            {
                ApplyFilter();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
