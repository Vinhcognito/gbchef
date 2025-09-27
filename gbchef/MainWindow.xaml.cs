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
                var results = await dbService.ExecuteSelectAllAsync("Recipes");
                var recipes = results.Select(row => Recipe.ProcessRow(row)).ToList();

                var ingredients = await GetAllIngredients(dbService);
                AddSelectionChangedHandlerInRecipes(recipes, ingredients);

                _mainViewModel = new MainViewModel(recipes);
                _mainViewModel.Vegetables = new ObservableCollection<SelectableIngredient>(ingredients);

            }


            DataContext = _mainViewModel;

            // listen to each selectable ingredient property change
            var li = new List<ObservableCollection<SelectableIngredient>>(
                [
                    _mainViewModel.Vegetables,
                    _mainViewModel.Fruits,
                    _mainViewModel.Products,
                    _mainViewModel.Foragables,
                    _mainViewModel.Fish,
                    _mainViewModel.Others,
            ]);

            foreach (var collection in li) {
                foreach (var ingredient in collection) {
                    ingredient.PropertyChanged += MainVM_PropertyChanged;
                }
            }
                

        }

        private void AddSelectionChangedHandlerInRecipes(List<Recipe> recipes, List<SelectableIngredient> ingredients)
        {
            foreach (SelectableIngredient ingredient in ingredients)
            {
                foreach (var (recipeId,  slot) in ingredient.RecipeIdSlotMap)
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
            var ingredients = results.Select(row => new SelectableIngredient(Convert.ToInt32(row[0]), (string)row[1], false)).ToList();

            foreach (SelectableIngredient ingredient in ingredients)
            {
                // Recipe ID, Slot
                ingredient.RecipeIdSlotMap = await dbService.ExecuteSelectRecipeSlotMapByIngredientId(ingredient.Id);
            }
            return ingredients;
        }

        private void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"{(sender as SelectableIngredient).IsSelected}");
            Debug.WriteLine($"{(sender as SelectableIngredient).Name}");

        }


    }
}