using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using gbchef.Models;

namespace gbchef.Controls
{
    public partial class FilterListBox : UserControl
    {
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            nameof(DisplayName),
            typeof(string),
            typeof(FilterListBox),
            new PropertyMetadata("")
        );

        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<string>),
            typeof(FilterListBox),
            new PropertyMetadata(null, OnDependencyPropertyChanged)
        );

        public IEnumerable<string> Items
        {
            get => (IEnumerable<string>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(ObservableCollection<string>),
            typeof(FilterListBox),
            new PropertyMetadata(new ObservableCollection<string>(), OnDependencyPropertyChanged)
        );

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterListBox control)
            {
                control.UpdateListBoxSelectedItems();
            }
        }

        private void UpdateListBoxSelectedItems()
        {
            if (Items != null && SelectedItems != null)
            {
                var selectedItems = SelectedItems.Cast<string>().ToList();
                foreach (var item in selectedItems)
                {
                    if (Items.Contains(item))
                    {
                        InnerListBox.SelectedItems.Add(item);
                    }
                }
            }
        }

        private bool _isSelected = false;
        private bool _isDragging;
        private Point _startPosition;
        private readonly HashSet<string> _processedItems = [];

        public ObservableCollection<string> SelectedItems
        {
            get => (ObservableCollection<string>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public FilterListBox()
        {
            InitializeComponent();

        }

        private void InnerListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var item = FindItemUnderMouse(listBox, e);

            if (item != null)
            {
                _isDragging = true;
                _isSelected = listBox.SelectedItems.Contains(item);
                _startPosition = e.GetPosition(listBox);
                _processedItems.Clear();
                if (_isSelected)
                {
                    listBox.SelectedItems.Remove(item);
                }
                else
                {
                    listBox.SelectedItems.Add(item);
                }
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
                if (_isSelected)
                {
                    listBox.SelectedItems.Remove(item);
                }
                else
                {
                    listBox.SelectedItems.Add(item);
                }
                _processedItems.Add(item);
            }
        }

        private void InnerListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _processedItems.Clear();
            foreach (var item in Items)
            {
                if (InnerListBox.SelectedItems.Contains(item))
                {
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                    }
                }
                else
                {
                    SelectedItems.Remove(item);
                }
            }
        }

        private string? FindItemUnderMouse(ListBox listBox, MouseEventArgs e)
        {
            var visual = listBox.InputHitTest(e.GetPosition(listBox)) as DependencyObject;

            while (visual != null && visual is not ListBoxItem)
            {
                visual = VisualTreeHelper.GetParent(visual);
            }

            return (visual as ListBoxItem)?.Content as string;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items)
            {
                if (!SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                    InnerListBox.SelectedItems.Add(item);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items)
            {
                SelectedItems.Remove(item);
                InnerListBox.SelectedItems.Remove(item);
            }
        }
    }
}
