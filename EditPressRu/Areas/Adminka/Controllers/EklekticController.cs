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
    public class EklekticController : Controller
    {
        private EditPressRuEntities Db = new EditPressRuEntities();

        int vendorId = 9;
        // GET: Adminka/Eklektic
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            EklekticAdminIndexViewModel model = new EklekticAdminIndexViewModel();

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
                    cat = Db.Database.SqlQuery<int>("EXEC CategoriesForVendorsAdmin @vendorId = {0},@par='CatId'", vendorId).FirstOrDefault();
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
            var prod_href = "https://eklektika.ru/xml/import.xml";

            //XDocument xdoc = XDocument.Load(prod_href);
            //var prod_href = "http://ebazaar.ru/export/products.xml";
            //var stock_href = "http://ebazaar.ru/export/products-quantity.xml";

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
                    prod.Comment = "all";

                    prod.Stock = int.Parse(prodElement.Element("Остаток").Value);
                    prod.StockRemote = int.Parse(prodElement.Element("ОстатокПоставщика").Value);
                    prod.ReserveTransit = int.Parse(prodElement.Element("ОплаченныйРезерв").Value);
                    prod.ReserveRemote = int.Parse(prodElement.Element("НеОплаченныйРезерв").Value);
                    try
                    {
                        prod.Price = decimal.Parse(prodElement.Element("ОсновнаяЦена").Value, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        prod.Price = 0;
                    }

                    try
                    {
                        prod.PriceOpt = decimal.Parse(prodElement.Element("РекламнаяЦена").Value, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        prod.PriceOpt = 0;
                    }

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
            var prod_href = "https://eklektika.ru/xml/import.xml";

            XDocument xdoc = XDocument.Load(prod_href);

            Db.Database.ExecuteSqlCommand("delete from EklekticBase_Categories");
            Db.Database.ExecuteSqlCommand("delete from EklekticBase_ProdCat");
            Db.Database.ExecuteSqlCommand("delete from EklekticBase_Products");

            ProductsToBase(xdoc);

            Db.Database.ExecuteSqlCommand("exec AddNewProductsFromEklektic");

            GetImages();

            Db.Database.ExecuteSqlCommand("delete from EklekticBase_Categories");
            Db.Database.ExecuteSqlCommand("delete from EklekticBase_ProdCat");
            Db.Database.ExecuteSqlCommand("delete from EklekticBase_Products");

            Db.Database.ExecuteSqlCommand("update Products set StatPlaceId = 1 where VendorId = 9 and StatPlaceId = 5");

            return RedirectToAction("Index");
        }

        private void ProductsToBase(XDocument prodxml)
        {
            Db.Configuration.ValidateOnSaveEnabled = false;

            List<EklekticBase_Products> prodList = new List<EklekticBase_Products>();
            List<EklekticBase_ProdCat> prodCatList = new List<EklekticBase_ProdCat>();

            foreach (XElement prodElement in prodxml.Element("КоммерческаяИнформация").Element("Каталог").Element("Товары").Elements("Товар"))
            {
                EklekticBase_Products product = new EklekticBase_Products();

                product.Artikul = prodElement.Element("Артикул").Value;
                product.Id = new Guid(prodElement.Element("Ид").Value);
                product.Name = prodElement.Element("Наименование").Value;
                product.Stock = int.Parse(prodElement.Element("Остаток").Value);
                product.Stock_Remote = int.Parse(prodElement.Element("ОстатокПоставщика").Value);
                product.Reserv_transit = int.Parse(prodElement.Element("ОплаченныйРезерв").Value);
                product.Reserv_Remote = int.Parse(prodElement.Element("НеОплаченныйРезерв").Value);
                product.TovarEklektic = int.Parse(prodElement.Element("ТоварЭклектика").Value);
                product.AnalogKode = new Guid(prodElement.Element("ГруппаАналогов").Value);
                product.ColorGroupCode = new Guid(prodElement.Element("ГруппаАналоговЦветов").Value);
                product.Color = prodElement.Element("Цвет").Value;
                product.ColorCode = prodElement.Element("СтрокаЦветов").Value;
                product.BazEdCod = int.Parse(prodElement.Element("БазоваяЕдиница").Attribute("Код").Value);
                product.BazEdName = prodElement.Element("БазоваяЕдиница").Attribute("НаименованиеПолное").Value;
                try
                {
                    product.Price = decimal.Parse(prodElement.Element("ОсновнаяЦена").Value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    product.Price = 0;
                }

                try
                {
                    product.Price_opt = decimal.Parse(prodElement.Element("РекламнаяЦена").Value, CultureInfo.InvariantCulture);
                }
                catch
                {
                    product.Price_opt = 0;
                }


                foreach (XElement atrElement in prodElement.Element("ЗначенияРеквизитов").Elements("ЗначениеРеквизита"))
                {
                    if (atrElement.Element("Наименование").Value == "Полное наименование")
                    {
                        product.FullName = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "Материал")
                    {
                        product.Material = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "МетодНанесения")
                    {
                        product.Nanesenie = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "Упаковка")
                    {
                        product.Preservativ = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "Описание")
                    {
                        product.Descript = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "Размеры")
                    {
                        product.Size = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "Вес")
                    {
                        product.Weight = atrElement.Element("Значение").Value;
                    }

                    if (atrElement.Element("Наименование").Value == "СнятСПроизводства")
                    {
                        product.PrPr = atrElement.Element("Значение").Value == "true" ? true : false;
                    }

                    if (atrElement.Element("Наименование").Value == "Отрасль №1")
                    {
                        product.Otrasl = atrElement.Element("Значение").Value;
                    }

                }

                foreach (XElement catElement in prodElement.Element("Группы").Elements("Ид"))
                {
                    EklekticBase_ProdCat prodCat = new EklekticBase_ProdCat();
                    prodCat.ProdId = new Guid(prodElement.Element("Ид").Value);
                    prodCat.CatId = new Guid(catElement.Value);
                    prodCatList.Add(prodCat);
                }

                prodList.Add(product);
            }

            Db.EklekticBase_Products.AddRange(prodList);
            Db.EklekticBase_ProdCat.AddRange(prodCatList);
            Db.SaveChanges();


            List<EklekticBase_Categories> catList = new List<EklekticBase_Categories>();

            foreach (XElement catElement in prodxml.Element("КоммерческаяИнформация").Element("Классификатор").Element("Группы").Elements("Группа"))
            {
                EklekticBase_Categories cat = new EklekticBase_Categories();

                cat.Id = new Guid(catElement.Element("Ид").Value);
                cat.Name = catElement.Element("Наименование").Value;

                catList.Add(cat);
            }

            Db.EklekticBase_Categories.AddRange(catList);
            Db.SaveChanges();
            //.Attribute("first").Value
        }

        private void GetImages()
        {
            List<SelectListItem> forDownLoadList = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 9, @par = N'main'").ToList();
            string rendomFldr = Db.Products.Where(x => x.VendorId == 9 && x.StatPlaceId == 5).Max(x => x.Id).ToString();
            string dirImg = Server.MapPath(String.Format("/ImagesEklektic/{0}", rendomFldr));

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

                    string fileImg = imgTodwnld;

                    if (fileImg != "")
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

        public class EklekticAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                EklekticController controller = new EklekticController();
                controller.SaveXMLfile();
            }
        }

        public class EklekticScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<EklekticAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger9", "group9")     // идентифицируем триггер с именем и группой
                     .StartAt(DateBuilder.FutureDate(45, IntervalUnit.Minute))                         // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(2)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Eklektic стартанул на автоматическое обновление");

            }
        }

    }
}