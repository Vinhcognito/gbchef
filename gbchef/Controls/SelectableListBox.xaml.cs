using gbchef.Models;
using gbchef.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gbchef.Controls
{
    public partial class SelectableListBox : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<SelectableIngredient>),
                typeof(SelectableListBox), new PropertyMetadata(null));

        public ObservableCollection<SelectableIngredient> Items
        {
            get => (ObservableCollection<SelectableIngredient>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public SelectableListBox()
        {
            InitializeComponent();
        }

        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var item = listBox.SelectedItem as SelectableIngredient;

            if (item != null) {
                item.IsSelected = !item.IsSelected;
            }
        }




    }
}
