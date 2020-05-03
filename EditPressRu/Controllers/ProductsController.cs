using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EditPressRu.Repository;
using EditPressRu.Models;
using System.Web;
using System.IO;
using System.Web.UI;
using WebMatrix.WebData;

namespace EditPressRu.Controllers
{
    public class ProductsController : Controller
    {
        DataRepository db = new DataRepository();
        EditPressRuEntities Db = new EditPressRuEntities();

        const int MaxPriceGlobal = 1000000000;//hack
        
        [HttpGet]
        //[OutputCache(Duration = 1800, Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index(string cpu, int page = 1,string brand="",bool hit=false,bool sale=false,bool nev=false)
        {
            IndexViewModel model = new IndexViewModel();
            model.Cpu = cpu;

            var category = Db.Categories.FirstOrDefault(x => x.CpuPath == cpu);
            if (category == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }

            int Id = category.Id;
            //#region GetAllSales id == 303

            model.id = Id;
            model.BreadCrambs = db.GetBredCrambs(Id);

            if (page == 1)
            {
                model.Description = category.Descript;
            }
            else
            {
                model.Description = "";
            }

            ViewBag.Title = category.MetaTitle;
            ViewBag.H1 = category.Name;
            ViewBag.metakeywords = category.MetaKey;
            ViewBag.metadesc = category.MetaDesc;

            //товары со скидкой вывод зеленого меню
            if (Id == 303 || category.ParentId == 303)
            {
                if (Id == 303)
                {
                    ViewBag.H1 = "Акционные товары";
                }
                else
                {
                    ViewBag.H1 = category.Name + " со скидкой";
                    ViewBag.SaleCatMarker = "/products/" + category.CpuPath.Trim();
                }
                
                ViewBag.PageName = "Sales";
                model.CatSaleList = Db.Database.SqlQuery<SelectListItem>("EXEC GetCatSaleList").ToList();
            }
            if (Id == 243)
            {
                ViewBag.PageName = "China";
            }
            if (Id == 231)
            {
                ViewBag.PageName = "Poligraf";
            }
            if (Id == 244)
            {
                ViewBag.PageName = "Individ";
            }


            LineJsonModel incommingModel = new LineJsonModel();

            incommingModel.MaxPrice = MaxPriceGlobal;
            incommingModel.UpPriceOrder = true;
            incommingModel.page = page;
            incommingModel.id = Id;
            incommingModel.PageProductCount = 24;
            //incommingModel.MaxCatPrice = (int)category.ProdInCategory.Max(x => x.Products.Price);
            incommingModel.MaxCatPrice=GetMaxPrice(category.Id);
            incommingModel.Hit = hit;
            incommingModel.New = nev;
            incommingModel.Sale = sale;
            incommingModel.Brand = new List<string>();

            if (brand != "")
            {
                incommingModel.Brand.Add(brand);
            }

            try
            {
                model.ProdForPaging = db.GetAllProducts(incommingModel);
            }
            catch (Exception)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }

            #region MaxMinPrice

            model.MinPrice = incommingModel.MinPrice;
            model.MaxPrice = incommingModel.MaxPrice;
            model.MinCatPrice = incommingModel.MinCatPrice;
            model.MaxCatPrice = model.ProdForPaging.maxPrice;

            #endregion

            #region PageInfo

            model.PageInfo = new PageInfo();
            model.PageInfo.page = page;
            model.PageInfo.PageSize = incommingModel.PageProductCount;
            model.PageInfo.TotalItems = model.ProdForPaging.TotalCount;

