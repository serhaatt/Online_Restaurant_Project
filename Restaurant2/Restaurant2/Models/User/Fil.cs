using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class Fil
    {

        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string SearchTerm { get; set; }

    }


}