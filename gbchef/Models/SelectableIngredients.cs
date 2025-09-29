using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gbchef.Models
{
    public class SelectableIngredient : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
        public IEnumerable<Tuple<int, int>> RecipeIdSlotMap { get; set; } = new List<Tuple<int, int>>();

        private bool isSelected = false;
        private bool isAutoSelected = false;


        public SelectableIngredient(int id, string name, bool isSelected)
        {
            Id = id;
            Name = name;
            this.isSelected = isSelected;
        }

        public SelectableIngredient(string name, bool isSelected)
        {
            Id = -1;
            Name = name;
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

        public bool IsAutoSelected
        {
            get => isAutoSelected;
            set
            {
                if (value != isAutoSelected)
                {
                    isAutoSelected = value;
                    OnPropertyChanged(nameof(IsAutoSelected));
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
