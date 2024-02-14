using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class Cupon
    {
        public int ID { get; set; }
        public String KuponName { get; set; }
        public String KuponKodu { get; set; }
        public int KuponTutarı { get; set; }
    }
}