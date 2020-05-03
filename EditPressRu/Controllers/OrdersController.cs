using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace EditPressRu.Controllers
{
    public class OrdersController : BaseController
    {
        public EditPressRuEntities Db = new EditPressRuEntities();

        //priceDelivery
        private const decimal price = 300;
        private const decimal summa_free = 30000;


        // GET: Orders
        [Authorize]
        ///id=StatusId
        public ActionResult Index(int id = 0, int ordNumber = 0, string dayCnt = "")
        {
            List<OrderDetailsView> model = new List<OrderDetailsView>();

            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;

            IQueryable<Orders> list = Db.Orders.Where(x => x.UserID == userId);


            switch (dayCnt)
            {
                case "Сегодня":
                    list = list.Where(x => x.Data.Day == DateTime.Now.Day && x.Data.Month == DateTime.Now.Month && x.Data.Year == DateTime.Now.Year);
                    break;

                case "Вчера":
                    var dat1 = DateTime.Now.AddDays(-1);
                    list = list.Where(x => x.Data.Day == dat1.Day && x.Data.Month == dat1.Month && x.Data.Year == dat1.Year);
                    break;

                case "Неделя":
                    var dat2 = DateTime.Now.AddDays(-7);
                    var dat21 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat2 && x.Data <= dat21);
                    break;

                case "Две недели":
                    var dat3 = DateTime.Now.AddDays(-14);
                    var dat31 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat3 && x.Data <= dat31);
                    break;

                case "30 дней":
                    var dat4 = DateTime.Now.AddDays(-30);
                    var dat41 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat4 && x.Data <= dat41);
                    break;

                case "90 дней":
                    var dat5 = DateTime.Now.AddDays(-90);
                    var dat51 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat5 && x.Data <= dat51);
                    break;

                case "180 дней":
                    var dat6 = DateTime.Now.AddDays(-180);
                    var dat61 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat6 && x.Data <= dat61);
                    break;

                case "365 дней":
                    var dat7 = DateTime.Now.AddDays(-365);
                    var dat71 = DateTime.Now;
                    list = list.Where(x => x.Data >= dat7 && x.Data <= dat71);
                    break;

                case "Текущий месяц":
                    var month = DateTime.Now.Month;
                    list = list.Where(x => x.Data.Month == month);
                    break;

                case "Текущий год":
                    var year = DateTime.Now.Year;
                    list = list.Where(x => x.Data.Year == year);
                    break;

                case "Прошлый месяц":
                    var month1 = DateTime.Now.Month - 1;
                    list = list.Where(x => x.Data.Month == month1);
                    break;

                case "Прошлый год":
                    var year1 = DateTime.Now.Year - 1;
                    list = list.Where(x => x.Data.Year == year1);
                    break;

                case "":
                    dayCnt = "Весь период";
                    break;

                case "Весь период":
                    dayCnt = "Весь период";
                    break;
            }

            if (ordNumber > 0)
            {
                list = list.Where(x => x.OrderID.ToString().Contains(ordNumber.ToString()));
                var ttt = list.FirstOrDefault();
            }

            switch (id)
            {
                case 1:
                    list = list.Where(x => x.StatusId == 1);
                    break;

                case 2:
                    list = list.Where(x => x.StatusId == 2);
                    break;

                case 3:
                    list = list.Where(x => x.StatusId == 3 ||x.StatusId==6);
                    break;

                case 4:
                    list = list.Where(x => x.StatusId == 4 || x.StatusId == 7);
                    break;
            }

            List<int> listOrdersId = list.OrderByDescending(x => x.Data).Select(x => x.OrderID).ToList();

            foreach (var orderId in listOrdersId)
            {
                OrderDetailsView modelItem = HelpFunctions.GetOrderCartModel(orderId,price,summa_free, "index");

                modelItem.DayCnt = dayCnt;
                if (!modelItem.ModelIsEmpty)
                {
                    model.Add(modelItem);
                }

            }

            return View(model);
        }


        [Authorize]
        public ActionResult OrderDetails(int id)
        {
            OrderDetailsView model = HelpFunctions.GetOrderCartModel(id, price, summa_free);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult EditOrderAnms(int id = 0)
        {
            OrderDetailsView model = new OrderDetailsView();

            string Sid = Session.SessionID;

            List<OrderDetailsKucha> listOrderKucha = new List<OrderDetailsKucha>();

            if (id == 0)
            {
                listOrderKucha = Db.Database.SqlQuery<OrderDetailsKucha>(@"select c.OrderID as OrderId,c.StatusId,b.Article as Articul,
                                                                                        b.Id as ProductId,a.OrderDetailID as OrderDetailsId, b.ShName as ProdName,
                                                                                        a.ItemPrice,a.Img,a.[Count] as Cnt,a.[Count]*a.ItemPrice as ItemSumm,
                                                                                        c.Data as DateOrder,d.TotalSumm as TotalSummOrder from OrderDetails a
                                                                                        inner join Products b on a.ProductId=b.Id
                                                                                        inner join Orders c on a.OrderID=c.OrderID
                                                                                        inner join (select OrderID,sum(ItemPrice*Count) as TotalSumm  from OrderDetails dd group by OrderID) d on d.OrderID=c.OrderId
                                                                                        where c.StatusId=7 and c.Sid={0}", Sid).ToList();
            }
            else
            {
                listOrderKucha = Db.Database.SqlQuery<OrderDetailsKucha>(@"select c.OrderID as OrderId,c.StatusId,b.Article as Articul,
                                                                                        b.Id as ProductId,a.OrderDetailID as OrderDetailsId, b.ShName as ProdName,
                                                                                        a.ItemPrice,a.Img,a.[Count] as Cnt,a.[Count]*a.ItemPrice as ItemSumm,
                                                                                        c.Data as DateOrder,d.TotalSumm as TotalSummOrder from OrderDetails a
                                                                                        inner join Products b on a.ProductId=b.Id
                                                                                        inner join Orders c on a.OrderID=c.OrderID
                                                                                        inner join (select OrderID,sum(ItemPrice*Count) as TotalSumm  from OrderDetails dd group by OrderID) d on d.OrderID=c.OrderId
                                                                                        where c.StatusId=7 and c.OrderID={0}", id).ToList();
            }


            model.OrderDetail = listOrderKucha;

            if (listOrderKucha.Count > 0)
            {
                model.ModelIsEmpty = false;

                model.TotalItems = listOrderKucha.Sum(x => x.Cnt);

                int status = listOrderKucha.FirstOrDefault().StatusId;

                model.PriznakZakaz = Db.OrdersStatus_spr.SingleOrDefault(x => x.Id == status).Name;

                if (model.OrderDetail.FirstOrDefault().TotalSummOrder < 15000)
                {
                    model.Message = "Оформление покупки возможно при стоимости заказа от 15 000 рублей";
                }

            }
            else
            {
                model.ModelIsEmpty = true;
            }

            return View("EditOrder", model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditOrder(int id = 0)
        {
            OrderDetailsView model = new OrderDetailsView();

            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;

            if (id == 0)
            {
                model.ModelIsEmpty = true;
            }
            else
            {

                //1) Редактируемый перевести в корзину -4-->7, 2-->5,1-->7,3-->6
                Orders order = Db.Orders.SingleOrDefault(x => x.OrderID == id);

                switch (order.StatusId)
                {
                    case 3:

                        //1) Если есть открытый макет на редактирование - сохранить его statusId 6-->3
                        try
                        {
                            Orders orderProt = Db.Orders.FirstOrDefault(x => x.UserID == userId && x.StatusId == 6);
                            orderProt.StatusId = 3;
                            Db.SaveChanges();
                        }
                        catch
                        {
                        }

                        order.StatusId = 6;
                        Db.SaveChanges();

                        break;

                    case 4:

                        //1) Если есть открытая корзина - сохранить ее statusId 7-->4,6-->3
                        try
                        {
                            Orders orderCart = Db.Orders.FirstOrDefault(x => x.UserID == userId && x.StatusId == 7);
                            orderCart.StatusId = 4;
                            Db.SaveChanges();
                        }
                        catch
                        {
                        }

                        order.StatusId = 7;
                        Db.SaveChanges();

                        break;
                    case 1:

                        //1) Если есть открытая корзина - сохранить ее statusId 7-->4,6-->3
                        try
                        {
                            Orders orderCart = Db.Orders.FirstOrDefault(x => x.UserID == userId && x.StatusId == 7);
                            orderCart.StatusId = 4;
                            Db.SaveChanges();
                        }
                        catch
                        {
                        }

                        order.StatusId = 7;
                        Db.SaveChanges();

                        break;
                }

                model = HelpFunctions.GetOrderCartModel(id, price, summa_free);
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult EditOrder(int OrderDetId, int Count)
        {
            OrderDetails orderDetails = Db.OrderDetails.SingleOrDefault(x => x.OrderDetailID == OrderDetId);
            orderDetails.Count = Count;
            Db.SaveChanges();

            OrderDetailsView model = HelpFunctions.GetOrderCartModel(orderDetails.OrderID, price, summa_free);

            return PartialView("_PartialOrder", model);
        }

        [HttpGet]
        public ActionResult CopyOrder(int id)
        {
            int newOrderId = Db.Database.SqlQuery<int>("exec CopyOrder @OrderId={0}, @StatusId=4", id).FirstOrDefault();

            return Redirect(String.Format("/orders/orderdetails/{0}", newOrderId));
        }

        [HttpGet]
        public ActionResult NewPrototype(int id)
        {
            int newOrderId = Db.Database.SqlQuery<int>("exec CopyOrder @OrderId={0}, @StatusId=3", id).FirstOrDefault();

            return Redirect(String.Format("/orders/orderdetails/{0}", newOrderId));
        }

        //Обработка доставки

        [Authorize]
        [HttpPost]
        public ActionResult OrderSetDelivery(int id, string adress)
        {
            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;



            UserProfileDetails userProf = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == userId);
            userProf.ShipmentAddress = adress;
            Db.SaveChanges();

            Orders order = Db.Orders.SingleOrDefault(x => x.OrderID == id);
            order.moscow_shipment = 1;
            Db.SaveChanges();

            List<OrderDetails> listOrders = Db.OrderDetails.Where(x => x.OrderID == id).ToList();

            decimal summ = listOrders.Where(x => x.ProductId != 0).Sum(x => x.ItemPrice * x.Count);

            //Проверим нет ли уже заказанной доставки
            bool deliveryYes = false;

            if (listOrders.FirstOrDefault(x => x.ProductId == 0) != null)
            {
                deliveryYes = true;
            }

            if (!deliveryYes)
            {
                OrderDetails ordInfo = new OrderDetails();

                ordInfo.Count = 1;
                ordInfo.Descripts = "";
                ordInfo.Img = "/images/deliveryRussia.png";

                if (summ >= summa_free)
                {
                    ordInfo.ItemPrice = 0;
                }
                else
                {
                    ordInfo.ItemPrice = price;
                }

                ordInfo.OrderID = id;
                ordInfo.ProductId = 0;

                Db.OrderDetails.Add(ordInfo);
                Db.SaveChanges();

            }

            else
            {
                if (summ >= summa_free)
                {
                    OrderDetails ordInfo = listOrders.FirstOrDefault(x => x.ProductId == 0);

                    if (ordInfo.ItemPrice == price)
                    {
                        ordInfo.ItemPrice = 0;
                        Db.SaveChanges();
                    }
                }
            }


            OrderDetailsView model = HelpFunctions.GetOrderCartModel(id,price,summa_free);

            return PartialView("_PartialOrder", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OrderResDelivery(int id)
        {
            OrderDetailsView model = new OrderDetailsView();
            try
            {
                Orders order = Db.Orders.SingleOrDefault(x => x.OrderID == id);
                order.moscow_shipment = 0;
                Db.SaveChanges();

                OrderDetails ordInfo = Db.OrderDetails.FirstOrDefault(x => x.ProductId == 0 && x.OrderID == id);
                Db.OrderDetails.Remove(ordInfo);
                Db.SaveChanges();
            }
            catch
            {
                model = HelpFunctions.GetOrderCartModel(id, price, summa_free);

                return PartialView("_PartialOrder", model);
            }


            model = HelpFunctions.GetOrderCartModel(id, price, summa_free);

            return PartialView("_PartialOrder", model);
        }


        [Authorize]
        public ActionResult SaveDruftOrder(FormCollection collection)
        {
            //string value = Convert.ToString(collection["inputName"]);

            //string Sid = Session.SessionID;

            int UserId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;

            int statusId = int.Parse(collection["statusId"]);
            int orderId = int.Parse(collection["orderId"]); ;

            sbyte rez = HelpFunctions.SaveOrder(UserId, orderId, null, statusId);


            if (rez == -1)
            {
                return RedirectToAction("editorder", "orders", new { id = orderId, message = "Оформление заказа возможно при сумме покупки от 15 000 рублей" });
            }

            return RedirectToAction("OrderDetails", "Orders", new { id = orderId });

        }


        public ActionResult SaveCloseOrder(int id)
        {
            SaveCloseOrderViewModel model = new SaveCloseOrderViewModel();

            Orders order = Db.Orders.SingleOrDefault(x => x.OrderID == id);

            switch (order.StatusId)
            {
                case 7:
                    order.StatusId = 4;
                    Db.SaveChanges();

                    break;
                case 6:
                    order.StatusId = 3;
                    Db.SaveChanges();

                    break;
            }

            model.OrderId = id;

            return View(model);
        }

        [Authorize]
        public ActionResult OrderDelete(int id)
        {
            DeleteOrderFunction(id);

            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpPost]
        public ActionResult DelOrderDetails(int id)
        {
            int orderId = Db.OrderDetails.SingleOrDefault(x => x.OrderDetailID == id).OrderID;

            int cnt = Db.OrderDetails.Where(x => x.OrderID == orderId && x.ProductId != 0).Count();

            if (cnt > 1)
            {
                Db.Database.ExecuteSqlCommand(@"DELETE FROM OrderDetails WHERE OrderDetailID={0}", id);

                OrderDetailsView model = HelpFunctions.GetOrderCartModel(orderId, price, summa_free);

                return PartialView("_PartialOrder", model);
            }
            else
            {
                DeleteOrderFunction(orderId);
                OrderDetailsView model = HelpFunctions.GetOrderCartModel(orderId, price, summa_free);
                return PartialView("_PartialOrder", model);
            }

        }
        

        private void DeleteOrderFunction(int orderId)
        {
            List<OrderDetails> orderDetailList = Db.OrderDetails.Where(c => c.OrderID == orderId).Select(c => c).ToList();
            foreach (var item in orderDetailList)
            {
                Db.OrderDetails.Remove(item);
            }
            Db.SaveChanges();
            Orders order = Db.Orders.First(c => c.OrderID == orderId);

            //если есть коммерческие к этому заказу  - удалим их
            try
            {
                KP_Store kpStore = Db.KP_Store.FirstOrDefault(x => x.OrderId == order.OrderID);
                Db.Database.ExecuteSqlCommand(@"delete from Kp_Store_dop where KpId={0}", kpStore.Id);
                Db.KP_Store.Remove(kpStore);
                Db.SaveChanges();
            }
            catch
            {
            }


            Db.Orders.Remove(order);
            Db.SaveChanges();

        }


    }
}