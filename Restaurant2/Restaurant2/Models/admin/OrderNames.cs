using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class OrderNames
    {
        public string Adres { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public string FoodName { get; set; }
        public DateTime date { get; set; }
        public string Situation { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public string Sit { set; get; }
        public int UserID { get; set; }

    }
}