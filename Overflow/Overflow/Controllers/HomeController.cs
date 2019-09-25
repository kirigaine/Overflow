using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Overflow.Models;

namespace Overflow.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(Login login)
        {
            login.LoginFlag = false;
            login.ErrorMessage = "";
            return View(login);
        }

        public ActionResult Login(Login login)
        {
            login.IsLoggedIn = false;
            /* 
             Use login.Email to access email entered, and login.Pass to access password entered.
             If email does not pass the required criteria, return a error modal.
             Else, call query to access db.
             
             */
            
            int result = Auth_Login(login.Email, login.Pass);

            if (result == 0)
            {
                login.IsLoggedIn = false;
                login.LoginFlag = true;
               return View("~/Views/Home/Index.cshtml", login);
            }
            else if(result == 1)
            {
                login.IsLoggedIn = true;
                return View("~/Views/Home/Index.cshtml", login);
            }
            return View("~/Views/Home/Index.cshtml", login);
        }

        public int Auth_Login(String uName, String pass)
        {
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand sqlCommand = new SqlCommand("SELECT dbo.fnLoginValidation(@uName_param, @pw_param)", con);

            SqlParameter username = new SqlParameter("@uName_param", System.Data.SqlDbType.VarChar);
            username.Value = uName;
            SqlParameter password = new SqlParameter("@pw_param", System.Data.SqlDbType.VarChar);
            password.Value = pass;

            sqlCommand.Parameters.Add(username);
            sqlCommand.Parameters.Add(password);

            return (int)sqlCommand.ExecuteScalar();

        }

        public ActionResult Signup(Login login)
        {
            login.ErrorMessage = "";

            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;           
            SqlConnection con = new SqlConnection(connection);

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand sqlCommand = new SqlCommand("SELECT dbo.uName_Check(@uName_param, @pw_param)", con);

            SqlParameter username = new SqlParameter("@uName_param", System.Data.SqlDbType.VarChar);
            username.Value = login.Email;
            SqlParameter password = new SqlParameter("@pw_param", System.Data.SqlDbType.VarChar);
            password.Value = "";

            sqlCommand.Parameters.Add(username);
            sqlCommand.Parameters.Add(password);
              
            if ((int)sqlCommand.ExecuteScalar() == 1)
            {
                login.ErrorMessage = "Username already exist";
                return View("~/Views/Home/Index.cshtml", login); // This needs to be adjusted in modal 
            }

            SqlCommand addUser = new SqlCommand("dbo.insertNewAccountProc", con);
            addUser.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter uName = new SqlParameter("@uName_proc_param", System.Data.SqlDbType.VarChar);
            uName.Value = login.Email;
            SqlParameter pass = new SqlParameter("@pw_proc_param", System.Data.SqlDbType.VarChar);
            pass.Value = login.Pass;
            SqlParameter firstName = new SqlParameter("@fName_proc_param", System.Data.SqlDbType.VarChar);
            firstName.Value = login.FirstName;
            SqlParameter lastName = new SqlParameter("@lName_proc_param", System.Data.SqlDbType.VarChar);
            lastName.Value = login.LastName;

            if(login.Pass != login.SecondPass)
            {
                login.ErrorMessage = "Passwords do not match";
            }

            if (login.ErrorMessage.Length == 0)
            {
                addUser.Parameters.Add(uName);
                addUser.Parameters.Add(pass);
                addUser.Parameters.Add(firstName);
                addUser.Parameters.Add(lastName);

                addUser.ExecuteNonQuery();
            }

           

            return View("~/Views/Home/Index.cshtml", login);
        }
    }
}