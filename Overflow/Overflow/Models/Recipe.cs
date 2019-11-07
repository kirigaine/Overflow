using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overflow.Models
{
    public class Recipe
    {
        public string ImageURL { get; set; }
        public string Source { get; set; }
        public string RecipeURL { get; set; }
        public double MatchPercent { get; set; }
        public List<String> IngredientList = new List<String>();
    }
}