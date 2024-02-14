using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class CoruierInfo
    {
        public int ID { get; set; }
        public int CoruierNumber { get; set; }
        public string CoruierPassword { get; set; }
        public string CoruierPhone { get; set; }
        public string Error { get; set; }
    }
}