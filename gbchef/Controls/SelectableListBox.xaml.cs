using gbchef.Models;
using gbchef.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private bool _selectionState = false;
        private bool _isDragging;
        private Point _startPosition;
        private readonly HashSet<SelectableIngredient> _processedItems = new();

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

        private void ClearButton_ClickHandler(object sender, RoutedEventArgs e)
        {
            foreach (SelectableIngredient ingredient in Items)
            {
                ingredient.IsSelected = false;
            }
        }

        private void SelectAllButton_ClickHandler(object sender, RoutedEventArgs e)
        {
            foreach (SelectableIngredient ingredient in Items)
            {
                ingredient.IsSelected = true;
            }
        }

        private void InnerListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var item = FindItemUnderMouse(listBox, e);
            
            if (item != null)
            {
                _isDragging = true;
                _selectionState = !item.IsSelected;
                _startPosition = e.GetPosition(listBox);
                _processedItems.Clear();

                item.IsSelected = _selectionState;
                _processedItems.Add(item);
                e.Handled = true;
            }
        }

        private void InnerListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || e.LeftButton != MouseButtonState.Pressed) return;

            var listBox = (ListBox)sender;
            var currentPosition = e.GetPosition(listBox);

            if (Math.Abs(currentPosition.Y - _startPosition.Y) < SystemParameters.MinimumVerticalDragDistance) return;

            var item = FindItemUnderMouse(listBox, e);
            if (item != null && !_processedItems.Contains(item))
            {
                item.IsSelected = _selectionState;
                _processedItems.Add(item);
            }
        }

        private void InnerListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _processedItems.Clear();
        }

        private SelectableIngredient? FindItemUnderMouse(ListBox listBox, MouseEventArgs e)
        {
            var visual = listBox.InputHitTest(e.GetPosition(listBox)) as DependencyObject;

            while (visual != null && visual is not ListBoxItem)
            {
                visual = VisualTreeHelper.GetParent(visual);
            }

            return (visual as ListBoxItem)?.Content as SelectableIngredient;
        }

    }
}
