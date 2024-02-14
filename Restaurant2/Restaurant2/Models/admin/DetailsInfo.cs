using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant1.Models.admin
{
    public class DetailsInfo
    {
        public int FoodID { get; set; }
        public string FoodName { get; set; }
        public string FoodCategory { get; set; }
        public int FoodPrice { get; set; }
        public string FoodCal { get; set; }
        public string FoodFat { get; set; }
        public string FoodCar { get; set; }
        public string FoodPro { get; set; }
        public string FoodSalt { get; set; }
        public string FoodSugar { get; set; }
        public string FoodMin { get; set; }
        public string FoodRecipe{ get; set; }
        public int InventoryPiece { get; set; }
        public string FoodPicture { get; set; }

    }
}