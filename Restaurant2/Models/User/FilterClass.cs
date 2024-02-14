using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class FilterClass
    {

        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string SearchTerm { get; set; }
        public int Value { get; set; }

    }


}