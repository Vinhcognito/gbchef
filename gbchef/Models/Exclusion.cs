using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbchef.Models
{
    public class Exclusion
    {
        public required string Name { get; set; }
        public required Predicate<Recipe?> Evaluate;
    }
}
