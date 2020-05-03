using EditPressRu.Models.DB;
using EditPressRu.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPress.Ru.Controllers
{
    public class DiscountController : Controller
    {
        //public EdPresDataContext Db = new EdPresDataContext();
        ////
        //// GET: /Discount/
        //[Authorize(Roles = "Administrator")]
        //public ActionResult Index()
        //{
        //    //DiscountContext cont = new DiscountContext();
        //    List<Discounts> list = Db.DiscountItems.OrderBy(c => c.DiscountPercent).ToList();


        //    return View(list);
        //}

        //[Authorize(Roles = "Administrator")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /Discount/Create
        //[Authorize(Roles = "Administrator")]
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here
        //       // DiscountContext cont = new DiscountContext();
        //        Discount d = new Discount();
        //        d.DiscountPercent = Decimal.Parse(collection["DiscountPercent"]);
        //        Db.DiscountItems.Add(d);
        //        Db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////
        //// GET: /Discount/Edit/5
        // [Authorize(Roles = "Administrator")]
        //public ActionResult Edit(int id)
        //{
        //    //DiscountContext cont = new DiscountContext();
        //    Discount d = Db.DiscountItems.Single(c => c.DiscountID == id);
        //    return View(d);
        //}

        ////
        //// POST: /Discount/Edit/5

        //[HttpPost]
        //[Authorize(Roles = "Administrator")]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //       // DiscountContext cont = new DiscountContext();
        //        Discount d = Db.DiscountItems.Single(c => c.DiscountID == id);
        //        d.DiscountPercent = Decimal.Parse(collection["DiscountPercent"]);
        //        Db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////
        //// GET: /Discount/Delete/5
        // [Authorize(Roles = "Administrator")]
        //public ActionResult Delete(int id)
        //{
        //    //DiscountContext cont = new DiscountContext();
        //    Discount d = Db.DiscountItems.Single(c=>c.DiscountID==id);
        //    Db.DiscountItems.Remove(d);
        //    Db.SaveChanges();

        //    return RedirectToAction("Index");
        //}
    }
}
