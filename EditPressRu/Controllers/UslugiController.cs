using EditPressRu.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace EditPressRu.Controllers
{
    [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
    public class UslugiController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "Uslugy").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "Uslugy").MetaDesk;
            ViewBag.PageName = "Uslugy";
            return View();
        }

        public ActionResult OfsetPrint()
        {
            return View();
        }
        public ActionResult DigitPrint()
        {
            return View();
        }

        public ActionResult LogoApplication()
        {
            return View();
        }

        public ActionResult Serigraphy()
        {
            return View();
        }

        public ActionResult Flex()
        {
            return View();
        }

        public ActionResult Fancywork()
        {
            return View();
        }

        public ActionResult PadPrinting()
        {
            return View();
        }

        public ActionResult UFPrinting()
        {
            return View();
        }

        public ActionResult Decal()
        {
            return View();
        }

        public ActionResult LaserEnrgaving()
        {
            return View();
        }

        public ActionResult Stamping()
        {
            return View();
        }

        public ActionResult SublimationOnMugs()
        {
            return View();
        }

        public ActionResult VolumeLabel()
        {
            return View();
        }

        public ActionResult Consult()
        {
            return View();
        }

    }
}