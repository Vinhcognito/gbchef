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
            new SelectableIngredient("Radish",false),
            new SelectableIngredient("Tomato",false),
            new SelectableIngredient("Corn",false),
            new SelectableIngredient("Cucumber",false),
            new SelectableIngredient("Onion",false),
            new SelectableIngredient("Carrot",false),
            new SelectableIngredient("Avocado",false),
            new SelectableIngredient("1",false),
            new SelectableIngredient("2",false),
            new SelectableIngredient("3",false),
            new SelectableIngredient("4",false),
            new SelectableIngredient("5",false),

        };

        public ObservableCollection<SelectableIngredient> Fruits { get; } = new()
        {
            new SelectableIngredient("Apple", false),
            new SelectableIngredient("Orange", false),
            new SelectableIngredient("Peach", false),
        };

        public ObservableCollection<SelectableIngredient> Products { get; } = new()
        {
            new SelectableIngredient("Cheese", false),
            new SelectableIngredient("Milk", false),
            new SelectableIngredient("Butter", false),
            new SelectableIngredient("Mayonnaise", false),
        };

        public ObservableCollection<SelectableIngredient> Foragables { get; } = new()
        {
            new SelectableIngredient("Shiitake Mushroom", false),
            new SelectableIngredient("Honey", false),
            new SelectableIngredient("Enoki", false),
        };

        public ObservableCollection<SelectableIngredient> Fish { get; } = new()
        {
            new SelectableIngredient("Salmon", false),
            new SelectableIngredient("Yellow Perch", false),
            new SelectableIngredient("Snakehead", false),
        };

        public ObservableCollection<SelectableIngredient> Others { get; } = new()
        {
            new SelectableIngredient("Rice Flour", false),
            new SelectableIngredient("Salt", false),
            new SelectableIngredient("Oil", false),
        };


        public ObservableCollection<Recipe> Recipes { get; } = new();

        public ObservableCollection<Recipe> Results { get; } = new();


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
        public string Name;
        public bool isSelected;

        public SelectableIngredient(string name, bool isSelected)
        {
            this.Name = name;
            this.isSelected = isSelected;
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
