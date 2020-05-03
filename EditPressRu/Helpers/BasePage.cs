using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Helpers
{
    public class BasePage<T> : WebViewPage<T>
    {
        public string Msg { get { return (string)ViewBag.Msg; } }
        public List<Products> LstProd { get { return (List<Products>)ViewBag.LstProd; } }
        public override void Execute()
        {

        }
    }
}