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

namespace Overflow.Controllers
{
    public class RecipeController : Controller
    {
        // GET: Recipe
        public ActionResult Recipe(Recipe recipe)
        {
            WebClient Client = new WebClient();
            //get a string representation of our json
            string urlPageCode = Client.DownloadString("https://api.edamam.com/search?q=chicken&app_id=e470194d&app_key=3d7bcf2d71b9b8c10c9a75576fc50e64&from=0&to=3&calories=591-722&health=alcohol-free");
            recipe = JsonConvert.DeserializeObject<Recipe>(urlPageCode);
                      
            return View();
        }
    }
}