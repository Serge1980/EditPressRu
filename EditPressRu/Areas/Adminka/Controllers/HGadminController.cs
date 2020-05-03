using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Controllers;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

//catalogue.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.
//-  папка «/clients/Ostatki/» - несколько файлов store(...).xml
//Обновляются каждые 15 минут в рабочие дни с 09.00 до 19.00 по Московскому времени,
//Пример выгрузки представлен ниже.


namespace EditPressRu.Areas.Adminka.Controllers
{
    public class HGadminController : BaseController
    {
        EditPressRuEntities Db = new EditPressRuEntities();

        const int vendorId = 7;
        const string username = "clients";
        const string password = "cLiENts2010";
        
        
        // GET: Adminka/HGadmin
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            HGadminIndexViewModel model = new HGadminIndexViewModel();

            IQueryable<VendorListStocks> Tovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    //PdfFiles = x.Products.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                    //CdrFiles = x.Products.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                    ProdId = x.Id,
                    ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                    Product = x
                });

            //model.
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
            StockWithPriceUpdate();

            return RedirectToAction("Index");
        }

        private void StockWithPriceUpdate()
        {
            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            //int vendorId = Db.Vendors.FirstOrDefault(x => x.Name == "HappyGift").Id;
           
            //catalogue.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.
            #region Catalogue
            DownLoadCatalogue();
            List<ShluseForVendors> containerList = new List<ShluseForVendors>();
            String destProdFile = HostingEnvironment.MapPath("~/Files/HGfiles/catalogue.xml");
            XDocument xdoc = XDocument.Load(destProdFile);

            DateTime dt = DateTime.Now;

            foreach (XElement tovItem in xdoc.Element("Каталог").Element("Товары").Elements("Товар"))
            {
                ShluseForVendors vendorItem = new ShluseForVendors();

                vendorItem.VendorId = vendorId;
                vendorItem.VendorStrId = tovItem.Element("ИД").Value;
                vendorItem.Article = tovItem.Element("Артикул").Value;
                vendorItem.DateUpdate = dateUpd;
                vendorItem.Comment = "Cat";

                if (tovItem.Element("ЦенаРуб").Value!=null)
                {
                    vendorItem.Price = decimal.Parse(tovItem.Element("ЦенаРуб").Value.Replace(",", "."), CultureInfo.InvariantCulture);
                }
               
                try
                {
                    vendorItem.Stock = int.Parse(tovItem.Element("Свободный").Value);
                }
                catch
                {
                    vendorItem.Stock = 0;
                }
                try
                {
                    vendorItem.Reserve = int.Parse(tovItem.Element("Занятый").Value);
                }
                catch
                {

                    vendorItem.Reserve = 0;
                }


                XElement catElement = tovItem.Element("Поставка");

                if (catElement != null)
                {
                    try
                    {
                        vendorItem.ReserveTransit = int.Parse(catElement.Element("ЗанятыйВПути").Value);
                    }
                    catch
                    {

                        vendorItem.ReserveTransit = 0;
                    }

                    try
                    {
                        vendorItem.StockTransit = int.Parse(catElement.Element("СвободныйВПути").Value);
                    }
                    catch
                    {

                        vendorItem.StockTransit = 0;
                    }
                }

                containerList.Add(vendorItem);

            }

           
            #endregion

            //stock(...).xml Обновляется каждые 15 минут.
            DownLoadStocks();

            //List<VendorsStock> containerList2 = new List<VendorsStock>();

            String destStockFile = HostingEnvironment.MapPath("~/Files/HGfiles/Stocks");
            System.IO.DirectoryInfo di = new DirectoryInfo(destStockFile);

            foreach (FileInfo file in di.GetFiles())
            {
                XDocument xdocStock = XDocument.Load(file.FullName);
                string rootElement = String.Format("ВыгрузкаОстатковНаСайт{0}", file.Name.Replace("store", "").Replace(".xml", ""));
                foreach (XElement tovItem in xdocStock.Element(rootElement).Element("Остатки").Elements("Остаток"))
                {
                    ShluseForVendors stock = new ShluseForVendors();

                    stock.VendorStrId = tovItem.Element("ИД").Value;
                    stock.VendorId = vendorId;
                    stock.DateUpdate = dateUpd;
                    stock.Comment = "Stock";
                    try
                    {
                        stock.Stock = int.Parse(tovItem.Element("Свободный").Value);
                    }
                    catch
                    {
                        stock.Stock = 0;
                    }
                    try
                    {
                        stock.Reserve = int.Parse(tovItem.Element("Занятый").Value);
                    }
                    catch
                    {
                        stock.Reserve = 0;
                    }

                    XElement catElement = tovItem.Element("Поставка");

                    if (catElement != null)
                    {
                        try
                        {
                            stock.ReserveTransit = int.Parse(catElement.Element("ЗанятыйВПути").Value);
                        }
                        catch
                        {

                            stock.ReserveTransit = 0;
                        }

                        try
                        {
                            stock.StockTransit = int.Parse(catElement.Element("СвободныйВПути").Value);
                        }
                        catch
                        {

                            stock.StockTransit = 0;
                        }
                    }

                    containerList.Add(stock);

                }
            }

            Db.ShluseForVendors.AddRange(containerList);
            Db.SaveChanges();
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));
        }


        private void DownLoadCatalogue()
        {
            //catalogue.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.

            //StockWithPrice
            string serverCatPath = "ftp://ftp.ipg.su/clients/Nomenklatura/catalogue.xml ";

            String destCatFile = HostingEnvironment.MapPath("~/Files/HGfiles/catalogue.xml");
            //string destCatFile = @"d:\Work\SITESITE\2018\ЭдитПресс\Console\HappyGifts\HappyGiftDownLoad\Store\catalogue.xml";

            NetworkCredential credentials = new NetworkCredential(username, password);

            FtpDownLd(serverCatPath, destCatFile, credentials);
        }

        private void DownLoadStocks()
        {
            //Обновляются каждые 15 минут в рабочие дни с 09.00 до 19.00 по Московскому времени,

            DeleteAllStockFiles();
            //MoscowStocks
            string serverStockPath = "ftp://ftp.ipg.su/clients/Ostatki/";

            String destStockFile = HostingEnvironment.MapPath("~/Files/HGfiles/Stocks");
            //string destStockFile = @"d:\Work\SITESITE\2018\ЭдитПресс\Console\HappyGifts\HappyGiftDownLoad\Store";

            //////////////RemoteStocks
            ////////////string serverStockRemPath = "ftp://ftp.ipg.su/clients/Ostatkifilials/";

            NetworkCredential credentials = new NetworkCredential(username, password);

            DownloadFtpDirectory(serverStockPath, destStockFile, credentials);
            ////////////DownloadFtpDirectory(serverStockRemPath, destStockFile, credentials);
        }

        private static void DownloadFtpDirectory(string url, string localPath, NetworkCredential credentials)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            List<string> lines = new List<string>();

            try
            {
                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lines.Add(listReader.ReadLine());
                    }
                }

                foreach (string line in lines)
                {
                    string[] tokens =
                        line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                    string name = tokens[8];
                    string permissions = tokens[0];

                    string localFilePath = Path.Combine(localPath, name);
                    string fileUrl = url + name;

                    FtpDownLd(fileUrl, localFilePath, credentials);


                }
            }
            catch
            {

                return;
            }

        }

        private static void FtpDownLd(string ftpPath, string destPath, NetworkCredential credentials)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);

            request.KeepAlive = true;
            request.UsePassive = true;
            request.UseBinary = true;

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = credentials;

            // Read the file from the server & write to destination                
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) // Error here
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            using (StreamWriter destination = new StreamWriter(destPath))
            {
                destination.Write(reader.ReadToEnd());
                destination.Flush();
            }
        }

        static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void DeleteAllStockFiles()
        {
            String destStockFile = HostingEnvironment.MapPath("~/Files/HGfiles/Stocks");
            System.IO.DirectoryInfo di = new DirectoryInfo(destStockFile);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

        }

        //sheduler autoStart Update from OASIS API - start to Global.asax
        public class GiftAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                HGadminController controller = new HGadminController();
                //controller.SaveXMLfile();
            }
        }

        public class HGiftScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<GiftAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger7", "group7")     // идентифицируем триггер с именем и группой
                    .StartNow()                           // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInMinutes(45)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("HG стартанул на автоматическое обновление");

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