using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using gbchef.Models;

namespace gbchef.ViewModels
{
    public class CompositeViewModel
    {
        public ItemsViewModel ItemsVM { get; }
        public ResultsViewModel ResultsVM { get; }

        public CompositeViewModel(IEnumerable<Recipe> recipes)
        {
            ItemsVM = new ItemsViewModel();
            ResultsVM = new ResultsViewModel(recipes,ItemsVM);
        }

        public void RefreshFilters()
        {
            CollectionViewSource.GetDefaultView(ResultsVM.Recipes).Refresh();
        }
    }
}
