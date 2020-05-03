using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Controllers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EditPressRu.Areas.Adminka.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrdersAdminController : BaseController
    {
        private EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Adminka/Orders
        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            int pageSize = 30;
            OrdersViewModel model = new OrdersViewModel();

            model.Pr = "";
            model.User = "";
            model.Number = 0;
            model.Dat = "";
            model.PrSort = "";

            //IQueryable<OrderUser> query = Db.Orders.Select(x => new OrderUser { Order = x, User = Db.UserProfile.FirstOrDefault(c => c.UserId.Equals(x.UserID)) });

            IQueryable<OrderUser> query = Db.Orders.Select(x => new OrderUser
            {
                OrderId = x.OrderID,
                OrderDate = x.Data,
                StatusId = x.StatusId,
                StatusDescr=Db.OrdersStatus_spr.FirstOrDefault(c=>c.Id==x.StatusId).Description,
                UserId = x.UserID,
                UserName = Db.UserProfile.FirstOrDefault(c => c.UserId == x.UserID).UserName
            });


            List<OrderUser> listRez = query.ToList();

            model.OrderUserList = listRez.Skip((page - 1) * pageSize).OrderByDescending(x => x.OrderDate).ThenBy(x => x.StatusId).Take(pageSize).ToList();

            model.Pager = new Helpers.PageInfo();
            model.Pager.page = page;
            model.Pager.PageSize = pageSize;
            model.Pager.TotalItems = listRez.Count;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int page = 1, string pr = "", string user = "", int number = 0, string dat = "", string sortPar = "numDesc")
        {
            int pageSize = 30;
            OrdersViewModel model = new OrdersViewModel();

            model.Pr = pr;
            model.User = user;
            model.Number = number;
            model.Dat = dat;
            model.PrSort = sortPar;

            //IQueryable<OrderUser> query = Db.Orders.Select(x => new OrderUser { Order = x, User = Db.UserProfile.FirstOrDefault(c => c.UserId.Equals(x.UserID)) });

            IQueryable<OrderUser> query = Db.Orders.Select(x => new OrderUser { OrderId=x.OrderID,
                OrderDate =x.Data,
                StatusId=x.StatusId,
                UserId=x.UserID,
                UserName =Db.UserProfile.FirstOrDefault(c=>c.UserId==x.UserID).UserName });

            //IQueryable<OrderUser> query = Db.Database.SqlQuery<OrderUser>("select * from Order").AsQueryable();

            if (pr == "C")
            {
                query = query.Where(x => x.StatusId==1);
            }

            if (pr == "M")
            {
                query = query.Where(x => x.StatusId == 2);
            }

            if (pr == "P")
            {
                query = query.Where(x => x.StatusId == 3);
            }

            if (user != "")
            {
                var tryQuery = query.Where(x => x.UserName.Trim().ToLower().Contains(user.Trim().ToLower()));
                if (tryQuery.Count() > 0)
                {
                    query = tryQuery; //query.Where(x => x.User.UserName.Trim().ToLower().Contains(user.Trim().ToLower()));
                }


            }

            if (number != 0)
            {
                var tryQuery = query.Where(x => x.OrderId == number);
                if (tryQuery.Count() > 0)
                {
                    query = tryQuery; //query.Where(x => x.Order.OrderNumber.Trim().ToLower().Contains(number.Trim().ToLower()));
                }
            }

            if (dat != "")
            {
                DateTime date;
                if (DateTime.TryParse(dat, out date))
                {
                    var tryQuery = query.Where(x => x.OrderDate >= date).Take(4).Union(query.Where(x => x.OrderDate <= date).Take(4));
                    int ttt = tryQuery.Count();
                    if (ttt > 0)
                    {
                        query = tryQuery;
                    }
                }

            }

            switch (sortPar)
            {
                case "user":
                    query = query.OrderBy(x => x.UserName);
                    break;
                case "userDesc":
                    query = query.OrderByDescending(x => x.UserName);
                    break;
                case "dat":
                    query = query.OrderBy(x => x.OrderDate);
                    break;
                case "datDesc":
                    query = query.OrderByDescending(x => x.OrderDate);
                    break;
                case "num":
                    query = query.OrderBy(x => x.OrderId);
                    break;
                case "numDesc":
                    query = query.OrderByDescending(x => x.OrderId);
                    break;
                default:
                    query = query.OrderByDescending(x => x.OrderId);
                    break;
            }


            List<OrderUser> listRez = query.ToList();

            model.OrderUserList = listRez.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            model.Pager = new Helpers.PageInfo();
            model.Pager.page = page;
            model.Pager.PageSize = pageSize;
            model.Pager.TotalItems = listRez.Count;

            return View(model);
        }

        public ActionResult Details(int id)
        {
            UserOrdersViewModel model = new UserOrdersViewModel();
            int UsId = Db.Orders.SingleOrDefault(x => x.OrderID.Equals(id)).UserID;
            model.UserName = "";
            try
            {
                model.UserName = Db.UserProfile.SingleOrDefault(x => x.UserId == UsId).UserName;
            }
            catch
            {
            }

           
            model.ListOrders = HelpFunctions.GetOrderCartModel(id,300,30000);
           
            return View(model);
        }

        public ActionResult OrderUser(int id)
        {
            OrderUserDetailsModel model = new OrderUserDetailsModel();
            model.OrderUser = Db.UserProfile.FirstOrDefault(x => x.UserId.Equals(id));
            model.UserInfo = Db.UserProfileDetails.FirstOrDefault(x => x.UserID.Equals(id));
            model.TotalCount = Db.Orders.Where(x => x.UserID.Equals(id)).Count();
            if (Db.OrderDetails.Where(x=>x.Orders.UserID==id).Count()>0)
            {
                model.TotalSumm = Db.Orders.Where(x => x.UserID.Equals(id)).Select(x => x.OrderDetails.Sum(y => y.Count * y.ItemPrice)).ToList().Sum();
            }
            else
            {
                model.TotalSumm = 0;
            }
           
            return View(model);
        }
    }
}