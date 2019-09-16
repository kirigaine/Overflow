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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(Login login)
        {
            /* 
             Use login.Email to access email entered, and login.Pass to access password entered.
             If email does not pass the required criteria, return a error modal.
             Else, call query to access db.
             
             */
            int result = Auth_Login(login.Email, login.Pass);
            return Content(login.Email + login.Pass);
        }

        public int Auth_Login(String uName, String pass)
        {
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["OverflowDB"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand sqlCommand = new SqlCommand("SELECT dbo.fnLoginValidation(@uName, @pass)", con);
            SqlParameter username = new SqlParameter("@uName", System.Data.SqlDbType.VarChar);
            username.Value = uName;
            SqlParameter password = new SqlParameter("@pass", System.Data.SqlDbType.VarChar);
            password.Value = pass;

            object a = new Object();
            int x;

            a = sqlCommand.ExecuteScalar();

            return 0;
        }
    }
}