            if (model.PageInfo.TotalPages > 1)
            {
                int pNext = page + 1;
                int pPrev = page - 1;

                if (page > 1)
                {
                    ViewBag.Canonical = String.Format("https://editpress.ru/products/{0}", model.Cpu);

                    ViewBag.Title = String.Format("{0}, страница {1}", ViewBag.Title, page);
                    ViewBag.H1 = String.Format("{0}, страница {1}", ViewBag.H1, page);
                    ViewBag.metadesc = String.Format("{0}. Cтраница {1}", ViewBag.metadesc, page);
                }
                if (page == 1)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/2", model.Cpu);
                }
                if (page == 2)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/3", model.Cpu);
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}", model.Cpu);
                }
                if (page > 2 && page < model.PageInfo.TotalPages)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pNext);
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pPrev);
                }
                if (page == model.PageInfo.TotalPages)
                {
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pPrev);
                }

               

            }

            #endregion

            ViewBag.Categories = db.GetCategories();
            ViewBag.Cityes = db.GetCityes();
            ViewBag.OrderId = CartId();
            string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            ViewBag.IsMobile = TestDeviceType.IsMobileBrowser(userAgent);
            model.ListCatId = Db.Categories.Where(x => x.ParentId == 0 && x.Id < 200).Select(x => x.Id).ToList();

            #region Filters on Attributes (color,material,nanesenie,brand)

            FilterModel fltrModel = db.GetFilterAtribut(model.ProdForPaging.ListProdId, null,
                null, incommingModel.Brand, null);

            model.Materials = fltrModel.Materials;
            model.Nanesenie = fltrModel.Nanesenie;
            model.Brand = fltrModel.Brands;
            model.ColorAll = fltrModel.ColorsAll;
            model.ColorSelected = fltrModel.ColorsSelected;

            #endregion

            model.UpPriceOrder = true;
            model.DownRankOrder = false;

            model.SaleHitNew = new SaleHitNewModel();
            model.SaleHitNew.Hit = hit;
            model.SaleHitNew.Sale = sale;
            model.SaleHitNew.New = nev;

            model.SaleHitNew.HitCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.Hit).Count();
            model.SaleHitNew.NewCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.New).Count();
            model.SaleHitNew.SaleCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.Sale).Count();

            return View(model);
        }

        [HttpPost]
        [OutputCache(Duration = 30, Location = OutputCacheLocation.Downstream)]
        public ActionResult Index(LineJsonModel incommingModel)
        {
            IndexViewModel model = new IndexViewModel();

            //сортировка
            if (!incommingModel.UpPriceOrder.HasValue)
            {
                incommingModel.UpPriceOrder = true;
            }
           

            if (incommingModel.page == 0)
            {
                incommingModel.page = 1;
            }

            incommingModel.PageProductCount = incommingModel.PageProductCount == 0 ? 24 : incommingModel.PageProductCount;

            if (incommingModel.MaxPrice == 0)
            {
                incommingModel.MaxPrice = MaxPriceGlobal;
            }

            if (incommingModel.id > 0)
            {
                var category = Db.Categories.FirstOrDefault(x => x.Id == incommingModel.id || x.ParentId == incommingModel.id);
                if (category == null)
                {
                    throw new HttpException(404, "HTTP/1.1 404 Not Found");
                }


                model.Cpu = category.CpuPath;
                model.id = incommingModel.id;
                model.BreadCrambs = db.GetBredCrambs(incommingModel.id);

                if (incommingModel.page == 1)
                {
                    model.Description = category.Descript;
                }
                else
                {
                    model.Description = "";
                }


                ViewBag.Title = category.MetaTitle;
                ViewBag.H1 = category.Name;
                
                //товары со скидкой вывод зеленого меню
                if (category.Id == 303 || category.ParentId == 303)
                {
                    if (category.Id == 303)
                    {
                        ViewBag.H1 = "Акционные товары";
                    }
                    else
                    {
                        ViewBag.H1 = category.Name + " со скидкой";
                        ViewBag.SaleCatMarker = "/products/"+category.CpuPath;
                    }

                    
                    ViewBag.PageName = "Sales";
                    model.CatSaleList = Db.Database.SqlQuery<SelectListItem>("EXEC GetCatSaleList").ToList();
                }
                ViewBag.metakeywords = category.MetaKey;
                ViewBag.metadesc = category.MetaDesc;
                ///sdffsdfsdfsdfdsfdsfsdfds
                try
                {
                    model.ProdForPaging = db.GetAllProducts(incommingModel);
                }
                catch (Exception)
                {
                    throw new HttpException(404, "HTTP/1.1 404 Not Found");
                }


            }
            else
            {
                model.QueryString = incommingModel.QueryString;
                incommingModel.DownRankOrder = true;
                model.Cpu = "search-result";
                ViewBag.Title = String.Format("Результаты поиска по запросу {0}", incommingModel.QueryString);
                ViewBag.H1 = String.Format("Результаты поиска по запросу {0}", incommingModel.QueryString);
                ViewBag.metakeywords = String.Format("купить оптом {0},купить неорого {0}, купить дешево {0}, где купить {0} оптом,где в Москве купить оптом {0},оптовый склад {0}, оптотовый склад в Москве {0},сувениры оптом", incommingModel.QueryString);
                ViewBag.metadesc = String.Format("Оптовый склад сувенирной продукции в Москве. Широкий выбор подарков. Вы ищите {0}? Тогда Вам сюда! ", incommingModel.QueryString);

                incommingModel.SpriceZprs = false;

                if (incommingModel.MaxPrice!=incommingModel.MaxCatPrice || incommingModel.MinCatPrice!=incommingModel.MinPrice)
                {
                    incommingModel.SpriceZprs = true;
                }

                try
                {
                    model.ProdForPaging = db.GetSearch(incommingModel);
                }
                catch (Exception)
                {
                    throw new HttpException(404, "HTTP/1.1 404 Not Found");
                }

                //если запрос был веден по английски  - то подменяем его на русский транслит
                if (!String.IsNullOrEmpty(model.ProdForPaging.QueryString))
                {
                    model.QueryString = model.ProdForPaging.QueryString;
                    ViewBag.Title = String.Format("Результаты поиска по запросу {0}", model.QueryString);
                    ViewBag.H1 = String.Format("Результаты поиска по запросу {0}", model.QueryString);
                }
            }

            #region MaxMinPrice

            model.MinPrice = incommingModel.MinPrice;
            model.MaxPrice = incommingModel.MaxPrice == MaxPriceGlobal ? model.ProdForPaging.maxPrice : incommingModel.MaxPrice;
            model.MinCatPrice = incommingModel.MinCatPrice;
            model.MaxCatPrice = model.ProdForPaging.maxPrice;

            #endregion

            #region PageInfo

            model.PageInfo = new PageInfo();
            model.PageInfo.page = incommingModel.page;
            model.PageInfo.PageSize = incommingModel.PageProductCount;
            model.PageInfo.TotalItems = model.ProdForPaging.TotalCount;

            if (model.PageInfo.TotalPages > 1)
            {
                int page = incommingModel.page;
                int pNext = page + 1;
                int pPrev = page - 1;

                if (page > 1)
                {
                    ViewBag.Canonical = String.Format("https://editpress.ru/products/{0}", model.Cpu);

                    ViewBag.Title = String.Format("{0}, страница {1}", ViewBag.Title, page); 
                    ViewBag.H1 = String.Format("{0}, страница {1}", ViewBag.H1, page);
                    ViewBag.metadesc = String.Format("{0}. Cтраница {1}", ViewBag.metadesc, page);
                }
                if (page == 1)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/2", model.Cpu);
                }
                if (page == 2)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/3", model.Cpu);
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}", model.Cpu);
                }
                if (page > 2 && page < model.PageInfo.TotalPages)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pNext);
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pPrev);
                }
                if (page == model.PageInfo.TotalPages)
                {
                    ViewBag.Prev = String.Format("https://editpress.ru/products/{0}/{1}", model.Cpu, pPrev);
                }

            }

            #endregion

            ViewBag.Categories = db.GetCategories(incommingModel.ListCatId);
            ViewBag.Cityes = db.GetCityes();
            ViewBag.OrderId = CartId();
            string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            ViewBag.IsMobile = TestDeviceType.IsMobileBrowser(userAgent);
            model.ListCatId = incommingModel.ListCatId;

            #region Filters on Attributes (color,material,nanesenie,brand)

            FilterModel fltrModel = db.GetFilterAtribut(model.ProdForPaging.ListProdId, incommingModel.Materials,
                incommingModel.Nanesenie, incommingModel.Brand, incommingModel.Color);

            model.Materials = fltrModel.Materials;
            model.Nanesenie = fltrModel.Nanesenie;
            model.Brand = fltrModel.Brands;
            model.ColorAll = fltrModel.ColorsAll;
            model.ColorSelected = fltrModel.ColorsSelected;

            #endregion

            model.UpPriceOrder = incommingModel.UpPriceOrder.Value;
            model.DownRankOrder = incommingModel.DownRankOrder.Value;

            model.SaleHitNew = new SaleHitNewModel();
            model.SaleHitNew.Hit = incommingModel.Hit;
            model.SaleHitNew.Sale = incommingModel.Sale;
            model.SaleHitNew.New = incommingModel.New;

            model.SaleHitNew.HitCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.Hit).Count();
            model.SaleHitNew.NewCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.New).Count();
            model.SaleHitNew.SaleCount = Db.Products.Where(x => model.ProdForPaging.ListProdId.Contains(x.Id) && x.Sale).Count();

           
            return View(model);
        }

        //[OutputCache(Duration = 30, Location = OutputCacheLocation.Downstream)]
        public ActionResult Product(int id)
        {
            //Для формирования хлебных крошек
            string cpu = String.Empty;
            try
            {
                cpu = Request.UrlReferrer.LocalPath.ToLower();
                if (cpu.Contains("products/product"))
                {
                    cpu = "";
                }
                else
                {
                    cpu = cpu.Replace("/", "").Replace("products", "");
                }
            }
            catch 
            {
                cpu = "";
            }

            ProductViewModel model = db.GetTovarModel(id, cpu);
            ViewBag.metakeywords = db.GetMetaTags(0, id, "").MetaKey;
            ViewBag.metadesc = db.GetMetaTags(0, id, "").MetaDesk;
            ViewBag.Categories = db.GetCategories();
            ViewBag.Cityes = db.GetCityes();
            ViewBag.OrderId = CartId();

            //Для тестирования мобильных устройств - добавлено в BaseController
            string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            ViewBag.IsMobile = TestDeviceType.IsMobileBrowser(userAgent);

            return View(model);
        }

        private int GetMaxPrice(int CatId)
        {
            int rezult = (int)Db.Database.SqlQuery<decimal>("select max(Price) from Products a inner join ProdInCategory b on a.Id=b.ProdId where b.CatId={0}", CatId).First();
            return rezult;
        }

        private int CartId()
        {
            //всегда будем иметь под рукой номер заказа с открытой корзиной --ViewBag.OrderId
            //**********************************************
            string Sid = Session.SessionID;
            int userId = 0;

            if (User.Identity.IsAuthenticated)
            {
                userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId; ;
            }

            int orderId = 0;

            try
            {
                orderId = db.GetCartOrderId(userId, Sid);
            }
            catch
            {
            }

            return orderId;
            //------------------------------------------------
        }



    }
}