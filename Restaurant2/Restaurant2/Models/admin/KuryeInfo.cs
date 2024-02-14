using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class KuryeInfo
    {
        public int KuryeNumber { get; set; }
        public int ID { get; set; }
        public string Password { get; set; }
        public string Error { get; set; }
        public string CoruierPhone { get; set; } 
    }
}