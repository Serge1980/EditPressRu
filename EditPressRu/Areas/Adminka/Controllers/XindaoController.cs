using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Controllers;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class XindaoController : Controller
    {
        EditPressRuEntities Db = new EditPressRuEntities();

        const int vendorId = 10;
        const string username = "userxd";
        const string password = "s7ep14ad";
        const string production_path = "~/Files/XinDao/production.xml";
        const string stock_path = "~/Files/XinDao/catalogue.xml";


        // GET: Adminka/Xindao
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            XinDaoIndexViewModel model = new XinDaoIndexViewModel();

            IQueryable<VendorListStocks> Tovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    //PdfFiles = x.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                    //CdrFiles = x.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                    ProdId = x.Id,
                    ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                    Product = x
                });

            model.LastDatUpdate = Tovars.Select(x => x.Product).Max(x => x.LastUpdate).ToString();

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

            return View(model);
        }

        public ActionResult GetNewXindao()
        {
            DownLoadProducts();
            AddToXindaoBaseProduct();

            Db.Database.ExecuteSqlCommand(@"Update Products
	                                        set StatPlaceId=1
	                                        where VendorId=10 and StatPlaceId=5");

            Db.Database.ExecuteSqlCommand("exec AddNewProductsFromXinDao");

            List<SelectListItem> listDwnldSmall = Db.Database.SqlQuery<SelectListItem>("EXEC [dbo].[GetImgListToDwnld] @vendorId = 10, @par = N'all'").ToList();
            GetImages(listDwnldSmall);

            XindaoClear();

            return RedirectToAction("Index");
        }

        public ActionResult GetStockXindao()
        {
            DownLoadStock();

            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();
            Db.Database.ExecuteSqlCommand(String.Format("delete from ShluseForVendors where VendorId = {0}", vendorId));

            String destProdFile = HostingEnvironment.MapPath(stock_path);
            XDocument xdoc = XDocument.Load(destProdFile);

            foreach (XElement tovItem in xdoc.Element("Каталог").Element("Товары").Elements("Товар"))
            {
                ShluseForVendors prod = new ShluseForVendors();

                prod.DateUpdate = dateUpd;
                prod.Article = tovItem.Element("Артикул").Value;
                prod.VendorId = 10;
                prod.StockRemote = GetIntValueFromString(tovItem.Element("ОстатокСкладЕвропа").Value);
                int stockFree = GetIntValueFromString(tovItem.Element("СкладМоскваСвободный").Value);
                prod.StockLocal = stockFree;
                //prod.StockLocal = int.Parse(tovItem.Element("СкладМоскваВсего").Value.Replace(" ", ""));
                prod.Reserve = GetIntValueFromString(tovItem.Element("СкладМоскваВсего").Value) - stockFree;
                prod.Stock = prod.StockRemote + prod.StockLocal;

                string Price = tovItem.Element("ЦенаРуб").Value.Replace(" ", "").Replace(" ", "");
                if (String.IsNullOrEmpty(Price))
                {
                    prod.Price = 0;
                }
                else
                {
                    prod.Price = Decimal.Parse(Price);
                }
                

                prod.Comment = "all";
                shluseList.Add(prod);
            }

            if (shluseList.Count > 0)
            {
                Db.ShluseForVendors.AddRange(shluseList);
                Db.SaveChanges();
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
                //Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));
            }

            return RedirectToAction("Index");
        }

        private void AddToXindaoBaseProduct()
        {
            //DateTime DateUpd = DateTime.Now;
            //var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            //production.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.

            XindaoClear();

            String destProdFile = HostingEnvironment.MapPath(production_path);
            XDocument xdoc = XDocument.Load(destProdFile);

            List<XinDao_Categories> xdCatLis = new List<XinDao_Categories>();

            foreach (XElement tovItem in xdoc.Element("Каталог").Element("Разделы").Elements("Раздел"))
            {
                XinDao_Categories xdCatItem = new XinDao_Categories();

                xdCatItem.Id = tovItem.Element("Ид").Value;
                xdCatItem.ParentId = tovItem.Element("ИдРодителя").Value;
                xdCatItem.Name = tovItem.Element("Наименование").Value;



                xdCatLis.Add(xdCatItem);
            }
            Db.XinDao_Categories.AddRange(xdCatLis);
            Db.SaveChanges();


            foreach (XElement tovItem in xdoc.Element("Каталог").Element("Товары").Elements("Товар"))
            {
                XinDao_Products xdProdtem = new XinDao_Products();

                xdProdtem.Article = tovItem.Element("Артикул").Value;
                xdProdtem.GroupArticle = tovItem.Element("ГруповойАртикул").Value;
                xdProdtem.Name = tovItem.Element("Наименование").Value;
                xdProdtem.Description = tovItem.Element("ОписаниеРус").Value;
                xdProdtem.Price = Decimal.Parse(tovItem.Element("Цена").Value.Replace(" ",""));
                xdProdtem.Color = tovItem.Element("Характеристики").Element("Цвет").Value;
                xdProdtem.Material = tovItem.Element("Характеристики").Element("Материал").Value;
                xdProdtem.Weight = tovItem.Element("Характеристики").Element("ВесНетто").Value;
                xdProdtem.Size = tovItem.Element("Характеристики").Element("Размер").Value;
                xdProdtem.Image = tovItem.Element("Характеристики").Element("Фотография").Value;

                xdProdtem.Pres_Size = tovItem.Element("Упаковка").Element("РазмерКоробки").Value;
                xdProdtem.Pres_Weight = tovItem.Element("Упаковка").Element("ВесКоробки").Value;
                xdProdtem.Pres_Count = tovItem.Element("Упаковка").Element("ШтукВКоробке").Value;

                xdProdtem.New = tovItem.Element("Атрибуты").Element("Новинка").Value== "Да" ? true:false;
                xdProdtem.Eko = tovItem.Element("Атрибуты").Element("Эко").Value == "Да" ? true : false;

                Db.XinDao_Products.Add(xdProdtem);
                Db.SaveChanges();

                List<XinDao_Foto> listFoto = new List<XinDao_Foto>();
                foreach (XElement fotItem in tovItem.Element("Фотографии").Elements("Фотография"))
                {
                    XinDao_Foto foto = new XinDao_Foto();

                    foto.ProdId = xdProdtem.Id;
                    foto.Name = fotItem.Value;
                    listFoto.Add(foto);
                }

                Db.XinDao_Foto.AddRange(listFoto);
                Db.SaveChanges();

                List<XinDao_ProdInCat> listProdCat = new List<XinDao_ProdInCat>();
                foreach (XElement pcItem in tovItem.Element("Разделы").Elements("Ид"))
                {
                    XinDao_ProdInCat prodCat = new XinDao_ProdInCat();

                    prodCat.ProdId = xdProdtem.Id;
                    prodCat.CatId = pcItem.Value;
                    listProdCat.Add(prodCat);
                }

                Db.XinDao_ProdInCat.AddRange(listProdCat);
                Db.SaveChanges();

                List<XinDao_Nanesenie> listNanesenie = new List<XinDao_Nanesenie>();
                foreach (XElement printItem in tovItem.Element("ТипыНанесения").Elements("ТипНанесения"))
                {
                    XinDao_Nanesenie prodCat = new XinDao_Nanesenie();

                    prodCat.ProdId = xdProdtem.Id;
                    prodCat.Name = printItem.Element("Тип").Value;
                    listNanesenie.Add(prodCat);
                }

                Db.XinDao_Nanesenie.AddRange(listNanesenie);
                Db.SaveChanges();


            }

            
            
            
        }

        private void DownLoadProducts()
        {
            //catalogue.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.

            //StockWithPrice
            string serverCatPath = "ftp://xindaorussia.ru/production.xml";

            String destCatFile = HostingEnvironment.MapPath(production_path);

            NetworkCredential credentials = new NetworkCredential(username, password);

            FtpDownLd(serverCatPath, destCatFile, credentials);
        }

        private void DownLoadStock()
        {
            //catalogue.xml Обновляется ежедневно по рабочим в период между 04.00 и 5.00 по Московскому времени.

            //StockWithPrice
            string serverCatPath = "ftp://xindaorussia.ru/catalogue.xml";

            String destCatFile = HostingEnvironment.MapPath(stock_path);

            NetworkCredential credentials = new NetworkCredential(username, password);

            FtpDownLd(serverCatPath, destCatFile, credentials);
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

        private void GetImages(List<SelectListItem> forDownLoadList)
        {
            string rendomFldr = Db.Products.Where(x => x.VendorId == 10 && x.StatPlaceId == 5).Max(x => x.Id).ToString();
            string dirImg = Server.MapPath(String.Format("/ImagesXinDao/{0}", rendomFldr));

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
                    string fileDownld =  item.Text;

                    Uri fileDownldUri = new Uri(fileDownld);

                    string fileImg = System.IO.Path.GetFileName(fileDownldUri.LocalPath);

                    WebClient wb = new WebClient();
                    wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                    //wb.Credentials = new NetworkCredential("26708_xmlexport", "WordPass654");
                    wb.DownloadFile(fileDownld, String.Format(@"{0}\{1}", folder, fileImg));

                    string path1 = String.Format(@"{0}\{1}", folder, fileImg); 
                    string path2 = String.Format(@"{0}\smal_{1}", folder, fileImg); 

                    HelpFunctions.ImageResizeSmall(path1,path2);

                }
                catch (Exception)
                {
                    continue;
                }

            }

        }

        private int GetIntValueFromString(string inpt)
        {
            int value;
            var rez= int.TryParse(string.Join("", inpt.Where(c => char.IsDigit(c))), out value);

            if (rez)
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        private void XindaoClear()
        {
            Db.Database.ExecuteSqlCommand(@"delete  FROM XinDao_Categories");
            Db.Database.ExecuteSqlCommand(@"delete  FROM XinDao_Foto");
            Db.Database.ExecuteSqlCommand(@"delete  FROM XinDao_Nanesenie");
            Db.Database.ExecuteSqlCommand(@"delete  FROM XinDao_ProdInCat");
            Db.Database.ExecuteSqlCommand(@"delete  FROM XinDao_Products");
        }

        public class XindaoAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                XindaoController controller = new XindaoController();
                controller.GetStockXindao();
            }
        }

        public class XindaoScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<XindaoAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger10", "group10")     // идентифицируем триггер с именем и группой
                     .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Minute))                         // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(12)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Xindao стартанул на автоматическое обновление");

            }
        }
    }
}