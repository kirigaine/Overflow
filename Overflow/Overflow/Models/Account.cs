using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overflow.Models
{
    public class Account
    {
        public Login Login { get; set; }
        public Signup Signup { get; set; }
    }
}