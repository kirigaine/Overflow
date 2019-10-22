using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using Overflow.Models;

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
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            //string userName = Session["username"].ToString();
            //inventory.Username = userName;
            //SqlCommand sqlCommand = new SqlCommand("SELECT dbo.getAID(@uname_param)", con);
            //SqlParameter username = new SqlParameter("@uName_param", System.Data.SqlDbType.VarChar);
            //username.Value = userName;

            //sqlCommand.Parameters.Add(username);
            //inventory.ID = (int)sqlCommand.ExecuteScalar();

            //// If unable to add item because account ID doesn't exist, sends to homepage'
            //if (inventory.ID == -1){
            //    return View("~/Views/Home/Index.cshtml");
            //}

            //SqlCommand sqlCommand2 = new SqlCommand("SELECT dbo.add_ingredientProc(@Aid_param, @fname_param)", con);

            //SqlParameter UserID = new SqlParameter("@Aid_param", System.Data.SqlDbType.Int);
            //UserID.Value = inventory.ID;
            //SqlParameter ingredientName = new SqlParameter("@fname_param", System.Data.SqlDbType.VarChar);

            //foreach (string item in inventory.Ingredients)
            //{
            //    ingredientName.Value = item;
            //    sqlCommand2.Parameters.Add(UserID);
            //    sqlCommand2.Parameters.Add(ingredientName);
            //    sqlCommand2.ExecuteNonQuery();
            //}
            return View("~/Views/Inventory/inventory.cshtml");

        }
    }
}