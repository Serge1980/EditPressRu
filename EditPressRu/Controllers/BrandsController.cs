using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace EditPressRu.Controllers
{
    [OutputCache(Duration = 3600, Location = OutputCacheLocation.Downstream)]
    public class BrandsController : BaseController
    {
        EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Default
        public ActionResult Index(int page = 1)
        {
            ViewBag.metakeywords = db.GetMetaTags(0, 0, "Brands").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, 0, "Brands").MetaDesk;

            int pageSize = 25;

            BrandsViewModel model = new BrandsViewModel();

            IQueryable<Brands> rezList = Db.Brands;

            model.ProdForPaging = new PagerResult<BrandCat>();
            model.ProdForPaging.TotalCount = rezList.Count();
            model.ProdForPaging.Items = rezList.Select(x => new BrandCat { Brand = x, TovarsHref = "/products/" + Db.Categories.FirstOrDefault(c => c.Name == x.Name).CpuPath }).OrderBy(x => x.Brand.ID).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            #region GetPageIfo
            PageInfo pagInfo = new PageInfo();
            pagInfo.PageSize = pageSize;
            pagInfo.TotalItems = model.ProdForPaging.TotalCount;
            pagInfo.page = page;
            model.PageInfo = pagInfo;

            if (model.ProdForPaging.TotalCount > 1)
            {
                int pNext = page + 1;
                int pPrev = page - 1;

                if (page > 1)
                {
                    ViewBag.Canonical = "https://editpress.ru/brands";
                }
                if (page == 1)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/brands?page={0}", page);
                }
                if (page == 2 && page < model.ProdForPaging.TotalCount)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/brands?page={0}", pNext);
                    ViewBag.Prev = "https://editpress.ru/brands";
                }
                if (page > 2 && page < model.ProdForPaging.TotalCount)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/brands?page={0}", pNext);
                    ViewBag.Prev = String.Format("https://editpress.ru/brands?page={0}", pPrev);
                }
                if (page == model.ProdForPaging.TotalCount)
                {
                    if (page > 2)
                    {
                        ViewBag.Prev = String.Format("https://editpress.ru/brands?page={0}", pPrev);
                    }
                    else
                    {
                        ViewBag.Prev = "https://editpress.ru/brands";
                    }
                }

            }

            #endregion

            return View(model);
        }
    }
}