using EditPressRu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class CasesController : BaseController
    {
        // GET: Default
        public ActionResult Index(int page = 2)
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "Cases").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "Cases").MetaDesk;
            ViewBag.PageName = "Cases";

            int pageSize = 30;

            CasesViewModel model = new CasesViewModel();

            model.ListFiles = db.GetImgFiles().Skip(0)
                .Take(pageSize).ToList();
            model.TotalPages = db.GetImgFiles().Count;
            model.Page = page;
            return View(model);
        }
    }
}