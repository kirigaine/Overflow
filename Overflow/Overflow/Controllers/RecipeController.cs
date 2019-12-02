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
            string urlPageCode = Client.DownloadString("https://api.edamam.com/search?q=chicken&app_id=e470194d&app_key=&from=0&to=100&calories=591-722&health=alcohol-free");

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

            


            return View();
        }
    }
}