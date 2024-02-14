using Restaurant1.Models;
using Restaurant1.Models.admin;
using Restaurant1.Models.User;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Restaurant.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            UserInfo u = new UserInfo();
            return View(u);
        }
        [HttpPost]
        public ActionResult Index(UserInfo u)
        {
            var list = new List<users>();
            using (var db = new RestaurantEntities12())
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                string psw = u.Password;
                string pswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(psw)));
                list = db.Database.SqlQuery<users>("Select * from users a where a.UserMail='" + u.Email + "' and a.UserPassword='" + pswHash + "'").ToList<users>();

            }
            if (list.Count() > 0)
            {
                users q = list[0];
                Session["w"] = q;
                Session["durum"] = 1;
                Session["cntrl"] = 1;

                return RedirectToAction("Food");
            }
            else
            {
                u.Error = "Hatalı şifre veya Email girdiniz.";
            }
            return View(u);
        }

        [HttpGet]
        public ActionResult Register()
        {
            UserInfo u = new UserInfo();
            return View(u);
        }
        [HttpPost]
        public ActionResult Register(UserInfo u)
        {
            u.Adress = u.semt + " mahallesi " + u.sokak + " sokak " + u.ApartmanAdı + " Apartmanı " + u.Kat + ". kat " +
            " Apt No:" + u.AptNo + " Daire No:" + u.DaireNo + " " + u.ilçe;

            var sclist = new List<users>();
            using (var db = new RestaurantEntities12())
            {
                sclist = db.Database.SqlQuery<users>("Select * from users a where a.UserMail='" + u.Email + "'").ToList<users>();
            }
            if (sclist.Count == 0)
            {
                using (var db = new RestaurantEntities12())
                {
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    string psw = u.Password;
                    string pswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(psw)));
                    int i = db.Database.ExecuteSqlCommand("insert into users(UserName,UserMail,UserPassword,UserAdress,UserPhone) values('" + u.Name + "','" + u.Email + "','"
                    + pswHash + "','" + u.Adress + "','" + u.Phone + "')");

                    var id = (from f in db.users where f.UserMail == u.Email select f.UserID).FirstOrDefault();
                    int e = db.Database.ExecuteSqlCommand("insert into Addresses(Address,ApartmanAdı,Ilce,Semt,Sokak,UserID,AptNo,Kat,DaireNo) values('" + u.Adress + "','" + u.ApartmanAdı + "','"
                    + u.ilçe + "','" + u.semt + "','" + u.sokak + "','" + id + "','" + u.AptNo + "','" + u.Kat + "','" + u.DaireNo + "')");

                }
                return RedirectToAction("Index", "User");
            }
            else
            {
                u.Error = "Bu email kayıtlı.";
                return View(u);
            }
        }
        public ActionResult Food(string str)
        {
            ViewData["str"] = str;
            return View();
        }

        public ActionResult Recipe(int id)
        {
            RecipeFood_Image image = new RecipeFood_Image();
            var sclist = new List<Recipe_Food>();
            Recipe_Food h = new Recipe_Food();
            using (var db = new RestaurantEntities12())
            {
                var foodDet = db.Food.Join(db.Recipe, f => f.FoodID, r => r.Food.FoodID, (_f, _r) => new
                {
                    FoodID = _f.FoodID,
                    FoodName = _f.FoodName,
                    FoodCal = _f.FoodCal,
                    FoodFat = _f.FoodFat,
                    FoodCar = _f.FoodCar,
                    FoodPro = _f.FoodPro,
                    FoodSalt = _f.FoodSalt,
                    FoodSugar = _f.FoodSugar,
                    FoodMin = _f.FoodMin,
                    FoodRecipe = _r.FoodRecipe,
                }).ToList();
                foreach (var y in foodDet)
                {
                    if (y.FoodID == id)
                    {
                        h.FoodName = y.FoodName;
                        h.Description = y.FoodRecipe;
                        h.Sugar = y.FoodSugar;
                        h.Salt = y.FoodSalt;
                        h.Calory = y.FoodCal;
                        h.Protein = y.FoodPro;
                        h.Carbohydrate = y.FoodCar;
                        h.Mineral = y.FoodMin;
                        h.Fat = y.FoodFat;
                        break;
                    }
                }
                var i = (from x in db.Food where x.FoodID == id select x.FoodPicture).FirstOrDefault();
                image.RecipeFood = h;
                image.Image = i;
                return View(image);
            }
        }
        public JsonResult Add(int FoodID)
        {
            if (Session["w"] == null)
            {
                return Json(JsonRequestBehavior.DenyGet);
            }
            else
            {
                users u = new users();
                u = (users)Session["w"];
                using (var db = new RestaurantEntities12())
                {
                    var OrderList = new List<orders>();
                    OrderList = db.Database.SqlQuery<orders>("Select * from orders").ToList<orders>();
                    Boolean isThere = false;
                    int amount = 0;
                    foreach (var item in OrderList)
                    {
                        if (item.FoodID == FoodID && item.UserID== u.UserID)
                        {
                            amount = (int)item.Amount + 1;
                            isThere = true;
                            break;
                        }
                    }
                    var piece = (from a in db.Food
                                 join b in db.Inventory on FoodID equals b.FoodID
                                 select
                                 new
                                 {
                                     b.InventoryPiece,
                                     a.FoodName
                                 }).FirstOrDefault();
                    if (amount - 1 != piece.InventoryPiece)
                    {
                        if (isThere == false)
                        {
                            Session["durum"] = 1;
                            int i = db.Database.ExecuteSqlCommand("insert into orders(UserID,FoodID) values(" + u.UserID + "," + FoodID + ")");
                            return Json(new { success = true, message = piece.FoodName + " Ürünü Sepetinize Eklendi" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Session["durum"] = 1;
                            int a = 0;
                            a = db.Database.ExecuteSqlCommand("Update orders set Amount = " + amount + "where UserID=" + u.UserID + " and FoodID =" + FoodID);
                            return Json(new { success = true, message = piece.FoodName + " Ürünü Sepetinize Eklendi" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        Session["durum"] = 0;
                        return Json(new { success = false,durum=0, hataMesaji = piece.FoodName + " ürününün maksimum adetine ulaştınız!" }, JsonRequestBehavior.DenyGet);
                    }
                }
            }
        }
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult Cartt()
        {
            var list = new List<orders>();
            using (var db = new RestaurantEntities12())
            {
                list = db.Database.SqlQuery<orders>("Select * from orders").ToList<orders>();
            }
            return RedirectToAction("RemoveCart", "User");
        }
        public JsonResult Remove(int FoodID)
        {
            using (var dbq = new RestaurantEntities12())
            {
                int a = dbq.Database.ExecuteSqlCommand("delete from orders where FoodID=" + FoodID);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult CartPartial()
        {
            Cart_Adress liste = new Cart_Adress(); 
            
            users r = (users)Session["w"];
            List<Cart> list1 = new List<Cart>();
            using (var db = new RestaurantEntities12())
            {
                var foodDet = db.Food.Join(db.orders, f => f.FoodID, t => t.Food.FoodID, (_f, _t) => new
                {
                    FoodName = _f.FoodName,
                    FoodID = _f.FoodID,
                    FoodPrice = _f.FoodPrice,
                    UserID = _t.UserID,
                    Amount = _t.Amount,
                    Picture = _f.FoodPicture
                }).ToList();
                var AdressList = (from a in db.Addresses where a.UserID == r.UserID select a).ToList();
                var orderC = (from a in db.users where a.UserID == r.UserID select a.OrderCount).FirstOrDefault();
                foreach (var item in foodDet)
                {
                    if (item.UserID == r.UserID)
                    {
                        Cart s = new Cart();
                        s.Phone = r.UserPhone;
                        s.FoodName = item.FoodName;
                        s.FoodPrice = item.FoodPrice;
                        s.FoodID = item.FoodID;
                        s.Amount = (int)item.Amount;
                        s.FoodPicture = item.Picture;
                        s.SiparişCounter = (int)orderC;
                        liste.cartList.Add(s);
                    }
                }
                foreach (var item in AdressList)
                {
                    Address_Information adres = new Address_Information();
                    adres.Adress = item.Address;
                    liste.addressList.Add(adres);
                }
            }

            return PartialView(liste);
        }
        public ActionResult RemoveCart()
        {
            users r = (users)Session["w"];
            var list = new List<orders>();
            int count;
            using (var db = new RestaurantEntities12())
            {
                list = db.Database.SqlQuery<orders>("Select * from orders").ToList<orders>();
                count = (int)(from co in db.users where co.UserID == r.UserID select co.OrderCount).FirstOrDefault();
                count += 1;
                int d = db.Database.ExecuteSqlCommand("update users set OrderCount=" + count + "where UserID=" + r.UserID);


            }
            foreach (orders i in list)
            {
                using (var db = new RestaurantEntities12())
                {
                    float sonTutar = float.Parse(Session["toplam"].ToString());
                    int adrID = (from a in db.Addresses where a.Address.Equals(r.UserAdress) select a.AddressID).FirstOrDefault();
                    int c = db.Database.ExecuteSqlCommand("insert into AdminOrder(UserID,FoodID,OrderDate,OrderAmount,OrderAddress,OrderPhone,OrderPrice) values(" + i.UserID + "," + i.FoodID + ",'" + DateTime.Now + "'," + i.Amount + "," + adrID + ",'" + r.UserPhone + "'," +sonTutar+ ")");
                }
            }
            foreach (var item in list)
            {
                using (var db = new RestaurantEntities12())
                {
                    var x = db.Database.SqlQuery<Inventory>("Select * from Inventory i where i.FoodID =" + item.FoodID).FirstOrDefault();
                    int a = db.Database.ExecuteSqlCommand("Update Inventory set InventoryPiece =InventoryPiece" + -item.Amount + " where FoodID =" + item.FoodID);
                    int b = db.Database.ExecuteSqlCommand("delete from orders where FoodID=" + item.FoodID);
                }
            }
            return View("OrderAccepted");
        }
        public PartialViewResult FoodPartial(string str)
        {
            if (str == null)
            {
                var db = new RestaurantEntities12();
                if (Session["filter"] is null)
                {
                    List<DetailsInfo> list = new List<DetailsInfo>();

                    var foodDet = (from a in db.Food
                                   join b in db.Inventory on a.FoodID equals b.FoodID
                                   select new
                                   {
                                       a.FoodID,
                                       a.FoodName,
                                       a.FoodCal,
                                       a.FoodFat,
                                       a.FoodCategory,
                                       a.FoodPrice,
                                       a.FoodCar,
                                       a.FoodPro,
                                       a.FoodSalt,
                                       a.FoodSugar,
                                       a.FoodMin,
                                       b.InventoryPiece,
                                       a.FoodPicture
                                   }).ToList();


                    foreach (var item in foodDet)
                    {
                        DetailsInfo d = new DetailsInfo();
                        if (item.InventoryPiece > 0)
                        {
                            d.FoodID = item.FoodID;
                            d.FoodName = item.FoodName;
                            d.FoodCategory = item.FoodCategory;
                            d.FoodPrice = item.FoodPrice;
                            d.FoodCal = item.FoodCal;
                            d.FoodFat = item.FoodFat;
                            d.FoodCar = item.FoodCar;
                            d.FoodPro = item.FoodPro;
                            d.FoodSalt = item.FoodSalt;
                            d.FoodSugar = item.FoodSugar;
                            d.FoodMin = item.FoodMin;
                            d.InventoryPiece = item.InventoryPiece;
                            d.FoodPicture = item.FoodPicture;
                            list.Add(d);
                        }
                    }
                    return PartialView(list);
                }
                else
                {
                    IQueryable<Food> foods = null;

                    var l = (FilterClass)(Session["filter"]);
                    int maxPrice = l.MaxPrice;
                    if (maxPrice == 0) maxPrice = 9999;
                    int minPrice = l.MinPrice;
                    String SearchTerm = l.SearchTerm;
                    List<DetailsInfo> list = new List<DetailsInfo>();

                    if (SearchTerm == null)
                    {
                        foods = from x in db.Food
                                where x.FoodPrice < maxPrice && x.FoodPrice > minPrice
                                select x;
                    }
                    else
                    {

                        foods = from x in db.Food
                                where x.FoodPrice < maxPrice && x.FoodPrice > minPrice && x.FoodName.Contains(SearchTerm)
                                select x;
                    }

                    var foodDet = (from a in foods
                                   join b in db.Inventory on a.FoodID equals b.FoodID
                                   select new
                                   {
                                       a.FoodID,
                                       a.FoodName,
                                       a.FoodCal,
                                       a.FoodFat,
                                       a.FoodCategory,
                                       a.FoodPrice,
                                       a.FoodCar,
                                       a.FoodPro,
                                       a.FoodSalt,
                                       a.FoodSugar,
                                       a.FoodMin,
                                       b.InventoryPiece,
                                       a.FoodPicture,
                                   }).ToList();


                    foreach (var item in foodDet)
                    {
                        DetailsInfo d = new DetailsInfo();
                        if (item.InventoryPiece > 0)
                        {
                            d.FoodPicture = item.FoodPicture;
                            d.FoodID = item.FoodID;
                            d.FoodName = item.FoodName;
                            d.FoodCategory = item.FoodCategory;
                            d.FoodPrice = item.FoodPrice;
                            d.FoodCal = item.FoodCal;
                            d.FoodFat = item.FoodFat;
                            d.FoodCar = item.FoodCar;
                            d.FoodPro = item.FoodPro;
                            d.FoodSalt = item.FoodSalt;
                            d.FoodSugar = item.FoodSugar;
                            d.FoodMin = item.FoodMin;
                            d.InventoryPiece = item.InventoryPiece;
                            list.Add(d);
                        }
                    }
                    FilterClass f = (FilterClass)Session["filter"];
                    List<DetailsInfo> list2 = new List<DetailsInfo>();
                    if (f.Value == 2)
                    {
                        list2 = list.OrderBy(x => x.FoodPrice).ToList();
                        list2.Reverse();
                        return PartialView(list2);
                    }
                    else if(f.Value==3)
                    {
                        list2 = list.OrderBy(x => x.FoodName).ToList();
                        return PartialView(list2);
                    }
                    else if(f.Value==4)
                    {
                        list2 = list.OrderBy(x => x.FoodName).ToList();
                        list2.Reverse();
                        return PartialView(list2);
                    }
                    else
                    {
                        list2 = list.OrderBy(x => x.FoodPrice).ToList();
                        return PartialView(list2);
                    }
                }


            }
            else
            {
                List<DetailsInfo> liste = new List<DetailsInfo>();
                RestaurantEntities12 db = new RestaurantEntities12();
                var list = (from a in db.Food
                            join b in db.Inventory on a.FoodID equals b.FoodID
                            select new
                            {
                                a.FoodID,
                                a.FoodName,
                                a.FoodCal,
                                a.FoodFat,
                                a.FoodCategory,
                                a.FoodPrice,
                                a.FoodCar,
                                a.FoodPro,
                                a.FoodSalt,
                                a.FoodSugar,
                                a.FoodMin,
                                b.InventoryPiece,
                                a.FoodPicture
                            }).ToList();
                var list2 = (from b in list where b.FoodCategory == str select b).ToList();
                foreach (var item in list2)
                {
                    DetailsInfo d = new DetailsInfo();
                    d.FoodID = item.FoodID;
                    d.FoodName = item.FoodName;
                    d.FoodCategory = item.FoodCategory;
                    d.FoodPrice = item.FoodPrice;
                    d.FoodCal = item.FoodCal;
                    d.FoodFat = item.FoodFat;
                    d.FoodCar = item.FoodCar;
                    d.FoodPro = item.FoodPro;
                    d.FoodSalt = item.FoodSalt;
                    d.FoodSugar = item.FoodSugar;
                    d.FoodMin = item.FoodMin;
                    d.InventoryPiece = item.InventoryPiece;
                    d.FoodPicture = item.FoodPicture;
                    liste.Add(d);
                }
                return PartialView(liste);
            }
        }

        public ActionResult Quit()
        {
            Session["w"] = null;
            Session["filter"] = null;
            return RedirectToAction("Login", "Login");
        }
        public ActionResult UserOrders()
        {
            users q = (users)Session["w"];
            List<Cart> list = new List<Cart>();
            using (var db = new RestaurantEntities12())
            {
                var foodDet = db.Food.Join(db.AdminOrder, f => f.FoodID, r => r.Food.FoodID, (_f, _r) => new
                {
                    FoodID = _f.FoodID,
                    FoodName = _f.FoodName,
                    OrderID = _r.AdminOrderID,
                    UserID = _r.UserID,
                    FoodPrice = _f.FoodPrice,
                    date = _r.OrderDate,
                    sit = _r.OrderSitutation,
                    amount = _r.OrderAmount,
                    FoodPicture = _f.FoodPicture,
                    OrderPrice = _r.OrderPrice
                }).ToList();
                foreach (var item in foodDet)
                {
                    if (item.UserID == q.UserID)
                    {
                        Cart o = new Cart();
                        o.FoodName = item.FoodName;
                        o.FoodPrice = item.FoodPrice;
                        o.Date = (DateTime)item.date;
                        o.Amount = (int)item.amount;
                        o.FoodPicture = item.FoodPicture;
                        o.OrderPrice =(float) item.OrderPrice;
                        if (item.sit == 0)
                        {
                            o.Situation = "Onay Bekliyor";
                        }
                        else if (item.sit == 1)
                        {
                            o.Situation = "Hazırlanıyor";
                        }
                        else if (item.sit == 2)
                        {
                            o.Situation = "Hazırlanıyor";
                        }
                        else if (item.sit == 3)
                        {
                            o.Situation = "Yola Çıktı";
                        }
                        else if (item.sit == 4)
                        {
                            o.Situation = "Teslim Edildi";
                        }
                        else if (item.sit == 5)
                        {
                            o.Situation = "İptal Edildi";
                        }
                        list.Add(o);
                    }
                }
                list.Reverse();
                return View(list);
            }

        }
        public JsonResult UpdateInform(Cart i)
        {
            users u = new users();
            u = (users)Session["w"];
            u.UserAdress = i.Address;
            Session["w"] = u;
            using (RestaurantEntities12 db = new RestaurantEntities12())
            {
                int a = db.Database.ExecuteSqlCommand("Update users set UserAdress ='" + i.Address.ToString().Substring(0, i.Address.Length - 1) + "'where UserID=" + u.UserID);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Account()
        {
            return View();
        }
        public PartialViewResult AccountPartial()
        {
            {
                users u = new users();
                u = (users)Session["w"];
                RestaurantEntities12 db = new RestaurantEntities12();
                var list = (from a in db.Addresses
                            join b in db.users on a.UserID equals b.UserID
                            select new
                            {
                                b.UserID,
                                a.Address,
                                b.UserName,
                                b.UserMail,
                                b.UserPhone,
                            }).ToList();
                var LastList = (from x in list where x.UserID == u.UserID select x).ToList();
                List<Address_Information> ModelList = new List<Address_Information>();
                foreach (var t in LastList)
                {
                    Address_Information a = new Address_Information();
                    a.Adress = t.Address;
                    a.UserPhone = t.UserPhone;
                    a.UserMail = t.UserMail;
                    a.UserName = t.UserName;
                    ModelList.Add(a);
                }
                var UsersDetails = (from a in db.users where a.UserID == u.UserID select a).FirstOrDefault();
                Address_Information ab = new Address_Information();
                ab.Adress = UsersDetails.UserAdress;
                ab.UserMail = UsersDetails.UserMail;
                ab.UserPhone = UsersDetails.UserPhone;
                ab.UserName = UsersDetails.UserName;
                ModelList.Add(ab);
                return PartialView(ModelList);
            }
        }
        public JsonResult UpdateInformations(UserInfo ui)
        {
            users u = new users();
            u = (users)Session["w"];
            using (RestaurantEntities12 db = new RestaurantEntities12())
            {
                int a = 0;
                a = db.Database.ExecuteSqlCommand("Update users set UserName ='" + ui.Name + "'where UserID=" + u.UserID);
                a = db.Database.ExecuteSqlCommand("Update users set UserMail ='" + ui.Email + "'where UserID=" + u.UserID);
                a = db.Database.ExecuteSqlCommand("Update users set UserAdress ='" + ui.Adress + "'where UserID=" + u.UserID);
                a = db.Database.ExecuteSqlCommand("Update users set UserPhone ='" + ui.Phone + "'where UserID=" + u.UserID);
            }
            u.UserMail = ui.Email;
            u.UserAdress = ui.Adress;
            u.UserPhone = ui.Phone;
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult IncreaseAmount(int FoodID)
        {
            users u = new users();
            u = (users)Session["w"];

            using (var db = new RestaurantEntities12())
            {
                var list = (from b in db.Inventory where b.FoodID == FoodID select b).FirstOrDefault();
                var amount = db.Database.SqlQuery<orders>("Select * from orders o where o.FoodID =" + FoodID).FirstOrDefault(); ;
                int a = 0;
                if (list.InventoryPiece != amount.Amount)
                {
                    int nwAmount = (int)amount.Amount + 1;
                    a = db.Database.ExecuteSqlCommand("Update orders set Amount = '" + nwAmount + "'where UserID=" + u.UserID + " and FoodID =" + FoodID);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list2 = (from b in db.Food where b.FoodID == FoodID select b).FirstOrDefault();
                    return Json(new { success = false, hataMesaji = list2.FoodName + " ürününün maksimum adetine ulaştınız!" }, JsonRequestBehavior.DenyGet);
                }
            }
        }
        public JsonResult DecreaseAmount(int FoodID)
        {
            users u = new users();
            u = (users)Session["w"];
            using (RestaurantEntities12 db = new RestaurantEntities12())
            {
                var amount = db.Database.SqlQuery<orders>("Select * from orders o where o.FoodID =" + FoodID).FirstOrDefault();
                if (amount.Amount != 1)
                {
                    int nwAmount = (int)amount.Amount - 1;
                    int a = 0;
                    a = db.Database.ExecuteSqlCommand("Update orders set Amount = '" + nwAmount + "'where UserID=" + u.UserID + " and FoodID =" + FoodID);
                }
                else
                {
                    int a = db.Database.ExecuteSqlCommand("delete from orders where FoodID=" + FoodID);
                }
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult FilterPartial()
        {
            return PartialView();
        }
        public JsonResult Filter(FilterClass f)
        {
            Session["filter"] = f;

            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment()
        {
            return View();
        }
        public ActionResult OrderAccepted()
        {
            return View();
        }
        public PartialViewResult OrderAcceptedPartial()
        {
            users u = new users();
            u = (users)Session["w"];
            RestaurantEntities12 db = new RestaurantEntities12();
            //var list = (from a in db.AdminOrder where a.UserID == u.UserID select a.AdminOrderID).ToList();
            var list1 = (from a in db.AdminOrder where a.UserID == u.UserID select a).ToList();
            //list.Reverse();
            list1.Reverse();

            var t = list1.First(); //burda adres id var joinle adresi bul 
            var adres = (from z in db.Addresses where z.AddressID == t.OrderAddress select z.Address).First();
            UserInfo g = new UserInfo();
            g.Adress = adres;
            g.Name = u.UserName;
            g.AptNo = t.AdminOrderID;
            Cupon c = new Cupon();
            //c.ID = list.First();
            return PartialView(g);
        }
        public PartialViewResult BuyButtonPartial()
        {
            return PartialView();
        }
        public ViewResult AddAddress()
        {
            return View();
        }

        public JsonResult AddAddressJson(AddressDetails a)
        {

            users u = new users();
            u = (users)Session["w"];
            a.userID = u.UserID;
            using (var db = new RestaurantEntities12())
            {

                if (a.ID == 0)
                {
                    var i = db.Database.ExecuteSqlCommand("insert into Addresses(UserID,Address,Ilce,Semt,Sokak,ApartmanAdı,AptNo,Kat,DaireNo) values(" + a.userID + ",'" + a.Adres + "','" + a.ilçe + "','" + a.semt + "','" + a.sokak + "','" + a.ApartmanAdı + "'," + a.AptNo + "," + a.Kat + "," + a.AptNo + ")");
                }
                else
                {
                    var x = (from q in db.Addresses where a.ID == q.AddressID select q).FirstOrDefault();
                    x.Address = a.Adres;
                    x.ApartmanAdı = a.ApartmanAdı;
                    x.Ilce = a.ilçe;
                    x.Semt = a.semt;
                    x.AptNo = a.AptNo;
                    x.Sokak = a.sokak;
                    x.Kat = a.Kat;
                    x.DaireNo = a.DaireNo;


                }
                db.SaveChanges();
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ViewResult AdresDuzenleme()

        {
            RestaurantEntities12 db = new RestaurantEntities12();
            users u = new users();
            u = (users)Session["w"];

            var LastList = (from x in db.Addresses where x.UserID == u.UserID select x).ToList();
            List<AddressDetails> ModelList = new List<AddressDetails>();
            foreach (var t in LastList)
            {
                AddressDetails a = new AddressDetails();
                a.ID = (int)t.AddressID;
                a.Adres = t.Address;
                a.ilçe = t.Ilce;
                a.semt = t.Semt;
                a.AptNo = (int)t.AptNo;
                a.ApartmanAdı = t.ApartmanAdı;
                a.sokak = t.Sokak;
                a.Kat = (int)t.Kat;
                a.DaireNo = (int)t.DaireNo;
                ModelList.Add(a);
            }
            return View(ModelList);
        }

        public JsonResult AddressDüzenleJson(int AddressID)
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            var adres = (from a in db.Addresses where a.AddressID == AddressID select a).FirstOrDefault();
            AddressDetails adresBilgi = new AddressDetails();
            adresBilgi.ID = (int)AddressID;
            adresBilgi.Adres = adres.Address;
            adresBilgi.semt = adres.Semt;
            adresBilgi.ilçe = adres.Ilce;
            adresBilgi.sokak = adres.Sokak;
            adresBilgi.ApartmanAdı = adres.ApartmanAdı;
            adresBilgi.AptNo = (int)adres.AptNo;
            adresBilgi.Kat = (int)adres.Kat;
            adresBilgi.DaireNo = (int)adres.DaireNo;
            return Json(new { success = true, adresBilgi }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Addresses()
        {
            return View();
        }
        public PartialViewResult AddresCardsPartial()
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            users u = new users();
            u = (users)Session["w"];

            var LastList = (from x in db.Addresses where x.UserID == u.UserID select x).ToList();
            List<AddressDetails> ModelList = new List<AddressDetails>();
            foreach (var t in LastList)
            {
                AddressDetails a = new AddressDetails();
                a.ID = (int)t.AddressID;
                a.Adres = t.Address;
                a.ilçe = t.Ilce;
                a.semt = t.Semt;
                a.AptNo = (int)t.AptNo;
                a.ApartmanAdı = t.ApartmanAdı;
                a.sokak = t.Sokak;
                a.Kat = (int)t.Kat;
                a.DaireNo = (int)t.DaireNo;
                ModelList.Add(a);
            }
            return PartialView(ModelList);
        }
        public JsonResult Kupon(Cupons k)
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            var kpn = (from a in db.Kupon where a.Kupon1.Equals(k.Kupon1) select a).FirstOrDefault();
            if (kpn != null && kpn.Durum != "1")
            {

                return Json(new { success = true, Name = kpn.KuponName, Tutar = kpn.KuponTutar }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdatePassword(UserInfo ui)
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            users u = new users();
            u = (users)Session["w"];
            var user = (from a in db.users where a.UserID == u.UserID select a).FirstOrDefault();
            SHA1 sha = new SHA1CryptoServiceProvider();
            string Oldpsw = ui.OldPassword;
            string newPsw = ui.Password;
            string OldpswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Oldpsw)));
            string NewpswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPsw)));
            if (OldpswHash.Equals(user.UserPassword))
            {
                user.UserPassword = NewpswHash;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShowFoodDetail(int id)
        {
            Session["id"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult FoodDetailPartial()
        {
            List<Cart> list = new List<Cart>();
            Cart s = new Cart();
            int a = (int)Session["id"];
            s.FoodID = a;
            list.Add(s);
            return PartialView(list);
        }
        public ViewResult UserCupons()
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            List<Cupon> cpnList = new List<Cupon>();
            var list = (from a in db.Kupon select a).ToList();
            foreach (var item in list)
            {
                Cupon c = new Cupon();
                c.KuponKodu = item.Kupon1;
                c.KuponName = item.KuponName;
                c.KuponTutarı = item.KuponTutar;
                cpnList.Add(c);
            }
            return View(cpnList);
        }
        public PartialViewResult Slides()
        {
            return PartialView();
        }
        public PartialViewResult KuryeAnimation()
        {
            return PartialView();
        }
        public PartialViewResult AddCartButton(int id)
        {
            Session["id"] = id;
            return PartialView();
        }
        public JsonResult GoPay(Cart c)
        {
            Session["toplam"] = c.OrderPrice;
            return Json(new { success = true,number=1 }, JsonRequestBehavior.AllowGet);
        } 
        public JsonResult AdresSil(int id)
        {
            RestaurantEntities12 db = new RestaurantEntities12();
            int a = db.Database.ExecuteSqlCommand("delete from Addresses where AddressID=" + id);
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}