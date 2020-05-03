using EditPressRu.Helpers;
using EditPressRu.Repository;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class BaseController : Controller
    {
        public DataRepository db = new DataRepository();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //всегда будем иметь под рукой номер заказа с открытой корзиной --ViewBag.OrderId
            //**********************************************
            string Sid = Session.SessionID;
            int userId = 0;

            if (User.Identity.IsAuthenticated)
            {
                string userName = filterContext.HttpContext.User.Identity.Name;

                userId = db.GetCurrentUserIdbyName(userName);
            }

            ViewBag.OrderId = 0;

            try
            {
                ViewBag.OrderId = db.GetCartOrderId(userId, Sid);
            }
            catch 
            {
            }
            
            //------------------------------------------------

            ViewBag.Categories = db.GetCategories();
            ViewBag.Cityes = db.GetCityes();
            //ViewBag.Msg = "ttt"; //db.GetTopSales();
            ViewBag.LstProd = db.GetTopSales(62744);
            string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            ViewBag.IsMobile= TestDeviceType.IsMobileBrowser(userAgent);
            
        }

        

    }
}