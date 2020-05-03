using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Controllers;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Quartz.Impl;
using System.Globalization;
using System.Web.Hosting;
using System.IO;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class OasisAdminController : BaseController
    {
        EditPressRuEntities Db = new EditPressRuEntities();
        

        const string Href_stock_api = "https://api.oasiscatalog.com/v4/stock?format=json&fields=id,article,stock,stock-remote,stock-local,reserve,reserve-remote,reserve-transit,reserve-local,price,price-discount"; //get stocks
        const string Href_tovar_api = @"https://api.oasiscatalog.com/v4/products?format=json&fields=id,article,name,full_name,price,brand_id,discount_group_id,rating,discount_price,old_price";

        private const string Href_new_tovar_api = "https://api.oasiscatalog.com/v4/products?format=json&fieldset=full"; //get tovars
                                                                                                                        //private const string Href_category_api = "https://api.oasiscatalog.com/v4/categories?format=json"; //get categories


        const string token = "key8290503dde8549849ff02c26f711fc45";

        private const string fullJason = "~/Files/Oasis/tovars.json";

        [HttpGet]
        public ActionResult Index(int cat = 0, int page = 1, string Artikle = "", string Name = "", string priceCat = "")
        {
            int pageSize = 30;
            bool search = false;
            int vendorId = 1;

            IQueryable<VendorListStocks> oasisTovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    PdfFiles = x.Makety.Where(c => c.Ext == "pdf").Select(c => c.FilePath).ToList(),
                    CdrFiles = x.Makety.Where(c => c.Ext == "cdr").Select(c => c.FilePath).ToList(),
                    ProdId = x.Id,
                    ImgPth = x.ProdImages.FirstOrDefault(c => c.Main).Small,
                    Product = x
                });

            IQueryable<ProdInCategory> allCatOasis = Db.ProdInCategory.Where(x => x.Products.VendorId == vendorId);

            List<int> CatList = allCatOasis.Select(x => x.CatId).Distinct().ToList();

            List<CatWithSubcat> catList = new List<CatWithSubcat>();
            IQueryable<Categories> catQuery = Db.Categories.Where(x => CatList.Contains(x.Id));
            foreach (var item in catQuery.Where(x => x.ParentId == 0 && x.Id < 200))
            {
                CatWithSubcat itemCatSubCat = new CatWithSubcat();
                itemCatSubCat.Category = item;
                catList.Add(itemCatSubCat);
            }

            foreach (var item in catList)
            {
                item.ListSubcat = new List<Categories>();
                foreach (var subitem in catQuery.Where(x => x.ParentId == item.Category.Id))
                {
                    try
                    {
                        item.ListSubcat.Add(subitem);
                    }
                    catch
                    {
                    }

                }
            }

            //Получаем cat чтоб отсечь для вывода ибо все тащить сразу -много
            if (cat == 0)
            {
                cat = allCatOasis.FirstOrDefault(x => CatList.Contains(x.CatId)).CatId;
            }

            OasisAdminIndexViewModel model = new OasisAdminIndexViewModel();

            model.CatId = cat;

            ////////var result = players.Join(teams, // второй набор
            //////// p => p.Team, // свойство-селектор объекта из первого набора
            //////// t => t.Name, // свойство-селектор объекта из второго набора
            //////// (p, t) => new { Name = p.Name, Team = p.Team, Country = t.Country }); // результат

            oasisTovars = oasisTovars.Where(x => x.Product.ProdInCategory.Any(t => t.CatId == cat));

            if (!String.IsNullOrEmpty(Artikle) || !String.IsNullOrEmpty(Name))
            {
                search = true;
                model.Artikle = Artikle;
                model.Name = Name;

                if (!String.IsNullOrEmpty(Artikle))
                {
                    oasisTovars = oasisTovars.Where(x => x.Product.Article.ToLower().Contains(Artikle.ToLower()));
                }
                if (!String.IsNullOrEmpty(Name))
                {
                    oasisTovars = oasisTovars.Where(x => x.Product.Name.ToLower().Contains(Name.ToLower()));
                }

                if (oasisTovars.Count() == 0)
                {
                    throw new HttpException(404, "HTTP/1.1 404 Not Found");
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(priceCat))
                {
                    //oasisTovars = oasisTovars.Where(x =>x.Stock.Discount_group_id== priceCat);
                }
            }

            if (search)
            {
                model.Title = "Товар 030 - Результaт поиска";
            }
            else
            {
                model.Title = String.Format("Товар 030 - {0}", Db.Categories.SingleOrDefault(x => x.Id == cat).Name);
            }

            catList = catList.Where(x => x.ListSubcat != null && x.ListSubcat.Count > 0).ToList();

            model.CatList = catList;

            model.ProdForPaging = new PagerResult<VendorListStocks>();
            PageInfo pagInfo = new PageInfo();

            model.ProdForPaging.TotalCount = oasisTovars.Count();
            model.ProdForPaging.Items = oasisTovars.OrderBy(x => x.ProdId).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            pagInfo.PageSize = pageSize;
            pagInfo.TotalItems = model.ProdForPaging.TotalCount;
            pagInfo.page = page;
            model.PagInfo = pagInfo;

            return View(model);
        }

        //Нужен для получния по id товара из Product.cshtml
        public ActionResult GetStokForProd(string id)
        {
            string Href_stock_api_id = String.Format("https://api.oasiscatalog.com/v4/stock?format=json&ids={0}&fields=id,article,stock,stock-remote,stock-local,reserve,reserve-remote,reserve-transit,reserve-local,price,price-discount", id); //get stocks
            string Href_tovar_api_id = String.Format(@"https://api.oasiscatalog.com/v4/products?format=json&ids={0}&fields=id,article,name,full_name,price,brand_id,discount_group_id,rating,discount_price,old_price", id);

            List<OasisStokJson> stockListJson = GetValues(token, Href_stock_api_id);

            Products stock = Db.Products.FirstOrDefault(x => x.VendorId_str == id && x.VendorId == 1);

            foreach (var item in stockListJson)
            {
                stock.Reserve = item.reserve;
                try
                {
                    stock.Price = decimal.Parse(item.price.Replace(",", "."));
                }
                catch
                {
                }

                stock.Stock = item.stock;
                stock.Stock_remote = item.stock_remote;

                Db.SaveChanges();
            }

            List<OasisStokJson> tovarListJson = GetValues(token, Href_tovar_api_id);

            foreach (var item in tovarListJson)
            {
                try
                {
                    stock.Price_opt = decimal.Parse(item.discount_price.Replace(",", "."));
                }
                catch
                {
                }

                try
                {
                    stock.Price_old = decimal.Parse(item.old_price.Replace(",", "."));
                }
                catch
                {
                }

                stock.Rating = item.rating;
                stock.Discount_group_id = item.discount_group_id;
                stock.LastUpdate = DateTime.Now;

                Db.SaveChanges();
            }

            Db.Database.ExecuteSqlCommand("exec UpdateSaleSizeOasisStock");

            return PartialView("_PartialStockProduct", stock);

        }

        //Получение остатков
        public ActionResult UpdateOasisStock()
        {
            ///////////Переделать все сучетом Oasis3Upgrade

            UpdOasisStockFromAPI();

            return Redirect("/Adminka/OasisAdmin/Index");

        }

        // Получение новых товаров
        public ActionResult GetNewTovar()
        {
            string fileTovarJson = HostingEnvironment.MapPath(fullJason);

            using (FileStream output = new FileStream(fileTovarJson, FileMode.Create))
            {
                GetValuesTJ(token, Href_new_tovar_api).CopyTo(output);
            }

            List<OasisTovarJson> tovarListJson = JsonConvert.DeserializeObject<List<OasisTovarJson>>(System.IO.File.ReadAllText(fileTovarJson));

            CreateTovars(tovarListJson);


            return Redirect("/Adminka/OasisAdmin/Index");
        }

        private void CreateTovars(List<OasisTovarJson> tovListJson)
        {
            Db.Configuration.ValidateOnSaveEnabled = false;

            int cntAll = tovListJson.Count;
            int counter = 0;
            List<Oasis_Products> listOasisProds = new List<Oasis_Products>();

            Db.Database.ExecuteSqlCommand("delete from Oasis_Categories");
            Db.Database.ExecuteSqlCommand("delete from Oasis_Materials");
            Db.Database.ExecuteSqlCommand("delete from Oasis_Packages");
            Db.Database.ExecuteSqlCommand("delete from Oasis_ProdColors");
            Db.Database.ExecuteSqlCommand("delete from Oasis_ProdImages");
            Db.Database.ExecuteSqlCommand("delete from Oasis_ProdInCategory");
            Db.Database.ExecuteSqlCommand("delete from Oasis_ProdInCategoryFull");
            Db.Database.ExecuteSqlCommand("delete from Oasis_Products");

            foreach (var item in tovListJson)
            {
                counter++;
                try
                {

                    Oasis_Products tovar = new Oasis_Products();
                    tovar.article = item.article;
                    tovar.id = item.id;
                    tovar.brand = item.brand;
                    tovar.description = item.description;
                    tovar.full_name = item.full_name;
                    tovar.id = item.id;
                    tovar.old_price = item.old_price;
                    tovar.price = item.price;
                    tovar.name = item.name;
                    tovar.group_id = item.group_id;
                    tovar.is_export_allowed = tovar.is_export_allowed;
                    tovar.is_high = item.is_high;
                    tovar.is_on_order = item.is_on_order;
                    //tovar.is_stopped = item.is_stopped;
                    tovar.is_vip = item.is_vip;
                    tovar.is_virtual = item.is_virtual;
                    tovar.is_virtual = item.is_visible;
                    tovar.kit_id = item.kit_id;
                    tovar.main_category = item.main_category;
                    tovar.name = item.name;
                    tovar.old_price = item.old_price;
                    tovar.parent_color_id = item.parent_color_id;
                    tovar.parent_gender_id = item.parent_gender_id;
                    tovar.parent_id = item.parent_id;
                    tovar.parent_size_id = item.parent_size_id;
                    tovar.parent_volume_id = item.parent_volume_id;
                    tovar.price = item.price;
                    tovar.provider_type_id = item.provider_type_id;
                    tovar.rating = item.rating;

                    tovar.size_sort = item.size_sort;
                    tovar.type_id = item.type_id;
                    tovar.video_id = item.video_id;
                    tovar.article_base = item.article_base;

                    #region Attributes
                    try
                    {
                        tovar.weight = item.attributes.FirstOrDefault(x => x.name == "Вес").value;
                    }
                    catch
                    {
                    }

                    try
                    {
                        tovar.complectnost = item.attributes.FirstOrDefault(x => x.name == "Комплектность").value;
                    }
                    catch
                    {
                    }

                    tovar.size = item.size;
                    if (String.IsNullOrEmpty(tovar.size))
                    {
                        try
                        {
                            tovar.size = item.attributes.FirstOrDefault(x => x.name == "Размер товара").value;
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        tovar.volume = item.attributes.FirstOrDefault(x => x.name == "Объем").value;
                    }
                    catch
                    {
                    }

                    try
                    {

                        if (item.attributes.FirstOrDefault(x => x.name == "Особенности товара").value == "Хит продаж")
                        {
                            tovar.hit = true;
                        }
                    }
                    catch
                    {
                    }

                    listOasisProds.Add(tovar);

                }
                catch { }
                #endregion

                #region Materials and ProdInCat

                if (item.materials!=null && item.materials.Count>0)
                {
                    List<Oasis_Materials> materialsList = new List<Oasis_Materials>();

                    foreach (var material in item.materials)
                    {
                        Oasis_Materials materItem = new Oasis_Materials();

                        materItem.id_string = item.id;
                        materItem.Material = material;
                        materialsList.Add(materItem);
                    }

                    Db.Oasis_Materials.AddRange(materialsList);
                    Db.SaveChanges();
                }

                if (item.categories!=null && item.categories.Count>0)
                {
                    List<Oasis_ProdInCategory> listProdCat = new List<Oasis_ProdInCategory>();

                    foreach (var cat in item.categories)
                    {
                        Oasis_ProdInCategory prodCatItem = new Oasis_ProdInCategory();
                        prodCatItem.OasisCAtId = cat;
                        prodCatItem.OasisId = item.id;

                        listProdCat.Add(prodCatItem);
                    }

                    Db.Oasis_ProdInCategory.AddRange(listProdCat);
                    Db.SaveChanges();
                }


                if (item.full_categories!=null && item.full_categories.Count>0)
                {
                    List<Oasis_ProdInCategoryFull> listProdCatFull = new List<Oasis_ProdInCategoryFull>();

                    foreach (var cat in item.full_categories)
                    {
                        Oasis_ProdInCategoryFull prodCatItem = new Oasis_ProdInCategoryFull();
                        prodCatItem.OasisFCatId = cat;
                        prodCatItem.OasisId = item.id;

                        listProdCatFull.Add(prodCatItem);
                    }

                    Db.Oasis_ProdInCategoryFull.AddRange(listProdCatFull);
                    Db.SaveChanges();
                }
               

                #endregion

                #region Packs

                if (item.package!=null && item.package.Count>0)
                {
                    List<Oasis_Packages> listPacks = new List<Oasis_Packages>();

                    foreach (var pack in item.package)
                    {
                        Oasis_Packages itemPack = new Oasis_Packages();

                        itemPack.is_main = pack.is_main;
                        itemPack.oasisId = item.id;
                        itemPack.size = pack.size;
                        itemPack.type = pack.type;
                        itemPack.volume = pack.volume;
                        itemPack.weight = pack.weight;

                        listPacks.Add(itemPack);
                    }

                    Db.Oasis_Packages.AddRange(listPacks);
                    Db.SaveChanges();
                }




                #endregion

                #region Colors

                if (item.colors!=null && item.colors.Count>0)
                {
                    List<Oasis_ProdColors> colorList = new List<Oasis_ProdColors>();

                    foreach (var color in item.colors)
                    {
                        Oasis_ProdColors itemColor = new Oasis_ProdColors();

                        itemColor.Name = color.name;
                        itemColor.OasisId = item.id;
                        itemColor.Pantone = color.pantone;
                        itemColor.Sort = color.sort;

                        colorList.Add(itemColor);
                    }

                    Db.Oasis_ProdColors.AddRange(colorList);
                    Db.SaveChanges();
                }



                #endregion

                #region Images

                if (item.images!=null && item.images.Count>0)
                {
                    List<Oasis_ProdImages> listImages = new List<Oasis_ProdImages>();

                    foreach (var img in item.images)
                    {
                        Oasis_ProdImages image = new Oasis_ProdImages();

                        image.big = img.big;
                        image.OasisId = item.id;
                        image.superbig = img.superbig;
                        image.thumbnail = img.thumbnail;
                        image.updated_at = img.updated_at;

                        listImages.Add(image);
                    }

                    Db.Oasis_ProdImages.AddRange(listImages);
                    Db.SaveChanges();
                }
               

                #endregion

            }

            Db.Oasis_Products.AddRange(listOasisProds);
            Db.SaveChanges();
        }

        private Stream GetValuesTJ(string token, string href)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(href).Result;
                //return response.Content.ReadAsStringAsync().Result;
                return response.Content.ReadAsStreamAsync().Result;
            }

        }

        private List<OasisStokJson> GetValues(string token, string href)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(href).Result;
                //var ttt = response.Content.ReadAsStringAsync().Result;
                //JsonConvert.DeserializeObject<List<OasisStokJson>>(File.ReadAllText(fileTovarJson));
                return JsonConvert.DeserializeObject<List<OasisStokJson>>(response.Content.ReadAsStringAsync().Result.Replace("-", "_"));
            }

        }

        // создаем http-клиента с токеном 
        private HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
     new AuthenticationHeaderValue(
         "Basic",
         Convert.ToBase64String(
             System.Text.ASCIIEncoding.ASCII.GetBytes(
                 string.Format("{0}:{1}", accessToken, ""))));
            }

            return client;

        }


        public void UpdOasisStockFromAPI()
        {
            int vendorId = Db.Vendors.FirstOrDefault(x => x.Name == "Оазис").Id;
            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");



            List<OasisStokJson> tovarListJson = GetValues(token, Href_tovar_api);
            List<OasisStokJson> stockListJson = GetValues(token, Href_stock_api);

            var ttt = stockListJson.Where(x => x.article == "628907").First();
            var ttt2 = stockListJson.Where(x => x.article == "50749").First();
            var ttt3 = stockListJson.Where(x => x.article == "58702").First();


            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();

            Db.Database.ExecuteSqlCommand(String.Format("delete from ShluseForVendors where VendorId = {0}", vendorId));

            //var ttt1 = tovarListJson.FirstOrDefault(x=>x.article=="111310");
            //var ttt2 = stockListJson.FirstOrDefault(x => x.article == "111310");

            foreach (var item in tovarListJson)
            {
                ShluseForVendors stock = new ShluseForVendors();

                stock.VendorId = vendorId;
                stock.Article = item.article;
                stock.VendorStrId = item.id;
                stock.DateUpdate = dateUpd;

                if (item.price != null)
                {
                    try
                    {
                        string price = item.price.Trim().Replace(",", ".").Replace("\r", "").Replace("\n", "");
                        stock.Price = decimal.Parse(price, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }
                }

                if (item.discount_price != null)
                {
                    try
                    {
                        stock.PriceOpt = decimal.Parse(item.discount_price.Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }
                }

                if (item.old_price != null)
                {
                    try
                    {
                        stock.PriceOld = decimal.Parse(item.old_price.Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }
                }

                if (item.rating != null)
                {
                    try
                    {
                        stock.Rating = item.rating;
                    }
                    catch
                    {
                    }
                }


                if (item.discount_group_id != null)
                {
                    try
                    {
                        stock.DiscountGroupId = item.discount_group_id;
                    }
                    catch
                    {
                    }
                }

                stock.Comment = "tovar";

                shluseList.Add(stock);
            }

            foreach (var item in stockListJson)
            {
                ShluseForVendors stock = new ShluseForVendors();

                stock.VendorId = vendorId;
                stock.Article = item.article;
                stock.VendorStrId = item.id;
                stock.DateUpdate = dateUpd;

                try
                {
                    stock.Reserve = item.reserve;
                }
                catch
                {
                }

                try
                {
                    stock.ReserveRemote = item.reserve_remote;
                }
                catch
                {
                }

                try
                {
                    stock.ReserveLocal = item.reserve_local;
                }
                catch
                {
                }

                try
                {
                    stock.ReserveTransit = item.reserve_transit;
                }
                catch
                {
                }

                try
                {
                    stock.Stock = item.stock;
                }
                catch
                {
                }

                try
                {
                    stock.StockRemote = item.stock_remote;
                }
                catch
                {
                }

                stock.Comment = "stock";
                shluseList.Add(stock);

            }


            Db.ShluseForVendors.AddRange(shluseList);
            Db.SaveChanges();
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));

        }

        //sheduler autoStart Update from OASIS API - start to Global.asax
        public class OasisAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                OasisAdminController controller = new OasisAdminController();
                controller.UpdOasisStockFromAPI();
            }
        }

        public class OasisScheduler
        {

            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));

                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<OasisAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                                                            //.StartNow()                            // запуск сразу после начала выполнения
                     .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Minute))
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(2)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Оазис стартанул на автоматическое обновление");

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