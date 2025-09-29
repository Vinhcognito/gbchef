using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gbchef.Models;
using gbchef.ViewModels;

// start with zero ingredients => zero results

//  ADD OR SUBTRACT AN INGREDIENT
// click an ingredient
// find all recipes that include that ingredient => roughlist
//  for each item in roughlist
//      we mark that recipes ingredientslot as fulfilled or unfillled
//         //then check if recipe is fully satisfied:
//         if item is fully satisfied => add to results
//         else REMOVE from results if is in results


namespace gbchef
{
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;
        public MainWindow()
        {
            InitializeComponent();


        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            using (var dbService = new DatabaseService()) {

                var ssService = new SpriteSheetService("sprite_sheet.png", "sprite_sheet.json");

                var results = await dbService.ExecuteSelectAllAsync("Recipes");
                var recipes = results.Select(row => Recipe.ProcessRow(row)).ToList();

                foreach (var recipe in recipes)
                {
                    var possibleImageName = $"{recipe.Name.ToLower()}.png";
                    try
                    {
                        recipe.Image = ssService.GetSprite(possibleImageName);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Missing recipe image for: {recipe.Name}. Tried: {possibleImageName}");
                    }
                }

                var ingredients = await GetAllIngredients(dbService);
                AddSelectionChangedHandlerInRecipes(recipes, ingredients);

                _mainViewModel = new MainViewModel(recipes);
                _mainViewModel.Vegetables = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Vegetables"));
                _mainViewModel.Fruits = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Fruits"));
                _mainViewModel.Products = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Products"));
                _mainViewModel.Forageables = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Forageables"));
                _mainViewModel.Fish = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Fish"));
                _mainViewModel.Recipes = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Recipes"));
                _mainViewModel.Others = new ObservableCollection<SelectableIngredient>(ingredients.Where(x => x.Category == "Others"));

                AddRecipeAsIngredientReference(recipes, _mainViewModel.Recipes);

            }

            DataContext = _mainViewModel;

        }

        private void AddRecipeAsIngredientReference(List<Recipe> recipes, ObservableCollection<SelectableIngredient> ingredients)
        {
            Dictionary<string, SelectableIngredient> map = new Dictionary<string, SelectableIngredient>();
            foreach (SelectableIngredient ingredient in ingredients)
            {
                map[ingredient.Name] = ingredient;
            }

            foreach (Recipe recipe in recipes) {
                if (map.ContainsKey(recipe.Name))
                {
                    recipe.AsIngredient = map[recipe.Name];
                }
            }
        }

        private void AddSelectionChangedHandlerInRecipes(List<Recipe> recipes, List<SelectableIngredient> ingredients)
        {
            foreach (SelectableIngredient ingredient in ingredients)
            {
                foreach (var (recipeId, slot) in ingredient.RecipeIdSlotMap)
                {
                    int recipeIndex = recipeId - 1;
                    Recipe recipe = recipes[recipeIndex];

                    if (slot == 1)
                    {
                        ingredient.PropertyChanged += recipe.HandleSlot1Changed;
                        recipe.Slot1.Add(ingredient);
                    }
                    else if (slot == 2)
                    {
                        ingredient.PropertyChanged += recipe.HandleSlot2Changed;
                        recipe.Slot2.Add(ingredient);
                    }
                    else if (slot == 3)
                    {
                        ingredient.PropertyChanged += recipe.HandleSlot3Changed;
                        recipe.Slot3.Add(ingredient);
                    }
                    else if (slot == 4)
                    {
                        ingredient.PropertyChanged += recipe.HandleSlot4Changed;
                        recipe.Slot4.Add(ingredient);
                    }
                }
            }
        }

        private async Task<List<SelectableIngredient>> GetAllIngredients(DatabaseService dbService)
        {
            var results = await dbService.ExecuteSelectAllAsync("Items");
            var ingredients = results.Select(row => new SelectableIngredient(Convert.ToInt32(row[0]), (string)row[1], false) { Category = (string)row[2] }).ToList();

            foreach (SelectableIngredient ingredient in ingredients)
            {
                ingredient.RecipeIdSlotMap = await dbService.ExecuteSelectRecipeSlotMapByIngredientId(ingredient.Id);
            }
            return ingredients;
        }

        private void ShowPartial_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowPartiallySatisfiedRecipes = ShowPartial.IsChecked;
            _mainViewModel.ApplyFilter();
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowAllRecipes = ShowAll.IsChecked;
            _mainViewModel.ApplyFilter();
        }

        private void IncludeRelatedRecipesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowAutoSelected = IncludeRelatedRecipesCheckBox.IsChecked;
            
            if (IncludeRelatedRecipesCheckBox.IsChecked == false)
            {
                foreach (SelectableIngredient ingredient in _mainViewModel.Recipes)
                {
                    ingredient.IsAutoSelected = false;
                }
            }
            _mainViewModel.ApplyFilter();
        }
    }
}