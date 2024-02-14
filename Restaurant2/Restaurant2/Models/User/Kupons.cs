using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class Kupons
    {
        public string Kupon1 { set; get; }
        public string Durum { set; get; }
        public string Name { get; set; }
        public int Tutar { get; set; }
        public double price { get; set; }
    }
}