using System;
using System.Collections.Generic;
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
            
            return Content(login.Email + login.Pass);
        }
    }
}