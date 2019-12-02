using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overflow.Models
{
    public class Rootobject // Rootobject from API
    {
        public string q { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public bool more { get; set; }
        public int count { get; set; }
        public Hit[] hits { get; set; }
    }

    public class Hit // Hit from API
    {
        public Recipe recipe { get; set; }
        public bool bookmarked { get; set; }
        public bool bought { get; set; }
    }

    public class Recipe // Recipe from API
    {
        public string label { get; set; }
        public string image { get; set; }
        public string source { get; set; }
        public string url { get; set; }
        public string[] dietLabels { get; set; }
        public string[] ingredientLines { get; set; }
        public float calories { get; set; }
        //public Totalnutrients totalNutrients { get; set; }
    }

    //public class Totalnutrients
    //{
    //    public FAT FAT { get; set; }
    //    public CHOCDF CHOCDF { get; set; }
    //    public PROCNT PROCNT { get; set; }
    //}
    //public class FAT
    //{
    //    public string label { get; set; }
    //    public float quantity { get; set; }
    //    public string unit { get; set; }
    //}
    //public class CHOCDF
    //{
    //    public string label { get; set; }
    //    public float quantity { get; set; }
    //    public string unit { get; set; }
    //}
    //public class PROCNT
    //{
    //    public string label { get; set; }
    //    public float quantity { get; set; }
    //    public string unit { get; set; }
    //}

public class OurRecipe //Assumed to be the main recipe class used
    {
        public string ImageURL { get; set; }
        public string Source { get; set; }
        public string RecipeLabel { get; set; }
        public string RecipeURL { get; set; }
        public Decimal MatchPercent { get; set; }
    }
}