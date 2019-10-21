using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overflow.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public List<String> Ingredients = new List<String>();

        public List<String> Add = new List<String>();

        public List<String> Delete = new List<String>();
          
  }
}