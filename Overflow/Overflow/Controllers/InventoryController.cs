using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using Overflow.Models;
using System.Data;

namespace Overflow.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        public ActionResult Inventory(Inventory inventory)
        {
            try
            {
                string userName = Session["username"].ToString();
                inventory.Username = userName;

                var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;
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
                    inventory.Ingredients.Add(reader.GetString(0));
                }

                // Login login = new Login();
                return View("~/Views/Inventory/inventory.cshtml", inventory);
            }
            catch
            {
                Login login = new Login();
                login.LoginFlag = true;
                login.ErrorMessage = "";
                return View("~/Views/Home/Index.cshtml", login);
            }
        }

        [HttpPost]
        public ActionResult AddItem(string[] function_param)
        {
            Inventory inventory = new Inventory();
            

            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);

            if (con.State == System.Data.ConnectionState.Closed) //Makes a connection to the database
            {
                con.Open();
            }
            
            // ******This was all commented out below******

            string userName = Session["username"].ToString();
            inventory.Username = userName;
            SqlCommand sqlCommand = new SqlCommand("SELECT dbo.getAID(@uname_param)", con);  //makes "sqlCommand" reference the getAID function
            SqlParameter username_parameter = new SqlParameter("@uName_param", System.Data.SqlDbType.VarChar);
            username_parameter.Value = userName;

            sqlCommand.Parameters.Add(username_parameter);  //Adds the username as a parameter to the getAID function
            inventory.ID = (int)sqlCommand.ExecuteScalar(); //The result of the getAID function becomes the ID for the inventory model

            // If unable to add item because account ID doesn't exist, sends to homepage'
            if (inventory.ID == -1){
                return View("~/Views/Home/Index.cshtml");
            }

            SqlCommand sqlCommand2 = new SqlCommand("dbo.add_ingredientProc", con); // sqlCommand2 references the add_ingredient procedure
            sqlCommand2.CommandType = CommandType.StoredProcedure; //

            
            SqlParameter UserID = new SqlParameter("@Aid_param", System.Data.SqlDbType.Int);
            UserID.Value = inventory.ID;
            SqlParameter ingredientName = new SqlParameter("@fname_param", System.Data.SqlDbType.VarChar);

            foreach (string item in function_param)
            {
                inventory.Add.Add(item);
            }

            foreach (string item in inventory.Add) //Goes through all the items in the current "Add" list in the inventory model
            {
                ingredientName.Value = item;
                sqlCommand2.Parameters.Add(UserID);
                sqlCommand2.Parameters.Add(ingredientName);
                sqlCommand2.ExecuteNonQuery();
            }
            return RedirectToAction("Inventory", "Index");

        }
    }
}