using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.User
{
    public class Cart_Adress
    {
        public List<Cart> cartList { get; set; }
        public List<Address_Information> addressList { get; set; }
        public Cart_Adress()
        {
            cartList = new List<Cart>();
            addressList = new List<Address_Information>();
        }
    }
}