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
using System.Text.RegularExpressions;


namespace Overflow.Controllers
{
    public class RecipeController : Controller
    {
        // GET: Recipe
        public ActionResult Recipe(OurRecipe recipe)
        {

            Inventory inventory = new Inventory();
            List<string> inventoryList = new List<string>();

            try
            {
                string userName = Session["username"].ToString();
                inventory.Username = userName;

                var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString; //Creates connection to database
                SqlConnection con = new SqlConnection(connection); 

                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand sqlCommand = new SqlCommand("SELECT dbo.getAID(@uname_param)", con);
                SqlParameter email = new SqlParameter("@uname_param", System.Data.SqlDbType.VarChar);
                email.Value = userName;
                sqlCommand.Parameters.Add(email);
                inventory.ID = (int)sqlCommand.ExecuteScalar();

                SqlCommand getFood = new SqlCommand("SELECT * FROM dbo.f_getFoodFromAID(@Aid_param)", con);
                SqlParameter AID = new SqlParameter("@Aid_param", System.Data.SqlDbType.Int);
                AID.Value = inventory.ID;
                getFood.Parameters.Add(AID);
                SqlDataReader reader = getFood.ExecuteReader();

                while (reader.Read())
                {
                    inventoryList.Add(reader.GetString(0));     //Adds read inventory items from the user's account to the inventory list      
                }

                // Login login = new Login();
                //return View;
            }
            catch ////returns to the login page if the user is not logged in////
            {
                Login login = new Login();
                login.LoginFlag = true;
                login.ErrorMessage = "";
                return View("~/Views/Home/Index.cshtml", login);
            }


            WebClient Client = new WebClient();
            //get a string representation of our json
           // string urlPageCode = Client.DownloadString("https://api.edamam.com/search?&app_id=e470194d&app_key=9efbee79595f1181598425c821e6e4bf&from=0&to=100&calories=591-722&health=alcohol-free");
           string urlPageCode = Client.DownloadString("https://api.edamam.com/search?q=milk&app_id=e470194d&app_key=9efbee79595f1181598425c821e6e4bf&from=0&to=100&calories=591-722&health=alcohol-free");

            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(urlPageCode);

            Dictionary<int, List<String>> d = new Dictionary<int, List<string>>();
            
            
            for (int i=0; i  <r.hits.Length; i++)
            {
                List<string> tempRecipeList = new List<string>();
                foreach (var item in r.hits.ElementAt(i).recipe.ingredientLines) //Goes through the ingredients in a recipe and adds it to the Ilist
                {
                    tempRecipeList.Add(item);
                }
                d.Add(i, tempRecipeList);
                //tempList.Clear();
            }

            
            var invContains = inventoryList.Select(w => @"\b" + Regex.Escape(w) + @"\b");
            var invMatch = new Regex("(" + string.Join(")|(", invContains) + ")");

            List<OurRecipe> recipes = new List<OurRecipe>(100);
            OurRecipe rec;

            //Iterates through all elements of dictionary
            for (int i = 0; i < d.Count(); i++)
            {
                int numMatches = 0;
                //Iterates through all ingredients for an element of dictionary
                foreach (string ingredient in d[i])
                {
                    string currentIngredient = ingredient;
                    bool found = false;
                    found = invMatch.IsMatch(currentIngredient);
                    if (found == true)
                    {
                        System.Diagnostics.Debug.WriteLine("Matched to" + currentIngredient + "!");
                        numMatches++;
                    }
                }
                int holder = r.hits.ElementAt(i).recipe.ingredientLines.Length;
                Decimal matchPercent = ((Decimal)numMatches / (Decimal)r.hits.ElementAt(i).recipe.ingredientLines.Length)*100;
                
                rec = new OurRecipe();
                rec.MatchPercent = matchPercent;
                rec.RecipeLabel = r.hits[i].recipe.label;
                rec.ImageURL = r.hits[i].recipe.image;
                rec.RecipeURL = r.hits[i].recipe.url;
                rec.Source = r.hits[i].recipe.source;
                recipes.Add(rec);

            }
           
            RecipeContainer rc = new RecipeContainer();
            rc.RecipeContainerListContainerofLists = recipes;
            
            return View("~/Views/Recipes/Recipes.cshtml", rc);
        }
    }
}