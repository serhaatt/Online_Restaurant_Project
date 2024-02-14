using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class UsersDetail
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public int cn { get; set; }
        public int Kod { get; set; }
    }
}