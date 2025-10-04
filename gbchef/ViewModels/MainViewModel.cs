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

        public IEnumerable<string> Exclusions { get; set; }
        public ObservableCollection<string> SelectedExclusions { get; set; }
        private Dictionary<string, Exclusion> _exclusionMap { get; set; } = [];

        public CollectionViewSource ViewSource { get; } = new();

        public MainViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                recipe.PropertyChanged += HandleRecipeChanged;
            }
            InitExclusionsFilterBoxData();

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

        public void RefreshView()
        {
            ViewSource.View.Refresh();
        }

        private void ApplyFilter()
        {
            ViewSource.View.Filter = item =>
            {
                var recipe = item as Recipe;
                bool show = false;
                if (ShowAllRecipes == true)
                {
                    show = true;
                }
                else
                {
                    show = ShowPartiallySatisfiedRecipes == true ? recipe.IsPartiallySatisfied : recipe.IsSatisfied;
                    UpdateAutoSelection(recipe, show);
                }

                if (show)
                {
                    show = !SelectedExclusions.Any(x => _exclusionMap[x].Evaluate(recipe));
                }
                    
                return show;
            };

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
                RefreshView();
            }
        }

        private void InitExclusionsFilterBoxData()
        {
            var noEffect = new Exclusion {
                Name = "No Effect",
                Evaluate = (Recipe? recipe) => { if (recipe?.Effect == "None") return true; return false; } };

            var hasEffect = new Exclusion { 
                Name = "Has Effect", 
                Evaluate = (Recipe? recipe) => { if (recipe?.Effect != "None") return true; return false; } };

            var unlockAtStart = new Exclusion { 
                Name = "Available at the start", 
                Evaluate = (Recipe? recipe) => { if (recipe.Unlock.Contains("Available at the start")) return true; return false; }};

            var unlockCafeMadeleine = new Exclusion { 
                Name = "Café Madeleine", 
                Evaluate = (Recipe? recipe) => { if (recipe.Unlock.Contains("Café Madeleine")) return true; return false; } };

            var unlockClara = new Exclusion { 
                Name = "Clara's Diner", 
                Evaluate = (Recipe? recipe) => { if (recipe.Unlock.Contains("Clara's Diner")) return true; return false; } };

            var unlockMiniMadeleine = new Exclusion { 
                Name = "Mini Madeleine", 
                Evaluate = (Recipe? recipe) => { if (recipe.Unlock.Contains("Mini Madeleine")) return true; return false; } };

            var unlockNadine = new Exclusion { 
                Name = "Nadine's Bistro", 
                Evaluate = (Recipe? recipe) => { if (recipe.Unlock.Contains("Nadine's Bistro")) return true; return false; } };

            _exclusionMap[noEffect.Name] = noEffect;
            _exclusionMap[hasEffect.Name] = hasEffect;
            _exclusionMap[unlockAtStart.Name] = unlockAtStart;
            _exclusionMap[unlockCafeMadeleine.Name] = unlockCafeMadeleine;
            _exclusionMap[unlockClara.Name] = unlockClara;
            _exclusionMap[unlockMiniMadeleine.Name] = unlockMiniMadeleine;
            _exclusionMap[unlockNadine.Name] = unlockNadine;

            Exclusions = [.. _exclusionMap.Keys];
            SelectedExclusions = [];
            SelectedExclusions.CollectionChanged += HandleExclusions;
        }
        private void HandleExclusions(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshView();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
