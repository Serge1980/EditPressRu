using EditPressRu.Models.DB;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class DeliveryTermsController : BaseController
    {
        // GET: Default
       
        public ActionResult Index()
        {
            ViewBag.PageName = "DeliveryTerms";
            List<Products> model = db.GetTopSales(62744);
            return View(model);
        }
    }
}