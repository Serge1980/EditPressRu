using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class ProizvodstvoController : BaseController
    {
        // GET: Default
        public ActionResult Index()
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "Proizvodstvo").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "Proizvodstvo").MetaDesk;
            return View();
        }

    }
}