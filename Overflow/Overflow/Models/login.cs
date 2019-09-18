using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overflow.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool? IsLoggedIn { get; set; }

        public bool LoginFlag { get; set; }
    }
}