using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EditPressRu.Models.DB;
using EditPressRu.Controllers;
using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Models;
using EditPressRu.Helpers;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class ProductsAdminController : BaseController
    {
        private EditPressRuEntities Db = new EditPressRuEntities();

       // GET: Adminka/Products
      
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            VendorAdminIndexViewModel model = new VendorAdminIndexViewModel();

            using (EditPressRuEntities Db = new EditPressRuEntities())
            {
                IQueryable<VendorListStocks> Tovars = Db.Products.
                    Select(x => new VendorListStocks
                    {
                        //PdfFiles = x.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                        //CdrFiles = x.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                        ProdId = x.Id,
                        ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                        Product = x
                    });

                model.LastDatUpdate = Tovars.Select(x => x.Product).Max(x => x.LastUpdate).ToString();

                model.ListCategories = Db.Database.SqlQuery<Categories>("EXEC CategoriesForVendorsAdmin @vendorId = {0},@par='CatList'", 0).ToList();


                if (!String.IsNullOrEmpty(Artikle) || !String.IsNullOrEmpty(Name))
                {
                    model.Artikle = Artikle;
                    model.Name = Name;

                    if (!String.IsNullOrEmpty(Artikle))
                    {
                        Tovars = Tovars.Where(x => x.Product.Article.ToLower().Contains(Artikle.ToLower()));
                    }
                    if (!String.IsNullOrEmpty(Name))
                    {
                        Tovars = Tovars.Where(x => x.Product.Name.ToLower().Contains(Name.ToLower()));
                    }

                    if (Tovars.Count() == 0)
                    {
                        throw new HttpException(404, "HTTP/1.1 404 Not Found");
                    }

                    cat = Tovars.FirstOrDefault().Product.ProdInCategory.FirstOrDefault().CatId;
                }
                else
                {
                    if (cat == 0)
                    {
                        cat = Db.Database.SqlQuery<int>("EXEC CategoriesForVendorsAdmin @vendorId = {0},@par='CatId'", 0).FirstOrDefault();
                    }
                    model.CatId = cat;
                    List<int> listProdId = Db.ProdInCategory.Where(x => x.CatId == cat).Select(x => x.ProdId).ToList();

                    Tovars = Tovars.Where(x => listProdId.Contains(x.ProdId));
                    // ТУТ косячит.--надо кадо как то пределать
                    //Tovars = Tovars.Where(x => x.Product.ProdInCategory.Any(z=>z.CatId==cat));

                }

                model.ProdForPaging = new PagerResult<VendorListStocks>();
                PageInfo pagInfo = new PageInfo();

                model.ProdForPaging.TotalCount = Tovars.Count();
                model.ProdForPaging.Items = Tovars.OrderBy(x => x.ProdId).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                pagInfo.PageSize = pageSize;
                pagInfo.TotalItems = model.ProdForPaging.TotalCount;
                pagInfo.page = page;
                model.PagInfo = pagInfo;
            }
            return View(model);
        }
        // GET: Adminka/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = Db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Adminka/Products/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Id = new SelectList(Db.Attributes, "ProdId", "Size");
        //    return View();
        //}

        // POST: Adminka/Products/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VendorId_int,VendorId_str,Publish,Name,ShName,Descript,MetaDescript,MetaKeyWords,Title,Article,OldArticle,NewItem,Hit,China,DusheUgodno,Rank,Sort,Price,Nalichie,TopSales")] Products products)
        {
            if (ModelState.IsValid)
            {
                Db.Products.Add(products);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.Id = new SelectList(Db.Attributes, "ProdId", "Size", products.Id);
            return View(products);
        }

        // GET: Adminka/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            Products products = Db.Products
                .Include(x=>x.ProdImages)
                .Include(x=>x.ProdInCategory)
                .Include(x=>x.Prods_Colors)
                .Include(x=>x.Prods_Materials)
                .Include(x=>x.Prods_Nanesenie)
                .SingleOrDefault(x=>x.Id==id);
            return View(products);
        }

        // POST: Adminka/Products/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VendorId_int,VendorId_str,Publish,Name,ShName,Descript,MetaDescript,MetaKeyWords,Title,Article,OldArticle,NewItem,Hit,China,DusheUgodno,Rank,Sort,Price,Nalichie,TopSales")] Products products)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(products).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Id = new SelectList(Db.Attributes, "ProdId", "Size", products.Id);
            return View(products);
        }

        // GET: Adminka/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = Db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Adminka/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = Db.Products.Find(id);
            Db.Products.Remove(products);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
