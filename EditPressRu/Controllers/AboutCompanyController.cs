using EditPressRu.Models.DB;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;

namespace EditPressRu.Controllers
{
    [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
    public class AboutCompanyController : BaseController
    {
        // GET: Default
        public ActionResult Index()
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "AboutCompany").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "AboutCompany").MetaDesk;
            ViewBag.PageName = "AboutCompany";
            List<Products> model = db.GetTopSales(62744);
            return View(model);
        }
    }
}