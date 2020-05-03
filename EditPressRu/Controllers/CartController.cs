using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using EditPressRu.Filters;
using EditPressRu.Models.DB;
using EditPressRu.Models;
using System.Web;

namespace EditPressRu.Controllers
{
    public class CartController : BaseController
    {
        public EditPressRuEntities Db = new EditPressRuEntities();

        [InitializeSimpleMembership]
        [HttpPost]
        public ActionResult AddToCart(int ProductId, int Count)
        {
            //Идет две проверки  - авторизован пользователь или нет, если авторизован то все по UserID иначе все по Sid
            string Sid = Session.SessionID;
            int UserId = 0;
            int orderId = 0;

            if (User.Identity.IsAuthenticated)
            {
                UserId = Db.UserProfile.SingleOrDefault(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            }
            

            decimal Price = Db.Products.SingleOrDefault(x => x.Id == ProductId).Price;
            

            List<OrderDetails> listOrders = new List<OrderDetails>();

            //1) Проверить есть ли открытая корзина
            if (UserId>0)
            {
                listOrders = Db.OrderDetails.Where(x => x.Orders.UserID == UserId && x.Orders.StatusId == 7).ToList();
            }
            else
            {
                listOrders = Db.OrderDetails.Where(x => x.Orders.Sid == Sid && x.Orders.StatusId == 7).ToList();
            }
            
            //2) Если есть, 
            if (listOrders!=null && listOrders.Count>0)
            {
                //то проверить нет ли в ней уже такого товара?
                OrderDetails tovar = new OrderDetails();
                try
                {
                    tovar = listOrders.FirstOrDefault(x => x.ProductId == ProductId);
                }
                catch 
                {
                }
              

                //3) Если есть отредактировать товар,
                if (tovar != null)
                {
                    tovar.Count = tovar.Count+Count;
                    Db.SaveChanges();

                    

                    if (UserId > 0)
                    {
                        orderId = Db.OrderDetails.Where(x => x.Orders.UserID == UserId && x.Orders.StatusId == 7).FirstOrDefault().OrderID;
                    }
                    else
                    {
                        orderId = Db.OrderDetails.Where(x => x.Orders.Sid == Sid && x.Orders.StatusId == 7).FirstOrDefault().OrderID;
                    }


                    return Json(new { orderId = orderId });

                }

                // если нет то добавить
                else
                {
                    OrderDetails newtovar = new OrderDetails();
                    newtovar.ItemPrice = Price;
                    newtovar.OrderID = listOrders.FirstOrDefault().OrderID;
                    newtovar.Count = Count;
                    newtovar.ProductId = ProductId;

                    ProdImages image = Db.ProdImages.FirstOrDefault(x => x.ProdId == ProductId && x.Main);

                    if (!String.IsNullOrEmpty(image.Small))
                    {
                        newtovar.Img = image.Small;
                    }
                    else if (!String.IsNullOrEmpty(image.ThumbNail))
                    {
                        newtovar.Img = image.ThumbNail;
                    }
                    else
                    {
                        newtovar.Img = image.Big;
                    }

                    Db.OrderDetails.Add(newtovar);
                    Db.SaveChanges();

                    if (UserId > 0)
                    {
                        orderId = Db.OrderDetails.Where(x => x.Orders.UserID == UserId && x.Orders.StatusId == 7).FirstOrDefault().OrderID;
                    }
                    else
                    {
                        orderId = Db.OrderDetails.Where(x => x.Orders.Sid == Sid && x.Orders.StatusId == 7).FirstOrDefault().OrderID;
                    }

                    return Json(new { orderId = orderId });
                }

            }
            
            //4) Если нет открытой корзины, то создать новый заказ
            else
            {
                Orders newOrder = new Orders();
                OrderDetails ci = new OrderDetails();

                newOrder.Data = DateTime.Now;
                newOrder.moscow_shipment = 0;
                newOrder.UserID = UserId;

                if (UserId == 0)
                {
                    newOrder.Sid = Sid;
                }

                newOrder.StatusId = 7;

                ci.Count = Count;
                ci.ProductId = ProductId;
                ci.ItemPrice = Price;

                ProdImages image = Db.ProdImages.FirstOrDefault(x => x.ProdId == ProductId && x.Main);

                if (!String.IsNullOrEmpty(image.Small))
                {
                    ci.Img = image.Small;
                }
                else if (!String.IsNullOrEmpty(image.ThumbNail))
                {
                    ci.Img = image.ThumbNail;
                }
                else
                {
                    ci.Img = image.Big;
                }

               

                Db.Orders.Add(newOrder);
                Db.SaveChanges();

                ci.OrderID = newOrder.OrderID;
                Db.OrderDetails.Add(ci);
                Db.SaveChanges();

                return Json(new { orderId = newOrder.OrderID });

            }

        }

        //[HttpPost]
        //public ActionResult AddToCart(int ProductId, int Count, string priznak)
        //{
        //    //**priznak**
        //    //prot-образцы
        //    //cart-корзина
        //    //maket-заказ макета

        //    //Идет две проверки  - авторизован пользователь или нет, если авторизован то все по UserID иначе все по Sid
        //    string Sid = Session.SessionID;
        //    int UserId = 0;

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        UserId = Db.UserProfile.SingleOrDefault(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
        //    }

        //    decimal Price = Db.Products.SingleOrDefault(x => x.Id == ProductId).Price;



        //    //нет ли уже такого в образцах?
        //    if (priznak == "prot")
        //    {
        //        OrderDetails prot_test = new OrderDetails();
        //        try
        //        {
        //            decimal Summ = 0;
        //            if (UserId > 0)
        //            {
        //                prot_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.UserID == UserId && x.Orders.StatusId == 6);
        //                Summ = Db.OrderDetails.Where(x => x.Orders.UserID == UserId && x.Orders.StatusId == 6).Sum(x => x.ItemPrice);
        //            }
        //            else
        //            {
        //                prot_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.Sid == Sid && x.Orders.StatusId == 6);
        //                Summ = Db.OrderDetails.Where(x => x.Orders.Sid == Sid && x.Orders.StatusId == 6).Sum(x => x.ItemPrice);
        //            }

        //            // На образцы количество равно 1шт.
        //            if (prot_test != null && prot_test.Count > 0)
        //            {
        //                return Json(new { sum = Summ });
        //            }

        //        }
        //        catch
        //        {
        //        }
        //    }


        //    //нет ли уже такого в макета?
        //    if (priznak == "maket")
        //    {
        //        Price = 0;
        //        OrderDetails mak_test = new OrderDetails();
        //        try
        //        {
        //            if (UserId > 0)
        //            {
        //                mak_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.UserID == UserId && x.Orders.StatusId == 5);
        //            }
        //            else
        //            {
        //                mak_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.Sid == Sid && x.Orders.StatusId == 5);
        //            }

        //            //На макеты нет цены
        //            if (mak_test != null && mak_test.Count > 0)
        //            {
        //                return Json(new { sum = 0 });
        //            }
        //            //если не нашли такой продукт в макетах на редакцию - то два пути - 1)найти макет на редакцию StatusId=5  и добавить сюда новый продукт, 
        //            //либо 2) Создать новый заказ c макетами

        //            //1)



        //        }
        //        catch
        //        {
        //        }
        //    }


        //    //нет ли уже такого в товар в корзине?
        //    if (priznak == "cart")
        //    {
        //        OrderDetails tov_test = new OrderDetails();
        //        try
        //        {
        //            if (UserId > 0)
        //            {
        //                tov_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.UserID == UserId && x.Orders.StatusId == 7);
        //            }
        //            else
        //            {
        //                tov_test = Db.OrderDetails.FirstOrDefault(x => x.ProductId == ProductId && x.Orders.Sid == Sid && x.Orders.StatusId == 7);
        //            }


        //        }
        //        catch
        //        {
        //        }
        //    }


        //    Orders newOrder = new Orders();
        //    OrderDetails ci = new OrderDetails();

        //    newOrder.Data = DateTime.Now;
        //    newOrder.moscow_shipment = 0;
        //    newOrder.UserID = UserId;

        //    if (UserId == 0)
        //    {
        //        newOrder.Sid = Sid;
        //    }


        //    switch (priznak)
        //    {
        //        case "prot":
        //            newOrder.StatusId = 6;
        //            break;
        //        case "maket":
        //            newOrder.StatusId = 5;
        //            break;
        //        default:
        //            //cart
        //            newOrder.StatusId = 7;
        //            break;
        //    }

        //    ci.Count = Count;
        //    ci.ProductId = ProductId;
        //    ci.ItemPrice = Price;

        //    ProdImages image = Db.ProdImages.FirstOrDefault(x => x.ProdId == ProductId && x.Main);

        //    if (!String.IsNullOrEmpty(image.Small))
        //    {
        //        ci.Img = image.Small;
        //    }
        //    else if (!String.IsNullOrEmpty(image.ThumbNail))
        //    {
        //        ci.Img = image.ThumbNail;
        //    }
        //    else
        //    {
        //        ci.Img = image.Big;
        //    }

        //    decimal ItemSumm = Count * Price;


        //   //Этап возвращения результата операции в зависимости от обработки

           
           
        //    else if (tov_test != null && tov_test.Orders.StatusId == ci.Orders.StatusId && tov_test.ProductId == ci.ProductId)
        //    {
        //        int newCnt = tov_test.Count + Count;
        //        tov_test.Count = newCnt;
        //        decimal Summ = Price * newCnt;

        //        if (UserId > 0)
        //        {
        //            Orders tovOrder = Db.Orders.SingleOrDefault(x => x.OrderID == tov_test.OrderID);
        //            tovOrder.UserID = Db.UserProfile.SingleOrDefault(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
        //        }

        //        Db.SaveChanges();
        //        return Json(new { sum = Summ });
        //    }
        //    else
        //    {
        //        Db.Orders.Add(newOrder);
        //        Db.SaveChanges();

        //        ci.OrderID = newOrder.OrderID;
        //        Db.OrderDetails.Add(ci);
        //        Db.SaveChanges();

        //        return Json(new { sum = ItemSumm });
        //    }

        //}


        [HttpGet]
        [AllowJsonGet]
        public JsonResult GetCartSumm()
        {
            string Sid = Session.SessionID;
            int UserId = 0;

            //флаг - авторизован или нет
            byte isa = 0;

            if (User.Identity.IsAuthenticated)
            {
                UserId = Db.UserProfile.SingleOrDefault(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
                isa = 1;
            }

            //Получим список товаров в корзине в заисимости авторизован пользователь или нет
            List<OrderDetails> list = new List<OrderDetails>();

            if (UserId>0)
            {
                list = Db.OrderDetails.Where(c => c.Orders.UserID== UserId && c.Orders.StatusId==7).ToList();
            }
            else
            {
                list = Db.OrderDetails.Where(c => c.Orders.Sid == Sid && c.Orders.StatusId == 7).ToList();
            }
            
            decimal Totalsumm = 0;
            decimal TovarCount = 0;
            int OrderId = 0;

            bool Result = false;
            if (list.Count > 0)
            {
                OrderId = list.FirstOrDefault().OrderID;

                foreach (OrderDetails rec in list)
                {
                    Totalsumm = Totalsumm + (rec.ItemPrice*rec.Count);
                    TovarCount = TovarCount + rec.Count;
                }
                Result = true;
            }

            return Json(new { orderId= OrderId, summa = Totalsumm, total = TovarCount, Result = Result,isa=isa });
        }


    }
}
