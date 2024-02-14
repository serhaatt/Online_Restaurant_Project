using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class Sepet
    {
        public string FoodName { get; set; }
        public int FoodPrice { get; set; }
        public int FoodID { get; set; }
        public string Situation { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Amount { get; set; }
        public string FoodPicture { get; set; }
        public int SiparişCounter { get; set; } 
    }
}