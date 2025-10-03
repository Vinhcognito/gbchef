using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace gbchef.Models
{
    public class Recipe : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public CroppedBitmap Image { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Ingredient1 { get; set; }
        public string Ingredient2 { get; set; }
        public string Ingredient3 { get; set; }
        public string Ingredient4 { get; set; }
        public int BaseValue { get; set; }
        public string Effect { get; set; }
        public string Unlock { get; set; }
        public string Festival { get; set; }

        public SelectableIngredient? AsIngredient { get; set; }

        public List<SelectableIngredient> Slot1 = [];
        public List<SelectableIngredient> Slot2 = [];
        public List<SelectableIngredient> Slot3 = [];
        public List<SelectableIngredient> Slot4 = [];

        public bool Is1Filled;
        public bool Is2Filled;
        public bool Is3Filled;
        public bool Is4Filled;

        private bool isSatisfied = false;
        public bool IsSatisfied
        {
            get => isSatisfied;
            set
            {
                if (value != isSatisfied)
                {
                    isSatisfied = value;
                    OnPropertyChanged(nameof(IsSatisfied));
                }
            }
        }
        private bool isPartiallySatisfied = false;
        public bool IsPartiallySatisfied
        {
            get => isPartiallySatisfied;
            set
            {
                if (value != isPartiallySatisfied)
                {
                    isPartiallySatisfied = value;
                    OnPropertyChanged(nameof(IsPartiallySatisfied));
                }
            }
        }


        public static Recipe ProcessRow(params object[] items)
        {
            Recipe recipe = new() {
                Id = Convert.ToInt32(items[0]),
                Type =          (string)items[1],
                Name =          (string)items[2],
                Ingredient1 =   (string)items[3],
                Ingredient2 =   (string)items[4],
                Ingredient3 =   (string)items[5],
                Ingredient4 =   (string)items[6],
                BaseValue = Convert.ToInt32(items[7]),
                Effect =        (string)items[8],
                Unlock =        (string)items[9],
                Festival =      (string)items[10]
            };

            if (recipe.Ingredient2.Trim().Length == 0)
            {
                recipe.Is2Filled = true;
            }

            if (recipe.Ingredient3.Trim().Length == 0)
            {
                recipe.Is3Filled = true;
            }

            if (recipe.Ingredient4.Trim().Length == 0)
            {
                recipe.Is4Filled = true;
            }

            return recipe;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Debug.WriteLine("Recipe propertychanged");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void HandleSlot1Changed(object? sender, PropertyChangedEventArgs e)
        {
            Is1Filled = Slot1.Any(item => item.IsSelected || item.IsAutoSelected);
            UpdateIngredientsSatisfaction();
        }

        internal void HandleSlot2Changed(object? sender, PropertyChangedEventArgs e)
        {
            Is2Filled = Slot2.Any(item => item.IsSelected || item.IsAutoSelected);
            UpdateIngredientsSatisfaction();
        }

        internal void HandleSlot3Changed(object? sender, PropertyChangedEventArgs e)
        {
            Is3Filled = Slot3.Any(item => item.IsSelected || item.IsAutoSelected);
            UpdateIngredientsSatisfaction();
        }

        internal void HandleSlot4Changed(object? sender, PropertyChangedEventArgs e)
        {
            Is4Filled = Slot4.Any(item => item.IsSelected || item.IsAutoSelected);
            UpdateIngredientsSatisfaction();
        }

        private void UpdateIngredientsSatisfaction()
        {
            IsSatisfied = Is1Filled && Is2Filled && Is3Filled && Is4Filled;
            IsPartiallySatisfied = Is1Filled 
                || (Is2Filled && Slot2.Count > 0)
                || (Is3Filled && Slot3.Count > 0)
                || (Is4Filled && Slot4.Count > 0);
        }
    }
}
