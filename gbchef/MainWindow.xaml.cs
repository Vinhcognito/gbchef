using gbchef.Models;
using gbchef.ViewModels;
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

namespace gbchef
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ItemsViewModel();

        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            using (var dbService = new DatabaseService())
            {
                var results = await dbService.ExecuteSelectAllAsync("Recipes");
                var recipes = results.Select(row => Recipe.ProcessRow(row)).ToList();
                Debug.WriteLine(recipes.Count);
                var resultsVM = new ResultsViewModel(recipes);
                resultsDataGrid.DataContext = resultsVM;
            }
        }


        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var item = listBox.SelectedItem as SelectableIngredient;

            if (item != null)
            {
                item.IsSelected = !item.IsSelected;
            }
        }

    }
}