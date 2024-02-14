using Restaurant1.Models;
using Restaurant1.Models.admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Restaurant1.Controllers
{
    public class KuryeController : Controller
    {
        public ActionResult YenıIndex()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Index(KuryeInfo k)
        {
            var list = new List<Kurye>();
            using (var db = new RestaurantEntities12())
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                string psw = k.Password;
                string pswHash = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(psw)));
                list = db.Database.SqlQuery<Kurye>("Select * from Kurye a where a.KuryeNumber='" + k.KuryeNumber + "' and a.KuryePassword='" + pswHash + "'").ToList<Kurye>();
            }
            if (list.Count() > 0)
            {
                Kurye b = list.First();
                Session["w"] = b;
                return RedirectToAction("Orders", "Kurye");
            }
            else
            {
                k.Error = "Hatalı şifre veya Email girdiniz.";
            }
            return View(k);
        }
        public ActionResult Orders()
        {

            return View();
        }
        public PartialViewResult OrdersPartial()
        {
            List<OrderNames> list = new List<OrderNames>();
            using (var db = new RestaurantEntities12())
            {
                var foodDet = db.Food.Join(db.AdminOrder, f => f.FoodID, r => r.Food.FoodID, (_f, _r) => new
                {
                    FoodID = _f.FoodID,
                    FoodName = _f.FoodName,
                    OrderID = _r.AdminOrderID,
                    date = _r.OrderDate,
                    sit = _r.OrderSitutation,
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
            using (var db = new RestaurantEntities12())
            {
                var list = db.AdminOrder.ToList();
                foreach (var item in list)
                {
                    if (sit == item.UserID && d == item.OrderDate && s.number != 5)
                    {
                        if (item.OrderSitutation != 4)
                        {
                            int sit1 = (int)item.OrderSitutation;
                            int nwSit = sit1 + 1;
                            int a = db.Database.ExecuteSqlCommand("Update AdminOrder set OrderSitutation =" + nwSit + "where UserID=" + item.UserID + "and OrderDate='" + item.OrderDate + "'");
                        }

                    }
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Quit()
        {
            return RedirectToAction("Login", "Login");
        }
    }
}