using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gbchef.ViewModels
{
    public class Items : INotifyPropertyChanged
    {
        public ObservableCollection<SelectableIngredient> MainItems { get; } = new()
        {
            new SelectableIngredient("Apple",true),
            new SelectableIngredient("Beer",true),
            new SelectableIngredient("Corn",true),

        };

        public ObservableCollection<SelectableIngredient> SecondaryItems { get; } = new()
        {
            new SelectableIngredient("Pear", true),
            new SelectableIngredient("Wine", true),
            new SelectableIngredient("Rice", true),
        };

        public ObservableCollection<SelectableIngredient> TertiaryItems { get; } = new()
        {
            new SelectableIngredient("Orange", true),
            new SelectableIngredient("Vodka", true),
            new SelectableIngredient("Barley", true),
        };

        public ObservableCollection<SelectableIngredient> Items4 { get; } = new()
        {
            new SelectableIngredient("Banana", true),
            new SelectableIngredient("Whiskey", true),
            new SelectableIngredient("Oats", true),
        };

        public ObservableCollection<SelectableIngredient> Items5 { get; } = new()
        {
            new SelectableIngredient("Grape", true),
            new SelectableIngredient("Tequila", true),
            new SelectableIngredient("Quinoa", true),
        };

        public event PropertyChangedEventHandler? PropertyChanged;
    }


    public class SelectableIngredient : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;

        public SelectableIngredient(string name, bool isSelected)
        {
            this.name = name;
            this.isSelected = isSelected;
        }

        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
