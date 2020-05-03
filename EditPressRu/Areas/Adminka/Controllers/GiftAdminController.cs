using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Xml.Serialization;
using static EditPressRu.Areas.Adminka.Models.GiftStockXml;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class GiftAdminController : Controller
    {
        private EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Adminka/GiftAdmin
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;
            int vendorId = 3;

            GiftAdminIndexViewModel model = new GiftAdminIndexViewModel();

            IQueryable<VendorListStocks> Tovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    PdfFiles = x.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                    CdrFiles = x.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                    ProdId = x.Id,
                    ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                    Product = x
                });

            model.ListCategories = Db.Database.SqlQuery<Categories>("EXEC CategoriesForVendorsAdmin @vendorId = 3,@par='CatList'").ToList();


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
                    cat = Db.Database.SqlQuery<int>("EXEC CategoriesForVendorsAdmin @vendorId = 3,@par='CatId'").FirstOrDefault();
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
            var prod_href = "http://api2.gifts.ru/export/v2/catalogue/product.xml";
            var stock_href = "http://api2.gifts.ru/export/v2/catalogue/stock.xml";

            int vendorId = Db.Vendors.FirstOrDefault(x => x.Name == "Gift").Id;
            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();
            Db.Database.ExecuteSqlCommand(String.Format("delete from ShluseForVendors where VendorId = {0}", vendorId));


            string rezProd = GetValues(prod_href);
            if (!String.IsNullOrEmpty(rezProd))
            {
                StringReader xmlStreang = new StringReader(rezProd);

                XmlSerializer serializer = new XmlSerializer(typeof(Doct));
                Doct xmlContainer = (Doct)serializer.Deserialize(xmlStreang);

                foreach (var item in xmlContainer.Product)
                {
                    ShluseForVendors prod = new ShluseForVendors();
                    prod.VendorId = vendorId;
                    prod.VendorIntId = item.Product_id; //int.Parse(xn["product_id"].InnerText);
                    prod.DateUpdate = dateUpd;
                    prod.Article = item.Code; //xn["code"].InnerText;

                    prod.Rating = item.Status.Id;
                    if (item.Price.Value != null)
                    {
                        try
                        {
                            prod.Price = decimal.Parse(item.Price.Value.Replace(",", "."), CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }

                    prod.Comment = "Prod";

                    #region SubProducts


                    if (item.product != null && item.product.Count > 0)
                    {
                        List<ShluseForVendors> subProdList = new List<ShluseForVendors>();
                        foreach (var subitem in item.product)
                        {
                            ShluseForVendors subProd = new ShluseForVendors();
                            subProd.VendorId = vendorId;
                            subProd.VendorIntId = subitem.Product_id;
                            subProd.Article = subitem.Code;
                            prod.Comment = "SubProd";
                            prod.DateUpdate = dateUpd;

                            if (subitem.Price.Value != null)
                            {
                                try
                                {
                                    prod.Price = decimal.Parse(subitem.Price.Value.Replace(",", "."), CultureInfo.InvariantCulture);
                                }
                                catch (Exception)
                                {
                                }
                            }


                            subProdList.Add(subProd);
                        }

                        shluseList.AddRange(subProdList);

                    }
                    shluseList.Add(prod);
                    #endregion
                }
            }

            string rezStock = GetValues(stock_href);
            if (!String.IsNullOrEmpty(rezStock))
            {
                StringReader xmlStreamStock = new StringReader(rezStock);
                XmlSerializer serializerStock = new XmlSerializer(typeof(DoctStock));
                DoctStock xmlContainerStock = (DoctStock)serializerStock.Deserialize(xmlStreamStock);

                foreach (var item in xmlContainerStock.Stock)
                {
                    ShluseForVendors prod = new ShluseForVendors();
                    prod.VendorId = vendorId;
                    prod.VendorIntId = int.Parse(item.Product_id); //int.Parse(xn["product_id"].InnerText);
                    prod.DateUpdate = dateUpd;
                    prod.Article = item.Code;

                    if (item.Amount != null)
                    {
                        try
                        {
                            prod.Stock = int.Parse(item.Amount);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (item.Free != null)
                    {
                        try
                        {
                            prod.Reserve = int.Parse(item.Free);
                        }
                        catch
                        {
                        }
                    }

                    if (item.Inwayamount != null)
                    {
                        try
                        {
                            prod.StockTransit = int.Parse(item.Inwayamount);
                        }
                        catch
                        {
                        }
                    }

                    if (item.Inwayfree != null)
                    {
                        try
                        {
                            prod.ReserveTransit = int.Parse(item.Inwayfree);
                        }
                        catch
                        {
                        }
                    }

                    if (item.Enduserprice != null)
                    {
                        try
                        {
                            prod.Price = decimal.Parse(item.Enduserprice.Replace(",", "."), CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }

                    if (item.Dealerprice != null)
                    {
                        try
                        {
                            prod.PriceOpt = decimal.Parse(item.Dealerprice.Replace(",", "."), CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }

                    prod.Comment = "Stock";
                    shluseList.Add(prod);
                }
            }

            if (shluseList.Count > 0)
            {
                Db.ShluseForVendors.AddRange(shluseList);
                Db.SaveChanges();
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));
            }

            return RedirectToAction("Index");
        }



        #region AddToGiftNewTovar

        public ActionResult GetNewsTovars()
        {
            var prod_href = "http://api2.gifts.ru/export/v2/catalogue/product.xml";
            var cat_href = "http://api2.gifts.ru/export/v2/catalogue/treeWithoutProducts.xml";
            var catProd_href = "http://api2.gifts.ru/export/v2/catalogue/tree.xml";

            GiftClear();

            string rezProd = GetValues(prod_href);
            if (!String.IsNullOrEmpty(rezProd))
            {
                StringReader xmlStreang = new StringReader(rezProd);
                XDocument xdoc = XDocument.Load(xmlStreang);
                ProductsToBase(xdoc);
            }

            string rezCat = GetValues(cat_href);
            if (!String.IsNullOrEmpty(rezCat))
            {
                StringReader xmlStreang = new StringReader(rezCat);
                XDocument xdoc = XDocument.Load(xmlStreang);
                TreeWithoutProductsToBase(xdoc);
            }

            string rezProdCat = GetValues(catProd_href);
            if (!String.IsNullOrEmpty(rezProdCat))
            {
                StringReader xmlStreang = new StringReader(rezProdCat);
                XDocument xdoc = XDocument.Load(xmlStreang);
                CatProdToBase(xdoc);
            }

            Db.Database.ExecuteSqlCommand("exec AddNewProductsFromGift");

            List<SelectListItem> listDwnldSmall = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 3, @par = N'small'").ToList();
            GetImages(listDwnldSmall);

            List<SelectListItem> listDwnldBig = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 3, @par = N'big'").ToList();
            GetImages(listDwnldBig);

            List<SelectListItem> listDwnldAttach = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 3, @par = N'attach'").ToList();
            GetImages(listDwnldAttach);

            GiftClear();

            ////////Db.Database.ExecuteSqlCommand("update Products set StatPlaceId = 1 where VendorId = 3 and StatPlaceId = 5");

            return RedirectToAction("Index");
        }

        private void GetImages(List<SelectListItem> forDownLoadList)
        {
            string rendomFldr = Db.Products.Where(x=>x.VendorId==3 && x.StatPlaceId==5).Max(x=>x.Id).ToString();
            string dirImg = Server.MapPath(String.Format("/ImagesGift/{0}", rendomFldr));

                foreach (var item in forDownLoadList)
                {
                    try
                    {
                        string folderName = item.Value;
                        string folder = String.Format(@"{0}\{1}\", dirImg, folderName);
                       
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    string fileDownld = String.Format(@"http://api2.gifts.ru/export/v2/catalogue/{0}", item.Text);
                        Thread.Sleep(250);

                        Uri fileDownldUri = new Uri(fileDownld);

                        string fileImg = System.IO.Path.GetFileName(fileDownldUri.LocalPath);

                        WebClient wb = new WebClient();
                        wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                        wb.Credentials = new NetworkCredential("26708_xmlexport", "WordPass654");
                        wb.DownloadFile(fileDownld, String.Format(@"{0}\{1}", folder, fileImg));


                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
               
        }

        private void ProductsToBase(XDocument prodxml)
        {
            #region Products

            List<GiftBase_Products> prodList = new List<GiftBase_Products>();

            foreach (XElement prodElement in prodxml.Element("doct").Elements("product"))
            {
                GiftBase_Products prod = new GiftBase_Products();
                prod.product_id = int.Parse(prodElement.Element("product_id").Value);
                try
                {
                    prod.group = int.Parse(prodElement.Element("group").Value);
                }
                catch (Exception)
                {
                }
                prod.code = prodElement.Element("code").Value;
                try
                {
                    prod.brand = prodElement.Element("brand").Value;
                }
                catch (Exception)
                {
                }

                prod.content = prodElement.Element("content").Value;
                try
                {
                    prod.matherial = prodElement.Element("matherial").Value;
                }
                catch (Exception)
                {
                }

                prod.name = prodElement.Element("name").Value;
                try
                {
                    prod.product_size = prodElement.Element("product_size").Value;
                }
                catch (Exception)
                {
                }

                prod.status = prodElement.Element("status").Value;
                prod.small_image = prodElement.Element("small_image").Attribute("src").Value;
                prod.super_big_image = prodElement.Element("super_big_image").Attribute("src").Value;
                prod.big_image = prodElement.Element("big_image").Attribute("src").Value;

                try
                {
                    prod.weight = prodElement.Element("weight").Value;
                }
                catch (Exception)
                {
                }

                #region Price

                if (prodElement.Element("price") != null)
                {
                    try
                    {
                        prod.price_price = prodElement.Element("price").Element("price").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.price_value = prodElement.Element("price").Element("value").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.price_currency = prodElement.Element("price").Element("currency").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.price_name = prodElement.Element("price").Element("name").Value;
                    }
                    catch (Exception)
                    {
                    }

                }

                #endregion

                #region Pcks

                if (prodElement.Element("pack") != null)
                {

                    try
                    {
                        prod.sizex_pack = prodElement.Element("pack").Element("sizex").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.sizey_pack = prodElement.Element("pack").Element("sizey").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.sizez_pack = prodElement.Element("pack").Element("sizez").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.volume_pack = prodElement.Element("pack").Element("volume").Value;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        prod.weight_pack = prodElement.Element("pack").Element("weight").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        prod.amount_pack = int.Parse(prodElement.Element("pack").Element("amount").Value);
                    }
                    catch (Exception)
                    {
                    }

                }


                #endregion

                #region SubProducts

                var xnSubProdList = prodElement.Elements("product");
                if (xnSubProdList != null && xnSubProdList.Count() > 0)
                {
                    List<GiftBase_SubProducts> subProdList = new List<GiftBase_SubProducts>();
                    foreach (XElement xnProd in xnSubProdList)
                    {
                        GiftBase_SubProducts subProd = new GiftBase_SubProducts();
                        subProd.product_id = int.Parse(xnProd.Element("product_id").Value);
                        subProd.main_product = int.Parse(xnProd.Element("main_product").Value);
                        subProd.code = xnProd.Element("code").Value;
                        subProd.name = xnProd.Element("name").Value;

                        try
                        {
                            subProd.size_code = xnProd.Element("size_code").Value;
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            subProd.price_price = xnProd.Element("price").Element("price").Value;
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            subProd.price_currency = xnProd.Element("price").Element("currency").Value;
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            subProd.price_name = xnProd.Element("price").Element("name").Value;
                        }
                        catch (Exception)
                        {
                        }

                        subProdList.Add(subProd);
                    }

                    Db.GiftBase_SubProducts.AddRange(subProdList);
                    Db.SaveChanges();
                }

                #endregion

                #region Print

                var xnPrintsList = prodElement.Elements("print");
                if (xnPrintsList != null && xnPrintsList.Count() > 0)
                {
                    List<GiftBase_Prints> ListPrints = new List<GiftBase_Prints>();

                    foreach (XElement xnPrint in xnPrintsList)
                    {
                        GiftBase_Prints print = new GiftBase_Prints();

                        print.product_id = int.Parse(prodElement.Element("product_id").Value);
                        print.name = xnPrint.Element("name").Value;
                        print.description = xnPrint.Element("description").Value;

                        ListPrints.Add(print);
                    }

                    Db.GiftBase_Prints.AddRange(ListPrints);
                    Db.SaveChanges();
                }
                
                #endregion

                #region Attashments

                var xnAttshList = prodElement.Elements("product_attachment");
                if (xnAttshList != null && xnAttshList.Count() > 0)
                {
                    List<GiftBase_Attachments> attachList = new List<GiftBase_Attachments>();
                    foreach (XElement xnAttach in xnAttshList)
                    {
                        GiftBase_Attachments attach = new GiftBase_Attachments();

                        attach.product_id = int.Parse(prodElement.Element("product_id").Value);

                        try
                        {
                            attach.image = xnAttach.Element("image").Value;
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            attach.meaning = xnAttach.Element("meaning").Value == "1" ? true : false;
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            attach.name = xnAttach.Element("name").Value;
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            attach.file = xnAttach.Element("file").Value;
                        }
                        catch (Exception)
                        {
                        }

                        attachList.Add(attach);
                    }

                    Db.GiftBase_Attachments.AddRange(attachList);
                    Db.SaveChanges();
                }
                

                #endregion

                prodList.Add(prod);

            }
            Db.GiftBase_Products.AddRange(prodList);
            Db.SaveChanges();

            #endregion
        }

        private void TreeWithoutProductsToBase(XDocument catxml)
        {
            List<GiftBase_Catalog> catList = new List<GiftBase_Catalog>();

            foreach (XElement prodElement in catxml.Element("doct").Element("page").Elements("page"))
            {
                GiftBase_Catalog cat = new GiftBase_Catalog();
                try
                {
                    cat.name = prodElement.Element("name").Value;
                }
                catch (Exception)
                {
                }
                try
                {
                    cat.uri = prodElement.Element("uri").Value;
                }
                catch (Exception)
                {
                }

                try
                {
                    cat.page_id = int.Parse(prodElement.Element("page_id").Value);
                }
                catch (Exception)
                {
                }

                try
                {
                    cat.parent_page_id = int.Parse(prodElement.Attribute("parent_page_id").Value);
                }
                catch (Exception)
                {
                }

                List<GiftBase_Catalog> subcatList = new List<GiftBase_Catalog>();

                foreach (XElement subProd in prodElement.Elements("page"))
                {
                    GiftBase_Catalog subcat = new GiftBase_Catalog();
                    try
                    {
                        subcat.name = subProd.Element("name").Value;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        subcat.uri = subProd.Element("uri").Value;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        subcat.page_id = int.Parse(subProd.Element("page_id").Value);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        subcat.parent_page_id = int.Parse(subProd.Attribute("parent_page_id").Value);
                    }
                    catch (Exception)
                    {
                    }

                    subcatList.Add(subcat);

                }
                Db.GiftBase_Catalog.Add(cat);
                Db.GiftBase_Catalog.AddRange(subcatList);
                Db.SaveChanges();
            }

        }

        private void CatProdToBase(XDocument prodCat)
        {


            List<GiftBase_Prod_Cat> prod_catList = new List<GiftBase_Prod_Cat>();


            foreach (XElement prodElement in prodCat.Element("doct").Element("page").Elements("page"))
            {
                foreach (XElement itemEl in prodElement.Element("page").Elements("product"))
                {
                    GiftBase_Prod_Cat prod_cat = new GiftBase_Prod_Cat();
                    var ttt = itemEl.Element("product").Value;
                    try
                    {
                        prod_cat.cat_id = int.Parse(itemEl.Element("page").Value);
                        prod_cat.prod_id = int.Parse(itemEl.Element("product").Value);
                    }
                    catch
                    {
                    }
                   
                        prod_catList.Add(prod_cat);
                }

                Db.GiftBase_Prod_Cat.AddRange(prod_catList);
                Db.SaveChanges();
            }

            

        }


        #endregion

        private void GiftClear()
        {
            Db.Database.ExecuteSqlCommand("delete from GiftBase_Attachments");
            Db.Database.ExecuteSqlCommand("delete from GiftBase_Catalog");
            Db.Database.ExecuteSqlCommand("delete from GiftBase_Prints");
            Db.Database.ExecuteSqlCommand("delete from GiftBase_Prod_Cat");
            Db.Database.ExecuteSqlCommand("delete from GiftBase_Products");
            Db.Database.ExecuteSqlCommand("delete from GiftBase_SubProducts");
        }

        static string GetValues(string href)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync(href).Result;
                HttpContent content = response.Content;

                //var result = content.ReadAsByteArrayAsync();

                //System.IO.File.WriteAllText(path, result.Result.ToString());
                //return result.Result.ToString();
                //var stream = context.Request.InputStream;
                //byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);
                string xml = Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result);
                return xml;
            }

        }

        // создаем http-клиента с авторизацией
        static HttpClient CreateClient()
        {
            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("26708_xmlexport:WordPass654");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;

        }

        //sheduler autoStart Update from OASIS API - start to Global.asax
        public class GiftAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                GiftAdminController controller = new GiftAdminController();
                controller.SaveXMLfile();
            }
        }

        public class GiftScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<GiftAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger2", "group2")     // идентифицируем триггер с именем и группой
                    .StartNow()                           // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(2)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Gift стартанул на автоматическое обновление");

                //Несколько слов по поводу возможных настроек отправки.Вместо минутного интервала выполнения мы можем задать еще ряд других:

                //WithInterval(milliseconds): интервал выполнения в миллисекундах

                //WithIntervalInSeconds(seconds): интервал в секундах

                //WithIntervalInHours(hours): интервал в часах

                //WithRepeatCount(number): определяет количество повторов

                //Момент запуска, который задается в триггере вызовом StartNow(), также имеет различные варианты:

                //StartNow(): запуск сразу же после начала выполнения

                //StartAt(): определяет время, когда триггер начинает запускать работу

                //EndAt: определяет время, когда триггер перестает запускать работу

                //И чтобы работа начала выполняться по расписанию со стартом приложения, изменим класс Global.asax.cs:


            }
        }

    }
}