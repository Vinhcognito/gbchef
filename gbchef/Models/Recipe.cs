using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbchef.Models
{
    public class Recipe
    {
        public int Id { get; set; }
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

        public static Recipe ProcessRow(params object[] items)
        {
            Recipe recipe = new() {
                //Id = (int)items[0],
                Type =          (string)items[0],
                Name =          (string)items[1],
                Ingredient1 =   (string)items[2],
                Ingredient2 =   (string)items[3],
                Ingredient3 =   (string)items[4],
                Ingredient4 =   (string)items[5],
                BaseValue =      Convert.ToInt32(items[6]),
                Effect =        (string)items[7],
                Unlock =        (string)items[8],
                Festival =      (string)items[9]
            };

            return recipe;
        }
    }
}
