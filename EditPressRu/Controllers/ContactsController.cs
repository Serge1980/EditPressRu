using EditPressRu.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class ContactsController : BaseController
    {
        // GET: Default
        
        public async Task<ActionResult> Index()
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "Contacts").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "Contacts").MetaDesk;
            ViewBag.PageName = "Contacts";
            ContacstViewModel model = new ContacstViewModel();
            model.TopSalesList = await db.GetTopSalesListForContacts();
            return View(model);
        }
    }
}