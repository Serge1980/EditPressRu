using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Controllers;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static EditPressRu.Areas.Adminka.Models.OceanStoreobjects;
using System.Threading.Tasks;
using Quartz.Impl;
using Quartz;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class OceanAdminController : BaseController
    {
        // GET: Adminka/Ocean
        private EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Adminka/GiftAdmin
        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;
            int vendorId = 6;

            OceanAdminIndexViemModel model = new OceanAdminIndexViemModel();

            IQueryable<VendorListStocks> Tovars = Db.Products.Where(x => x.VendorId == vendorId).
                Select(x => new VendorListStocks
                {
                    //PdfFiles = x.Makets.Where(c => !String.IsNullOrEmpty(c.pdf)).Select(c => c.pdf).ToList(),
                    //CdrFiles = x.Makets.Where(c => !String.IsNullOrEmpty(c.cdr)).Select(c => c.cdr).ToList(),
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


        public ActionResult UpdateOceanStock()
        {
            //ViewBag.Categories = db.GetCategories();

            UpdateOceanFromAPI();
            return Redirect("/Adminka/OceanAdmin/Index");
        }

        //sheduler autoStart Update from OASIS API - start to Global.asax
        public class OceanAPIupdater : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                OceanAdminController controller = new OceanAdminController();
                controller.UpdateOceanStock();
            }
        }

        public class OceanScheduler
        {
            public static async void Start()
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Global));

                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<OceanAPIupdater>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger6", "group6")     // идентифицируем триггер с именем и группой
                                                            //.StartNow()                            // запуск сразу после начала выполнения
                     .StartAt(DateBuilder.FutureDate(59, IntervalUnit.Minute))
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(2)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы

                log.Info("Океан стартанул на автоматическое обновление");

            }
        }


        #region auxiliary methods


        public void UpdateOceanFromAPI()
        {
            string urlStores = @"http://www.oceangifts.ru/upload/stores.json";
            List<OceanRootObject> tovarListJson = GetValues(urlStores);

            int vendorId = Db.Vendors.FirstOrDefault(x => x.Name == "OceanGift").Id;
            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();



            foreach (var item in tovarListJson)
            {
                ShluseForVendors prod = new ShluseForVendors();
                prod.VendorId = vendorId;
                prod.DateUpdate = dateUpd;
                prod.VendorIntId = item.product_id;
                prod.Article = item.article;

                try
                {
                    prod.Price = decimal.Parse(item.price.ToString());
                }
                catch
                {
                }

                int cntRem = 0;
                foreach (var itemrem in item.stores.remains)
                {
                    cntRem = cntRem + itemrem.count;
                }

                int cntRes = 0;
                foreach (var itemres in item.stores.reserves)
                {
                    cntRes = cntRes + itemres.count;
                }
                prod.Reserve = cntRes;
                prod.Stock = cntRem;
                shluseList.Add(prod);

            }

            Db.ShluseForVendors.AddRange(shluseList);
            Db.SaveChanges();
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
            Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));

        }

        static List<OceanRootObject> GetValues(string href)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync(href).Result;

                return JsonConvert.DeserializeObject<List<OceanRootObject>>(response.Content.ReadAsStringAsync().Result);
            }

        }

        static HttpClient CreateClient()
        {
            var client = new HttpClient();
            return client;

        }
        #endregion
    }

}