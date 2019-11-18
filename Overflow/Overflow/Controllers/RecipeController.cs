using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using Overflow.Models;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Reflection;

namespace Overflow.Controllers
{
    public class RecipeController : Controller
    {
        // GET: Recipe
        public ActionResult Recipe(Recipes recipe)
        {
            WebClient Client = new WebClient();
            //get a string representation of our json
            string urlPageCode = Client.DownloadString("https://api.edamam.com/search?q=chicken&app_id=e470194d&app_key=9b0f6f1c05136f8a36d48da9bd586891&from=0&to=100&calories=591-722&health=alcohol-free");

            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(urlPageCode);


            for(int i=0; i  <r.hits.Length; i++)
            {
                Dictionary<int, List<String>> d = new Dictionary<int, List<string>>;

                List<string> IList = new List<string>();
                foreach (var item in r.hits.ElementAt(i).recipe.ingredientLines) //Goes through the ingredients in a recipe and adds it to the Ilist
                {
                    IList.Add(item);
                }

            }



            
            

            return View();
        }
    }
}