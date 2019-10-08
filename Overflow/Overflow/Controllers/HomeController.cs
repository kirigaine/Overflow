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
            login.ErrorMessage = "";
            /* 
             Use login.Email to access email entered, and login.Pass to access password entered.
             If email does not pass the required criteria, return a error modal.
             Else, call query to access db.
             
             */

            try
            {

                System.Net.Mail.MailAddress userEmail = new System.Net.Mail.MailAddress(login.Email);
                if (userEmail.Address == login.Email)
                {
                    int result = Auth_Login(login.Email, login.Pass);

                    if (result == 0)
                    {
                        login.IsLoggedIn = false;
                        login.LoginFlag = true;
                        return View("~/Views/Home/Index.cshtml", login);
                    }

                    else if (result == 1)
                    {
                        login.IsLoggedIn = true;
                        Session["username"] = login.Email;

                        return View("~/Views/Home/Index.cshtml", login);
                    }


                }

            }
            catch
            {
                Console.WriteLine("Invalid email format");
                login.IsLoggedIn = false;
                login.LoginFlag = true;
                //return PartialView("~/Views/Shared/loginErrorModal.cshtml", login);
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

            // Validate email as well as duplicacy
            try
            {
                System.Net.Mail.MailAddress userEmail = new System.Net.Mail.MailAddress(login.Email);
                if (userEmail.Address == login.Email)
                {
                    if ((int)sqlCommand.ExecuteScalar() == 1)
                    {
                        login.ErrorMessage = "Username already exist";
                        return View("~/Views/Home/Index.cshtml", login); // This needs to be adjusted in modal 
                    }
                }
            }
            catch
            {
                login.ErrorMessage = "Invalid email format";
                return View("~/Views/Home/Index.cshtml", login);
            }

            // Declare Regex patterns
            System.Text.RegularExpressions.Regex nameRegex = new System.Text.RegularExpressions.Regex("^[A-Za-z]{1,15}(-|'){0,1}[A-Za-z]{1,15}$");
            System.Text.RegularExpressions.Regex passwordRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#\$%\&*-])[A-Za-z\d!@#$%&*-]{8,20}$");
            System.Text.RegularExpressions.Match fnameMatch = nameRegex.Match(login.FirstName);
            System.Text.RegularExpressions.Match lnameMatch = nameRegex.Match(login.LastName);
            System.Text.RegularExpressions.Match passwordMatch = passwordRegex.Match(login.Pass);
            System.Text.RegularExpressions.Match sndpasswordMatch = passwordRegex.Match(login.SecondPass);

            if ((!fnameMatch.Success) || (!lnameMatch.Success))
            {
                login.ErrorMessage = "Invalid name format";
                return View("~/Views/Home/Index.cshtml", login);
            }

            if ((!passwordMatch.Success) || (!sndpasswordMatch.Success))
            {
                login.ErrorMessage = "Invalid password format";
                return View("~/Views/Home/Index.cshtml", login);
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

            if (login.Pass != login.SecondPass)
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
        public ActionResult inventory()
        {
            Login login = new Login();
            return View("inventory", login);
        }
    }
}