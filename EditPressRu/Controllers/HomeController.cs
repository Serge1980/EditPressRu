using System.Web.Mvc;
using EditPressRu.Models;
using System.Web.UI;

namespace EditPressRu.Controllers
{
    [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
    public class HomeController : BaseController
    {
        
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.metakeywords = db.GetMetaTags(0,0,"Index").MetaKey;
            ViewBag.PageName = "Index";
            //ViewBag.Categories = db.GetCategories();
            IndexProductsViewModel model = db.GetIndexProductsViewModel();
            
            return  View(model);
        }

        

    }
}