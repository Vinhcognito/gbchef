using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
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


        public ObservableCollection<Recipe> Recipes { get; } = new();


        public MainViewModel(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes) {
                Recipes.Add(recipe);
            }    
        }







        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.WriteLine("itemviewmodel propertychanged");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }









        private HashSet<string> GetSelectedIngredients()
        {
            
            return new HashSet<string>(
                Vegetables.Where(i => i.IsSelected).Select(i => i.Name)
                .Concat(Fruits.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(Products.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(Foragables.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(Fish.Where(i => i.IsSelected).Select(i => i.Name))
                .Concat(Others.Where(i => i.IsSelected).Select(i => i.Name))
            );
        }
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
            Debug.WriteLine("selectable ingredient propertychanged");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
