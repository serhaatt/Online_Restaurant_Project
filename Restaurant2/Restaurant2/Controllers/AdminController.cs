using Restaurant1.Models;
using Restaurant1.Models.admin;
using Restaurant1.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Restaurant1.Controllers
{
    public class AdminController : Controller
    {
        Random rd = new Random();
        public static int onay = 0;
        [HttpGet]
        public ActionResult Index()
        {
            adminInfo a = new adminInfo();
            return View(a);
        }
        [HttpPost]
        public ActionResult Index(adminInfo a)
        {
            var list = new List<Admin>();
            using (var db = new RestaurantEntities10())
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                string psw = a.Password;
                string pswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(psw)));
                list = db.Database.SqlQuery<Admin>("Select * from Admin a where a.AdminMail='" + a.Email + "' and a.AdminPassword='" + pswHash + "'").ToList<Admin>();
            }
            if (list.Count() > 0)
            {
                Admin q = list.First();
                Session["w"] = q;
                return RedirectToAction("AFood", "Admin");
            }
            else
            {
                a.Error = "Hatalı şifre veya Email girdiniz.";
            }
            return View(a);
        }
        public ActionResult AFood(string str)
        {
            ViewData["str"] = str;
            return View();
        }
        public ActionResult Deneme()
        {
            return View();
        }
        public PartialViewResult AFoodPartials(string str)
        {
            if (str == null)
            {
                var db = new RestaurantEntities10();
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
                    return PartialView(list);
                }
                else
                {
                    IQueryable<Food> foods = null;

                    var l = (aFil)(Session["filter"]);
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
                    //Session["filter"] = null;
                    return PartialView(list);
                }


            }
            else
            {
                List<DetailsInfo> liste = new List<DetailsInfo>();
                RestaurantEntities10 db = new RestaurantEntities10();
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
        public ActionResult Orders()
        {

            return View();
        }
        public ActionResult Users()
        {

            return View();
        }
        public ActionResult FoodDetail()
        {
            return View();
        }
        public JsonResult Update(DetailsInfo F, string Image)
        {
            using (RestaurantEntities10 db = new RestaurantEntities10())
            {
                int a = 0;
                a = db.Database.ExecuteSqlCommand("Update Food set FoodName ='" + F.FoodName + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodPrice ='" + F.FoodPrice + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodCategory ='" + F.FoodCategory + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodCal ='" + F.FoodCal + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodFat ='" + F.FoodFat + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodCar ='" + F.FoodCar + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodPro ='" + F.FoodPro + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodSalt ='" + F.FoodSalt + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodSugar ='" + F.FoodSugar + "'where FoodID=" + F.FoodID);
                a = db.Database.ExecuteSqlCommand("Update Food set FoodMin ='" + F.FoodMin + "'where FoodID=" + F.FoodID);
                int b = db.Database.ExecuteSqlCommand("Update Recipe set FoodRecipe ='" + F.FoodRecipe + "'where FoodID=" + F.FoodID);
                int c = db.Database.ExecuteSqlCommand("Update Inventory set InventoryPiece ='" + F.InventoryPiece + "'where FoodID=" + F.FoodID);

                var x = (from q in db.Food where q.FoodID == F.FoodID select q).FirstOrDefault();
                x.FoodPicture = "/Resimler/" + Image;
                db.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult DetailPartial(int id)
        {
            Image_DetailsInfo image = new Image_DetailsInfo();
            DetailsInfo d = new DetailsInfo();

            using (RestaurantEntities10 db = new RestaurantEntities10())
            {
                var x = db.Database.SqlQuery<Food>("Select * from Food a where a.FoodID=" + id).First();
                var y = db.Database.SqlQuery<Recipe>("Select * from Recipe a where a.FoodID=" + id).First();
                var z = db.Database.SqlQuery<Inventory>("Select * from Inventory a where a.FoodID=" + id).First();
                d.FoodID = id;
                d.FoodName = x.FoodName;
                d.FoodCategory = x.FoodCategory;
                d.FoodPrice = x.FoodPrice;
                d.FoodCal = x.FoodCal;
                d.FoodFat = x.FoodFat;
                d.FoodCar = x.FoodCar;
                d.FoodPro = x.FoodPro;
                d.FoodSalt = x.FoodSalt;
                d.FoodSugar = x.FoodSugar;
                d.FoodMin = x.FoodMin;
                d.FoodRecipe = y.FoodRecipe;
                d.InventoryPiece = z.InventoryPiece;
                var q = (from a in db.Food where id == a.FoodID select a.FoodPicture).FirstOrDefault();
                image.Details = d;
                image.Image = q;

            }
            return PartialView(image);
        }
        public ActionResult FoodAdd()
        {
            return View();
        }
        public JsonResult Add(DetailsInfo d)
        {
            Food food = new Food();
            Inventory i = new Inventory();
            Recipe recipe = new Recipe();
            food.FoodName = d.FoodName;
            food.FoodCategory = d.FoodCategory;
            food.FoodPrice = d.FoodPrice;
            food.FoodCal = d.FoodCal;
            food.FoodFat = d.FoodFat;
            food.FoodCar = d.FoodCar;
            food.FoodPro = d.FoodPro;
            food.FoodSalt = d.FoodSalt;
            food.FoodSugar = d.FoodSugar;
            food.FoodMin = d.FoodMin;
            food.FoodPicture = "/Resimler/" + d.FoodPicture;
            i.InventoryPiece = d.InventoryPiece;
            recipe.FoodRecipe = d.FoodRecipe;
            using (var db = new RestaurantEntities10())
            {
                int a = db.Database.ExecuteSqlCommand("insert into Food(FoodName,FoodCategory,FoodPrice,FoodCal,FoodFat,FoodCar,FoodPro,FoodSalt,FoodSugar,FoodMin, FoodPicture)" +
                                    "values('" + food.FoodName + "','" + food.FoodCategory + "','" + food.FoodPrice + "','" + food.FoodCal + "','" + food.FoodFat + "','" + food.FoodCar + "','" + food.FoodPro + "','" + food.FoodSalt + "','" + food.FoodSugar + "','" + food.FoodMin + "','" + food.FoodPicture + "')");
                var x = db.Database.SqlQuery<Food>("select * from Food f where f.FoodName='" + food.FoodName + "'");
                int b = db.Database.ExecuteSqlCommand("insert into Recipe(FoodID,FoodRecipe) values(" + x.First().FoodID + ",'" + recipe.FoodRecipe + "')");
                int c = db.Database.ExecuteSqlCommand("insert into Inventory(FoodID,InventoryPiece) values(" + x.First().FoodID + ",'" + i.InventoryPiece + "')");

            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Remove(int id)
        {
            using (var dbq = new RestaurantEntities10())
            {
                int a = dbq.Database.ExecuteSqlCommand("delete from Food where FoodID=" + id);
            }
            return RedirectToAction("AFood", "Admin");
        }
        public ActionResult Quit()
        {
            Session["w"] = null;
            return RedirectToAction("Login", "Login");
        }
        public JsonResult DeleteUser(int UserID)
        {
            using (var dbq = new RestaurantEntities10())
            {
                int a = dbq.Database.ExecuteSqlCommand("delete from users where UserID=" + UserID);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult UsersPartial()
        {
            var list = new List<users>();
            List<UsersDetail> HiddenList = new List<UsersDetail>();
            using (var db = new RestaurantEntities10())
            {
                list = db.Database.SqlQuery<users>("Select * from users").ToList<users>();
            }
            foreach (var item in list)
            {
                UsersDetail u = new UsersDetail();
                u.UserID = item.UserID;
                u.Name = item.UserName;
                string HiddenMail = "";
                string HiddenAdress = "";
                string HiddenPhone = "";
                char[] Mail = item.UserMail.ToCharArray();
                char[] Adress = item.UserAdress.ToCharArray();
                char[] Phone = item.UserPhone.ToCharArray();
                int count = 0;
                int length = Mail.Length;
                foreach (char c in Mail)
                {
                    if (count < 2)
                    {
                        count++;
                        HiddenMail += c;
                    }
                    else if (count < length - 10)
                    {
                        count++;
                        HiddenMail += "*";
                    }
                    else
                    {
                        count++;
                        HiddenMail += c;
                    }
                }
                u.Mail = HiddenMail;
                count = 0;
                length = Adress.Length;
                foreach (char c in Adress)
                {
                    if (count < 3)
                    {
                        count++;
                        HiddenAdress += c;
                    }
                    else if (count < length - 3)
                    {
                        count++;
                        HiddenAdress += "*";
                    }
                    else
                    {
                        count++;
                        HiddenAdress += c;
                    }
                }
                u.Adress = HiddenAdress;
                count = 0;
                length = Phone.Length;
                foreach (char c in Phone)
                {
                    if (count < 2)
                    {
                        count++;
                        HiddenPhone += c;
                    }
                    else if (count < length - 4)
                    {
                        count++;
                        HiddenPhone += "*";
                    }
                    else
                    {
                        count++;
                        HiddenPhone += c;
                    }
                }
                u.Phone = HiddenPhone;
                HiddenList.Add(u);
            }

            return PartialView(HiddenList);
        }
        public PartialViewResult OrdersPartial()
        {
            List<OrderNames> list = new List<OrderNames>();
            using (var db = new RestaurantEntities10())
            {
                var foodDet = db.Food.Join(db.AdminOrder, f => f.FoodID, r => r.Food.FoodID, (_f, _r) => new
                {
                    FoodID = _f.FoodID,
                    FoodName = _f.FoodName,
                    OrderID = _r.AdminOrderID,
                    date = _r.OrderDate,
                    sit = _r.OrderSituation,
                    amount = _r.OrderAmount,
                    price = _f.FoodPrice,
                    id = _r.UserID,
                    AdresId = _r.OrderAddress,
                    Phone = _r.OrderPhone
                }).ToList();
                var userDet = db.users.Join(db.AdminOrder, f => f.UserID, r => r.users.UserID, (_f, _r) => new
                {
                    UserID = _f.UserID,
                    UserName = _f.UserName,
                    OrID = _r.AdminOrderID
                }).ToList();
                foreach (var item in foodDet)
                {
                    OrderNames o = new OrderNames();
                    o.FoodName = item.FoodName;
                    o.date = (DateTime)item.date;
                    o.Price = item.price;
                    o.Amount = (int)item.amount;
                    o.UserID = item.id;
                    var adr = (from a in db.Addresses where a.AddressID == item.AdresId select a).FirstOrDefault();
                    o.Adres = adr.Address;
                    o.Phone = item.Phone;
                    if (item.sit == 0)
                    {
                        o.Situation = "Onay Bekliyor";
                        o.Sit = "Onayla";
                    }
                    else if (item.sit == 1)
                    {
                        o.Situation = "Hazırlanıyor";
                        o.Sit = "Kuryeyi Çağır";
                    }
                    else if (item.sit == 2)
                    {
                        o.Situation = "Kurye Bekleniyor";
                    }
                    else if (item.sit == 3)
                    {
                        o.Situation = "Yola Çıktı";

                    }
                    else if (item.sit == 4)
                    {
                        o.Situation = "Teslim Edildi";
                    }
                    else
                    {
                        o.Situation = "İptal Edildi";
                    }
                    foreach (var item1 in userDet)
                    {
                        if (item.OrderID == item1.OrID)
                        {
                            o.CustomerName = item1.UserName;
                            break;
                        }
                    }
                    list.Add(o);
                }
            }
            list.Reverse();
            return PartialView(list);
        }
        public JsonResult Situation(Situation s)
        {
            int sit = s.UserID;
            DateTime d = s.date;
            using (var db = new RestaurantEntities10())
            {
                var list = db.AdminOrder.ToList();
                foreach (var item in list)
                {
                    if (sit == item.UserID && d == item.OrderDate && s.number != 5)
                    {
                        if (item.OrderSituation != 4)
                        {
                            int sit1 = (int)item.OrderSituation;
                            int nwSit = sit1 + 1;
                            int a = db.Database.ExecuteSqlCommand("Update AdminOrder set OrderSituation =" + nwSit + "where UserID=" + item.UserID + "and OrderDate='" + item.OrderDate + "'");
                        }

                    }
                    else if (sit == item.UserID && d == item.OrderDate && s.number == 5)
                    {
                        int a = db.Database.ExecuteSqlCommand("Update AdminOrder set OrderSituation =" + 5 + "where UserID=" + item.UserID + "and OrderDate='" + item.OrderDate + "'");
                        var upInvent = (from i in db.AdminOrder where i.OrderSituation == 5 && i.OrderDate == d && i.UserID == sit select i).ToList();
                        var Inventory = (from b in db.Inventory select b).ToList();
                        foreach (var o in upInvent)
                        {
                            foreach (var i in Inventory)
                            {
                                if (o.FoodID == i.FoodID)
                                {
                                    var newPiece = i.InventoryPiece + o.OrderAmount;
                                    int b = db.Database.ExecuteSqlCommand("Update Inventory set InventoryPiece =" + newPiece + "where FoodID=" + i.FoodID);
                                }
                            }
                        }
                    }
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult KodGönder(UsersDetail st)
        {
            onay = rd.Next(1000, 10000);
            string fourNumber = st.Phone.ToString().Substring(7);
            return Json(new { success = true, message = "Sonu " + fourNumber + " olan telefona doğrulama kodu gönderildi!", kod = onay }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KodOnay(UsersDetail s)
        {
            if (s.Kod == onay)
            {
                string mail = "";
                string phone = "";
                using (var db = new RestaurantEntities10())
                {
                    var list = (from a in db.users where a.UserID == s.UserID select a).FirstOrDefault();
                    mail = list.UserMail;
                    phone = list.UserPhone;
                }
                return Json(new { success = true, message = "Onaylandı!", Mail = mail, Phone = phone }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Kodu Kontrol Ediniz!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Filter(aFil f)
        {
            Session["filter"] = f;

            return Json(JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AFilterPartial()
        {
            return PartialView();
        }
        public ActionResult Cupons()
        {

            return View();
        }
        public PartialViewResult CuponPartial()
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            List<Cupon> cpnList = new List<Cupon>();
            var list = db.Kupon.ToList();
            foreach (var item in list)
            {
                Cupon c = new Cupon();
                c.ID = item.KuponID;
                c.KuponKodu = item.Kupon1;
                c.KuponName = item.KuponName;
                c.KuponTutarı = item.KuponTutar;
                cpnList.Add(c);
            }
            return PartialView(cpnList);
        }
        public JsonResult AddCpn(Cupon c)
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            int b = db.Database.ExecuteSqlCommand("insert into Kupon(Kupon,Durum,KuponName,KuponTutar) values('" + c.KuponKodu + "'," + 0 + ",'" + c.KuponName + "'," + c.KuponTutarı + ")");
            return Json(new { success = true, message = "Başarıyla Eklendi!" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveCpn(Cupon c)
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            int a = db.Database.ExecuteSqlCommand("delete from Kupon where KuponID=" + c.ID);
            return Json(new { success = true, message = "Başarıyla Silindi!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Coruier()
        {
            return View();
        }
        public PartialViewResult CoruierPartial()
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            List<KuryeInfo> courirerList = new List<KuryeInfo>();
            var list = db.Kurye.ToList();
            foreach (var item in list)
            {
                KuryeInfo c = new KuryeInfo();
                c.CoruierPhone = "+9";
                c.KuryeNumber = (int)item.KuryeNumber;
                c.ID = item.KuryeID;
                c.CoruierPhone += item.KuryePhone;
                courirerList.Add(c);
            }
            return PartialView(courirerList);
        }
        public JsonResult AddCoruier(KuryeInfo cr)
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            var control = (from a in db.Kurye where a.KuryeNumber == cr.KuryeNumber select a).FirstOrDefault();
            if (control == null)
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                string psw = cr.Password;
                string pswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(psw)));
                Kurye x = new Kurye();
                x.KuryeNumber = cr.KuryeNumber;
                x.KuryePassword = pswHash;
                x.KuryePhone = cr.CoruierPhone;
                db.Kurye.Add(x);
                db.SaveChanges();
                return Json(new { success = true, message = "Başarıyla Eklendi!" }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { success = false, message = "Bu Kurye Numarası ile Bir Kurye Zaten Var!" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult RemoveCoruier(KuryeInfo c)
        {
            RestaurantEntities10 db = new RestaurantEntities10();
            var cr = (from a in db.Kurye where a.KuryeID == c.ID select a).FirstOrDefault();
            db.Kurye.Remove(cr);
            db.SaveChanges();
            return Json(new { success = true, message = "Başarıyla Silindi!" }, JsonRequestBehavior.AllowGet);
        }

    }
}