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

            // Subscribe to PropertyChanged event
            ((MainViewModel)DataContext).PropertyChanged += MainVM_PropertyChanged;

        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            using (var dbService = new DatabaseService()) {
                var queryResults = await dbService.ExecuteSelectAllAsync("Recipes");
                var recipes = queryResults.Select(row => Recipe.ProcessRow(row)).ToList();
                _mainViewModel = new MainViewModel(recipes);
            }
            DataContext = _mainViewModel;
        }


        private void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check which property changed
            if (e.PropertyName == nameof(MainViewModel.Vegetables)) {
                YourMethod();
            }
        }

        private void YourMethod()
        {
            Debug.WriteLine("vegetable");
        }

    }
}