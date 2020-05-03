using EditPressRu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class ErrorPageController : BaseController
    {
        //public ActionResult Error(HandleErrorInfo error)
        //{
        //    //Response.StatusCode = id;
        //    Response.StatusCode = 404;

        //    return View(error);
        //}

        public ActionResult Error(int id=404)
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}