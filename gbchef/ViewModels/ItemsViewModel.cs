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
    public class ItemsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SelectableIngredient> Vegetables { get; } = new()
        {
            new SelectableIngredient("Radish",true),
            new SelectableIngredient("Tomato",true),
            new SelectableIngredient("Corn",true),
            new SelectableIngredient("Cucumber",true),
            new SelectableIngredient("Onion",true),
            new SelectableIngredient("Carrot",true),
            new SelectableIngredient("Avocado",true),
            new SelectableIngredient("1",true),
            new SelectableIngredient("2",true),
            new SelectableIngredient("3",true),
            new SelectableIngredient("4",true),
            new SelectableIngredient("5",true),

        };

        public ObservableCollection<SelectableIngredient> Fruits { get; } = new()
        {
            new SelectableIngredient("Apple", true),
            new SelectableIngredient("Orange", true),
            new SelectableIngredient("Peach", true),
        };

        public ObservableCollection<SelectableIngredient> Products { get; } = new()
        {
            new SelectableIngredient("Cheese", true),
            new SelectableIngredient("Milk", true),
            new SelectableIngredient("Butter", true),
            new SelectableIngredient("Mayonnaise", true),
        };

        public ObservableCollection<SelectableIngredient> Foragables { get; } = new()
        {
            new SelectableIngredient("Shiitake Mushroom", true),
            new SelectableIngredient("Honey", true),
            new SelectableIngredient("Enoki", true),
        };

        public ObservableCollection<SelectableIngredient> Fish { get; } = new()
        {
            new SelectableIngredient("Salmon", true),
            new SelectableIngredient("Yellow Perch", true),
            new SelectableIngredient("Snakehead", true),
        };

        public ObservableCollection<SelectableIngredient> Others { get; } = new()
        {
            new SelectableIngredient("Rice Flour", true),
            new SelectableIngredient("Salt", true),
            new SelectableIngredient("Oil", true),
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
