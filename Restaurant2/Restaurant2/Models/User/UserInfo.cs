using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class UserInfo
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string OldPassword { get; set; }

        public string Adress { get; set; }

        public string Phone { get; set; }

        public string Error { get; set; }
        public string ilçe { get; set; }
        public string semt { get; set; }
        public string sokak { get; set; }
        public string ApartmanAdı { get; set; }
        public int AptNo { get; set; }
        public int Kat { get; set; }
        public int DaireNo { get; set; }
    }
}