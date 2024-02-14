using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class AdresBilgi
    {
        public int userID { get; set; }
        public int ID { get; set; }
        public string Adres { get; set; }
        public string ilçe { get; set; }
        public string semt { get; set; }
        public string sokak { get; set; }
        public string ApartmanAdı { get; set; }
        public int AptNo { get; set; }
        public int Kat { get; set; }
        public int DaireNo { get; set; }
    }
}