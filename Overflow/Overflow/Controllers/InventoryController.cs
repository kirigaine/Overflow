using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Overflow.Models;

namespace Overflow.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        public ActionResult Index()
        {
            Login login = new Login();
            return View("~/Views/Inventory/inventory.cshtml", login);
        }
    }
}