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
        public ActionResult Recipe(Recipes recipe)
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
           // string urlPageCode = Client.DownloadString("https://api.edamam.com/search?&app_id=e470194d&app_key=&from=0&to=100&calories=591-722&health=alcohol-free");
           string urlPageCode = Client.DownloadString("https://api.edamam.com/search?q=milk&app_id=e470194d&app_key=&from=0&to=100&calories=591-722&health=alcohol-free");

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

            // Create string to concatenate to as well as sample data. Will pass an inventory later AND THEN THIS CODE CAN BE REMOVED
            string longlist = "";
            /*inventory.Ingredients.Add("2taco");
            inventory.Ingredients.Add("SEASONIN7G");
            inventory.Ingredients.Add("be3ef");
            inventory.Ingredients.Add("chicke5N");
            inventory.Ingredients.Add("che4ese");
            inventory.Ingredients.Add("leTTuce1");
            inventory.Ingredients.Add("r6iCe");
            inventory.Ingredients.Add("tomato");*/


            //TEST USING INVENTORY VS RECIPE INSTEAD OF RECIPE VS INVENTORY. SHOULD WORK BETTER BY NOT HAVING TO CHANGE AS MUCH
            inventory.Ingredients.Add("taco");
            inventory.Ingredients.Add("seasoning");
            inventory.Ingredients.Add("beef");
            inventory.Ingredients.Add("chicken");
            inventory.Ingredients.Add("cheese");
            inventory.Ingredients.Add("lettuce");
            inventory.Ingredients.Add("rice");
            inventory.Ingredients.Add("tomato");
            inventory.Ingredients.Add("milk");

            //Reads each ingredient from the inventory and concatenates to a blank string
            foreach (string ingredient in inventory.Ingredients)
            {
                //ingredient.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                longlist = String.Concat(longlist, (ingredient.ToLower() + ";"));
            }


            //Goes through the list of inventory ingredients and removes numbers THIS IS UNNECESSARY FOR THE SAMPLE DATA, BUT NEEDED FOR REAL
            do
            {
                int numberFoundIndex = -1;
                if ((numberFoundIndex = longlist.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' })) != -1) longlist = longlist.Remove(numberFoundIndex, 1);
                else break;
            } while (1 == 1);

            //For quickly comparing lists, use the below
            //var whatithas = inventory.Ingredients.Except(d[0]);
            int numMatches = 0;
            var invContains = inventory.Ingredients.Select(w => @"\b" + Regex.Escape(w) + @"\b");
            var invMatch = new Regex("(" + string.Join(")|(", invContains) + ")");

            List<Recipes> recipes = new List<Recipes>(100);
            Recipes rec;

            //Iterates through all elements of dictionary
            for (int i = 0; i < d.Count(); i++)
            {
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
                double matchPercent = (numMatches / d[i].Count);
                rec = new Recipes();
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