using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class EbazarController : Controller
    {
        private EditPressRuEntities Db = new EditPressRuEntities();
        
        int vendorId = 8;
        // GET: Adminka/Ebazar
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            EbazarAdminIndexViewModel model = new EbazarAdminIndexViewModel();

            IQueryable<VendorListStocks> Tovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    //PdfFiles = x.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                    //CdrFiles = x.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                    ProdId = x.Id,
                    ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                    Product = x
                });

            model.ListCategories = Db.Database.SqlQuery<Categories>("EXEC CategoriesForVendorsAdmin @vendorId = {0},@par='CatList'", vendorId).ToList();


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
                    cat = Db.Database.SqlQuery<int>("EXEC CategoriesForVendorsAdmin @vendorId = {0},@par='CatId'",vendorId).FirstOrDefault();
                }
                model.CatId = cat;
                Tovars = Tovars.Where(x => x.Product.ProdInCategory.FirstOrDefault().CatId == cat);

            }

            model.ProdForPaging = new PagerResult<VendorListStocks>();
            PageInfo pagInfo = new PageInfo();

            model.ProdForPaging.TotalCount = Tovars.Count();
            model.ProdForPaging.Items = Tovars.OrderBy(x => x.ProdId).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            pagInfo.PageSize = pageSize;
            pagInfo.TotalItems = model.ProdForPaging.TotalCount;
            pagInfo.page = page;
            model.PagInfo = pagInfo;

            return View(model);
        }

        public ActionResult SaveXMLfile()
        {
            var prod_href = "http://ebazaar.ru/export/products.xml";
            var stock_href = "http://ebazaar.ru/export/products-quantity.xml";

            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();
            Db.Database.ExecuteSqlCommand(String.Format("delete from ShluseForVendors where VendorId = {0}", vendorId));
            //////////////////////////////////////////
            string rezProd = GetValues(prod_href);
            if (!String.IsNullOrEmpty(rezProd))
            {
                StringReader xmlStreang = new StringReader(rezProd);
                XDocument xdoc = XDocument.Load(xmlStreang);

                foreach (XElement prodElement in xdoc.Element("document").Element("products").Elements("product"))
                {
                    ShluseForVendors prod = new ShluseForVendors();

                    prod.VendorId = vendorId;
                    prod.DateUpdate = dateUpd;
                    prod.Article = prodElement.Element("articul").Value;
                    prod.Comment = "prod";

                    try
                    {
                        prod.Price = decimal.Parse(prodElement.Element("price").Value, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        prod.Price = 0;
                    }

                    shluseList.Add(prod);
                }
            }

            string rezStock = GetValues(stock_href);
            if (!String.IsNullOrEmpty(rezStock))
            {
                StringReader xmlStreang = new StringReader(rezStock);
                XDocument xdoc = XDocument.Load(xmlStreang);

                foreach (XElement prodElement in xdoc.Element("document").Elements("product"))
                {
                    ShluseForVendors prod = new ShluseForVendors();

                    prod.VendorId = vendorId;
                    prod.DateUpdate = dateUpd;
                    prod.Article = prodElement.Element("articul").Value;
                    prod.Comment = "stock";
                    
                    try
                    {
                        prod.Reserve = int.Parse(prodElement.Element("quantity").Value) - int.Parse(prodElement.Element("quantity_free").Value);
                    }
                    catch
                    {
                        prod.Reserve = 0;
                    }

                    prod.Stock = int.Parse(prodElement.Element("quantity_free").Value);

                    shluseList.Add(prod);
                }
            }

            //////////////////////////////////////////////////
            if (shluseList.Count > 0)
            {
                Db.ShluseForVendors.AddRange(shluseList);
                Db.SaveChanges();
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetNewsTovars()
        {
            var prod_href = "http://ebazaar.ru/export/products.xml";


            string rezProd = GetValues(prod_href);
            if (!String.IsNullOrEmpty(rezProd))
            {
                StringReader xmlStreang = new StringReader(rezProd);
                XDocument xdoc = XDocument.Load(xmlStreang);
                ProductsToBase(xdoc);
            }

            Db.Database.ExecuteSqlCommand("exec AddNewProductsFromEbazar");

            List<SelectListItem> listDwnldSmall = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 8, @par = N'smal'").ToList();
            GetImages(listDwnldSmall, "smal");

            List<SelectListItem> listDwnldBig = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 8, @par = N'big'").ToList();
            GetImages(listDwnldBig, "big");

            List<SelectListItem> listDwnldMiddle = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 8, @par = N'midl'").ToList();
            GetImages(listDwnldMiddle, "midl");

            List<SelectListItem> listDwnldAttach = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 8, @par = N'advanc'").ToList();
            GetImages(listDwnldAttach, "");

            Db.Database.ExecuteSqlCommand("delete from EbazarBase_Categories");
            Db.Database.ExecuteSqlCommand("delete from EbazarBase_ImagesAdvanced");
            Db.Database.ExecuteSqlCommand("delete from EbazarBase_Products");
           

            Db.Database.ExecuteSqlCommand("update Products set StatPlaceId = 1 where VendorId = 8 and StatPlaceId = 5");

            return RedirectToAction("Index");
        }

        private void ProductsToBase(XDocument prodxml)
        {
            Db.Configuration.ValidateOnSaveEnabled = false;
            
            List<EbazarBase_Products> prodList = new List<EbazarBase_Products>();
            List<EbazarBase_ImagesAdvanced> imgList = new List<EbazarBase_ImagesAdvanced>();

            foreach (XElement prodElement in prodxml.Element("document").Element("products").Elements("product"))
            {
                EbazarBase_Products product = new EbazarBase_Products();

                product.title = prodElement.Element("title").Value;
                product.articul = prodElement.Element("articul").Value;
                try
                {
                    product.price = decimal.Parse(prodElement.Element("price").Value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    product.price = 0;
                }

                product.quantity = int.Parse(prodElement.Element("quantity").Value);
                product.quantity_free = int.Parse(prodElement.Element("quantity_free").Value);
                product.category_id = int.Parse(prodElement.Element("category_id").Value);
                product.brand = prodElement.Element("brand").Value;
                product.collection = prodElement.Element("collection").Value;
                product.description = prodElement.Element("description").Value;
                product.manufacturer = prodElement.Element("manufacturer").Value;
                product.color = prodElement.Element("color").Value;
                product.size = prodElement.Element("size").Value;
                product.small_image = prodElement.Element("small_image").Value;
                product.middle_image = prodElement.Element("middle_image").Value;
                product.big_image = prodElement.Element("big_image").Value;

                foreach (XElement imgElement in prodElement.Element("add_images").Elements("image"))
                {
                    EbazarBase_ImagesAdvanced image = new EbazarBase_ImagesAdvanced();
                    image.Image = imgElement.Value;
                    image.articul = product.articul;
                    imgList.Add(image);
                }

                prodList.Add(product);
            }

            Db.EbazarBase_Products.AddRange(prodList);
            Db.EbazarBase_ImagesAdvanced.AddRange(imgList);
            Db.SaveChanges();


            List<EbazarBase_Categories> catList = new List<EbazarBase_Categories>();

            foreach (XElement catElement in prodxml.Element("document").Element("categories").Elements("category"))
            {
                EbazarBase_Categories cat = new EbazarBase_Categories();

                cat.Id = int.Parse(catElement.Attribute("id").Value);
                cat.parentId = int.Parse(catElement.Attribute("parent_id").Value);
                cat.Title = catElement.Attribute("title").Value;

                catList.Add(cat);
            }

            Db.EbazarBase_Categories.AddRange(catList);
            Db.SaveChanges();
            //.Attribute("first").Value
        }

        private void GetImages(List<SelectListItem> forDownLoadList,string par)
        {
            string rendomFldr = Db.Products.Where(x => x.VendorId == 8 && x.StatPlaceId == 5).Max(x => x.Id).ToString();
            string dirImg = Server.MapPath(String.Format("/ImagesEbazar/{0}", rendomFldr));

            foreach (var item in forDownLoadList)
            {
                try
                {
                    string folderName = item.Value;
                    string folder = String.Format(@"{0}\{1}\", dirImg, folderName);

                   
                    string fileDownld = item.Text;
                    Thread.Sleep(250);

                    Uri fileDownldUri = new Uri(fileDownld);

                    string imgTodwnld = System.IO.Path.GetFileName(fileDownldUri.LocalPath);
                    string fileImg = "";
                    if (par!="")
                    {
                        fileImg = String.Format(@"{0}{1}", par, System.IO.Path.GetFileName(fileDownldUri.LocalPath));
                    }
                    else
                    {
                        fileImg = imgTodwnld;
                    }

                    if (fileImg!="")
                    {
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        WebClient wb = new WebClient();
                        wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                        wb.DownloadFile(fileDownld, String.Format(@"{0}\{1}", folder, fileImg));
                    }
                    


                }
                catch (Exception)
                {
                    continue;
                }

            }

        }


        static string GetValues(string href)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync(href).Result;
                HttpContent content = response.Content;

                var result = content.ReadAsByteArrayAsync();
               
                string xml = Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result);
                return xml;
            }

        }

        
        static HttpClient CreateClient()
        {
            var client = new HttpClient();
            return client;
        }

        public class EbazarAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                EbazarController controller = new EbazarController();
                controller.SaveXMLfile();
            }
        }

        public class EbazarScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<EbazarAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger8", "group8")     // идентифицируем триггер с именем и группой
                     .StartAt(DateBuilder.FutureDate(15, IntervalUnit.Minute))                         // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(12)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Ebazar стартанул на автоматическое обновление");

            }
        }

    }
}