using CsQuery;
using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class CPrestigeController : Controller
    {
        private string mainUrl = "http://www.center-prestige.ru/";
        const int vendorId = 11;

        // GET: Adminka/CPrestige
        //_doc.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", classToFind));
        //_doc.DocumentNode.SelectNodes("//*[contains(@class,'float')]")

        public ActionResult Index(int page = 1, int cat = 0, string Artikle = "", string Name = "")
        {
            int pageSize = 30;

            CPrestigeViewModel model = new CPrestigeViewModel();

            using (EditPressRuEntities Db = new EditPressRuEntities())
            {
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

                var listProd = model.ProdForPaging.Items.Select(x => x.Product);
                if (listProd!=null)
                {
                    model.LastDatUpdate = listProd.Max(x => x.LastUpdate).ToString();
                }
                
            }
            return View(model);
        }

        public ActionResult GetStockCprestige()
        {
            EditPressRuEntities Db = new EditPressRuEntities();

            DateTime DateUpd = DateTime.Now;
            var dateUpd = DateUpd.ToString("yyyy-MM-dd HH':'mm':'ss").Replace(" ", "T");

            List<ShluseForVendors> shluseList = new List<ShluseForVendors>();
            Db.Database.ExecuteSqlCommand(String.Format("delete from ShluseForVendors where VendorId = {0}", vendorId));

            List<string> prodList = GetStockListProdHref();

            //List<string> prodList = Db.CPrestige_Products.Where(x=>x.Href.Contains("videobuklety")).Select(x=>x.Href).ToList();

            //List<string> prodList = new List<string>() { "/Netshop/tematicheskie-suveniri/stranitsa-energetika/stranitsa-energetika_49949.html" };

            foreach (var url in prodList)
            {
                string prodCardUrl = mainUrl.Remove(mainUrl.Length - 1) + url;

                var Webget = new HtmlWeb();

                try
                {
                    var yyy = Webget.Load(prodCardUrl);
                }
                catch
                {
                    continue;
                }

                var doc = Webget.Load(prodCardUrl);

                ShluseForVendors prod = new ShluseForVendors();

                prod.DateUpdate = dateUpd;

                decimal price = 0;
                string artikle = "";
                //var ttt = "";
                int stock = 0;
                int free = 0;


                try
                {
                    string price1 = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'product-page-price')]").InnerText.Replace(" &#8381;", "");
                    price = Decimal.Parse(price1);
                }
                catch
                {
                }

                try
                {
                    artikle = doc.DocumentNode.SelectSingleNode("//p[contains(@class,'text-muted')]").InnerText.Replace("Артикул: ", "");

                }
                catch
                {
                }


                try
                {

                    HtmlNodeCollection tttList = doc.DocumentNode.SelectNodes("//table[contains(@class,'product-page-features-table')]//tr");

                    foreach (var item in tttList)
                    {
                        if (item.InnerText.Contains("Остаток"))
                        {
                            //stock=item.InnerHtml.ToString();
                            string stock1 = item.InnerText.Replace("Остаток", "").Replace(" ", "").Replace("\n", "").Replace("шт.", "");

                            try
                            {
                                stock = int.Parse(stock1);
                            }
                            catch
                            {

                            }
                        }

                        if (item.InnerText.Contains("Свободно"))
                        {
                            string free1 = item.InnerText.Replace("Свободно", "").Replace(" ", "").Replace("\n", "").Replace("шт.", "");

                            try
                            {
                                free = int.Parse(free1);
                            }
                            catch
                            {

                            }
                        }
                    }


                }
                catch
                {
                }



                prod.VendorId = vendorId;

                //          ,FreeCnt as[Stock]
                //,0 as[Stock_remote]
                //,0 as[Stock_transit]
                //,0 as[Stock_local]
                //,Stock - FreeCnt as[Reserve]


                prod.Stock = stock;
                prod.Reserve = stock - free;
                prod.Price = price;


                prod.Comment = "all";
                shluseList.Add(prod);
            }

            if (shluseList.Count > 0)
            {
                Db.ShluseForVendors.AddRange(shluseList);
                Db.SaveChanges();
                Db.Database.ExecuteSqlCommand(String.Format("exec UpdateProductsFromShluse @VendorId = {0}", vendorId));
                ////Db.Database.ExecuteSqlCommand(String.Format("exec UpdateSaleSizeOasisStock @VendorId = {0},@date='{1}'", vendorId, dateUpd));
            }

            return RedirectToAction("Index");
        }

        private List<string> GetStockListCatHref()
        {
            //Очищаем категории

            //Получаем список категорий (пока без субкатегорий)
            //CQ dom = new CQ();
            //dom = CQ.CreateFromUrl(mainUrl);

            //CQ cats = dom["#titleCatalogMenu div a"];

            var Webget = new HtmlWeb();
            var doc = Webget.Load("https://www.center-prestige.ru/Netshop");

            var cats = doc.DocumentNode.SelectNodes("//ul[@class='cateogry-filters-list']//a");

            List<string> catList = new List<string>();

            foreach (var item in cats)
            {
                catList.Add(item.GetAttributeValue("href", "unknown"));
            }

            //******************************************************
            //Получаем список подкатегорий - перешел на agilitiPack - csQuery задолбал - ghjt,fk  сним более 4х часов
            //- цикл выше если будет время тоже переведу на агилиnи и выкину нахрен csQuery из проекта

            List<string> subCatList = new List<string>();

            foreach (var item in catList)
            {
                string subCatUrl = mainUrl.Remove(mainUrl.Length - 1) + item;

                var Webget2 = new HtmlWeb();
                var doc2 = Webget2.Load(subCatUrl);

                var subCats = doc2.DocumentNode.SelectNodes("//a[@class='banner-category owl-item-slide']");

                if (subCats != null && subCats.Count > 0)
                {


                    foreach (var subitem in subCats)
                    {
                        string href = subitem.GetAttributeValue("href", "unknown");
                        subCatList.Add(href);


                    }

                }
            }

            //3 уровень
            List<string> treeHrefList = new List<string>();
            foreach (var item in subCatList)
            {
                string threeCatUrl = mainUrl.Remove(mainUrl.Length - 1) + item;

                var Webget3 = new HtmlWeb();
                var doc3 = Webget3.Load(threeCatUrl);


                try
                {
                    var threeCats = doc3.DocumentNode.SelectNodes("//a[@class='banner-category owl-item-slide']");

                    if (threeCats != null && threeCats.Count > 0)
                    {
                        foreach (var threeitem in threeCats)
                        {
                            string threeHref = threeitem.GetAttributeValue("href", "unknown");
                            treeHrefList.Add(threeHref);
                        }
                    }
                }
                catch
                {
                }

            }
            subCatList.AddRange(treeHrefList);

            catList.AddRange(subCatList);

            return catList;

        }

        private List<string> GetStockListProdHref()
        {
            List<string> rezList = new List<string>();

            List<string> listCatHref = GetStockListCatHref();

            foreach (var catUrl in listCatHref)
            {
                int i = 1;
                int cnt = 60;

                while (cnt == 60)
                {
                    cnt = 0;
                    string prodsHref = mainUrl.Remove(mainUrl.Length - 1) + catUrl + i.ToString() + "/?num=60";

                    var Webget = new HtmlWeb();
                    var doc = Webget.Load(prodsHref);

                    var prodsList = doc.DocumentNode.SelectNodes("//a[@class='product-link']");

                    if (prodsList != null && prodsList.Count > 0)
                    {
                        foreach (var item in prodsList)
                        {
                            rezList.Add(item.GetAttributeValue("href", "unknown"));
                        }

                        cnt = prodsList.Count;
                        i++;
                    }

                }




            }

            return rezList;
        }

        //private List<string> GetStockPagingProdHref()
        //{


        //}


        #region Парсер

        public ActionResult ParseToBase()
        {
            //Скачиваем - парсим в базу категории
            GetCategories();

            //Скачиваем - парсим в базу ссылки на продукты
            GetProducts();

            //По скачанным ссылкам парсим продукты в таблицу CPrestige_Products
            FillProducts();

            //Запускаем процедуру предварительной очистки полученных данных
            using (EditPressRuEntities Db = new EditPressRuEntities())
            {
                Db.Database.ExecuteSqlCommand("EXEC Update_CPrestige_Products_AfterAdding");
            }

            //Запускаем процедуру Обновления существующих товаров с вениры
            using (EditPressRuEntities Db = new EditPressRuEntities())
            {
                Db.Database.ExecuteSqlCommand("EXEC Replace_Venira_CPrestige");
            }

            ////////////////Докачка картинок
            //////////////DownLoadImgLast();



            ////////ToDo
            ////////Здесь Хорошо бы заменить на диске старые картинки товаров, которые уже у нас есть
            //////// на новые скачанные, но этот этап я пропущу - закоментирована процедура получения
            ////////списка старых картинок

            ////////using (EditPressRuEntities Db=new EditPressRuEntities())
            ////////{
            ////////   List<string> listOfDirImages= Db.Database.SqlQuery<string>("EXEC [dbo].[CPrestige_GetListImagesFolders]").ToList();
            ////////}



            return View("Index");
        }


        private void GetCategories()
        {
            //Очищаем категории
            EditPressRuEntities Db = new EditPressRuEntities();

                Db.Database.ExecuteSqlCommand("delete from CPrestige_Categories");
                Db.Database.ExecuteSqlCommand("delete from CPrestige_CatProd");

                //Получаем список категорий (пока без субкатегорий)
                var Webget = new HtmlWeb();
                var doc = Webget.Load("https://www.center-prestige.ru/Netshop");

                var cats = doc.DocumentNode.SelectNodes("//ul[@class='cateogry-filters-list']//a");

                List<CPrestige_Categories> catList = new List<CPrestige_Categories>();

                foreach (var item in cats)
                {

                    CPrestige_Categories cat = new CPrestige_Categories();

                    cat.Href = item.GetAttributeValue("href", "unknown"); ;
                    cat.Name = item.InnerText;
                    cat.ParentId = 0;

                    catList.Add(cat);
                    //var ttt = item.InnerText;
                }

                Db.CPrestige_Categories.AddRange(catList);
                Db.SaveChanges();
           
            //******************************************************
            //Получаем список подкатегорий 
    
                foreach (var item in Db.CPrestige_Categories.ToList())
                {
                    string subCatUrl = mainUrl.Remove(mainUrl.Length - 1) + item.Href;

                    var Webget2 = new HtmlWeb();
                    var doc2 = Webget2.Load(subCatUrl);

                    var subCats = doc2.DocumentNode.SelectNodes("//a[@class='banner-category owl-item-slide']");//("SelectNodes("//*[contains(@class,'float')]")")

                    if (subCats != null && subCats.Count > 0)
                    {
                        List<CPrestige_Categories> subCatList = new List<CPrestige_Categories>();

                        foreach (var subitem in subCats)
                        {
                            CPrestige_Categories subCat = new CPrestige_Categories();

                            subCat.Href = subitem.GetAttributeValue("href", "unknown");
                            subCat.Name = subitem.SelectSingleNode("//h5[@class='banner-category-title']").InnerText;
                            subCat.ParentId = item.Id;

                            subCatList.Add(subCat);

                        }

                        Db.CPrestige_Categories.AddRange(subCatList);
                        Db.SaveChanges();
                    }
                }

                //3 уровень
               
                foreach (var item in Db.CPrestige_Categories.Where(x=>x.ParentId>0))
                {
                    string threeCatUrl = mainUrl.Remove(mainUrl.Length - 1) + item.Href;

                    var Webget3 = new HtmlWeb();
                    var doc3 = Webget3.Load(threeCatUrl);


                    try
                    {
                        var threeCats = doc3.DocumentNode.SelectNodes("//a[@class='banner-category owl-item-slide']");

                        if (threeCats != null && threeCats.Count > 0)
                        {
                            List<CPrestige_Categories> subCatList = new List<CPrestige_Categories>();

                            foreach (var threeitem in threeCats)
                            {
                                CPrestige_Categories subCat = new CPrestige_Categories();

                                subCat.Href = threeitem.GetAttributeValue("href", "unknown");
                                subCat.Name = threeitem.SelectSingleNode("//h5[@class='banner-category-title']").InnerText; 
                                subCat.ParentId = item.Id;
                                
                            }

                            Db.CPrestige_Categories.AddRange(subCatList);
                            Db.SaveChanges();
                        }
                    }
                    catch
                    {
                    }

                }


        }

        private void GetProducts()
        {
            EditPressRuEntities Db = new EditPressRuEntities();

            Db.Database.ExecuteSqlCommand("delete from CPrestige_CatProd");
            Db.Database.ExecuteSqlCommand("delete from CPrestige_Products");

            List<CPrestige_Categories> listCat = Db.CPrestige_Categories.ToList();

            foreach (var cat in listCat)
            {
                string prodsHref = mainUrl.Remove(mainUrl.Length - 1) + cat.Href;

                var Webget = new HtmlWeb();
                var doc = Webget.Load(prodsHref);

                var prodsList = doc.DocumentNode.SelectNodes("//div[@class='prodImg']//a");

                if (prodsList != null && prodsList.Count > 0)
                {

                    foreach (var item in prodsList)
                    {
                        CPrestige_Products product = new CPrestige_Products();
                        CPrestige_CatProd prodCat = new CPrestige_CatProd();

                        product.Href = item.GetAttributeValue("href", "unknown");

                        Db.CPrestige_Products.Add(product);
                        Db.SaveChanges();

                        prodCat.CatId = cat.Id;
                        prodCat.ProdId = product.Id;

                        Db.CPrestige_CatProd.Add(prodCat);
                        Db.SaveChanges();

                    }

                }
            }
        }

        private void FillProducts()
        {
            EditPressRuEntities Db = new EditPressRuEntities();

            //Удалим картинки и макеты -очистим место на диске

            string fldrMak = Db.CPrestige_Makety.FirstOrDefault().FilePath;

            string dirMakets = Server.MapPath("/MaketsCPrestige");
            string dirImages = Server.MapPath("/ImagesCPrestige");

            EmptyFolder(new DirectoryInfo(dirMakets));
            EmptyFolder(new DirectoryInfo(dirImages));

            Db.Database.ExecuteSqlCommand("delete from CPrestige_Makety");
            Db.Database.ExecuteSqlCommand("delete from CPrestige_Images");

            //List<CPrestige_Products> prodList = Db.CPrestige_Products.ToList();
            //List<CPrestige_Products> prodList = Db.CPrestige_Products.Where(x => x.Href == "/Netshop/9-may/zazhigalki/RPD_87.html").ToList();
            //List<CPrestige_Products> prodList = Db.CPrestige_Products.Where(x => x.Href == "/Netshop/nagradnaya-produktsiya/nagradi-iz-stekla/s32.html").ToList();
            //List<CPrestige_Products> prodList = Db.CPrestige_Products.Where(x => x.Href == "/Netshop/tematicheskie-suveniri/stranitsa-medika/stranitsa-medika_30692.html").ToList();

            List<CPrestige_Products> prodList = Db.CPrestige_Products.ToList();

            string rendomFldr = Guid.NewGuid().ToString();

            string contImgFdr = String.Format("/ImagesCPrestige/{0}", rendomFldr);

            string contMakFdr = String.Format("/MaketsCPrestige/{0}", rendomFldr);


            List<CPrestige_Makety> maketList = new List<CPrestige_Makety>();
            List<CPrestige_Images> imagesList = new List<CPrestige_Images>();

            foreach (var prod in prodList)
            {
                string prodCardUrl = mainUrl.Remove(mainUrl.Length - 1) + prod.Href;

                var Webget = new HtmlWeb();
                var doc = Webget.Load(prodCardUrl);




                try
                {
                    prod.Name = doc.DocumentNode.SelectSingleNode("//title").InnerText;

                }
                catch
                {
                    prod.Name = "Купить необычный сувенир в компании ЭдитПресс";
                }



                try
                {
                    prod.Artikul = doc.GetElementbyId("prodParams").ChildNodes[1].InnerText;
                }
                catch
                {
                }


                try
                {
                    var tt = doc.DocumentNode.SelectSingleNode("//div[@class='prodPrice']").InnerText;
                    prod.Price = doc.DocumentNode.SelectSingleNode("//div[@class='prodPrice']").InnerText.Replace(" &#8381", "").Replace(";", "");
                }
                catch
                {
                }

                var tt1 = "";

                try
                {
                    tt1 = doc.GetElementbyId("prodParams").SelectSingleNode("//dl").InnerText;
                }
                catch
                {
                }



                if (tt1 != "")
                {
                    string qq1 = tt1.Replace("\t\t\t\t\t\t\t\t", "|").Replace(" ", "").Replace("\n", "").Replace("\t", "");

                    List<string> list = qq1.Split('|').ToList();

                    foreach (var item in list)
                    {
                        if (item.Contains("Остаток"))
                        {
                            if (item.Contains("запросу"))
                            {
                                prod.Stock = 0;
                            }
                            else
                            {
                                try
                                {
                                    string ostat = item.Replace("Остаток", "").Replace("шт.", "");
                                    prod.Stock = int.Parse(ostat);
                                }
                                catch
                                {
                                }
                            }
                        }


                        if (item.Contains("Свободно"))
                        {
                            if (item.Contains("запросу"))
                            {
                                prod.FreeCnt = 0;
                            }
                            else
                            {
                                try
                                {
                                    string free = item.Replace("Свободно", "").Replace("шт.", "");
                                    prod.FreeCnt = int.Parse(free);
                                }
                                catch
                                {
                                }

                            }
                        }

                        if (item.Contains("Цвет"))
                        {
                            try
                            {
                                prod.Color = item.Replace("Цвет", "");
                            }
                            catch
                            {
                            }

                        }

                        if (item.Contains("Материал"))
                        {
                            try
                            {
                                prod.Material = item.Replace("Материал", "");
                            }
                            catch
                            {
                            }

                        }

                        if (item.Contains("Размеры"))
                        {
                            try
                            {
                                prod.Size = item.Replace("Размеры", "");
                            }
                            catch
                            {
                            }
                        }

                        if (item.Contains("Нанесение"))
                        {
                            try
                            {
                                prod.Nanesenie = item.Replace("Нанесение", "");
                            }
                            catch
                            {
                            }

                        }

                    }

                    Db.SaveChanges();

                    //Makets
                    if (tt1.Contains("Векторный макетскачать"))
                    {


                        // DownloadFile(hrefMaket);
                        try
                        {
                            var hrefMaket = mainUrl.Remove(mainUrl.Length - 1) + doc.GetElementbyId("prodParams").SelectSingleNode(".//dd//a").GetAttributeValue("href", "unknown");

                            string tekFldr = prod.Id.ToString();

                            CPrestige_Makety maket = new CPrestige_Makety();

                            maket.ProdId = prod.Id;
                            maket.FilePath = DownloadFile(hrefMaket, contMakFdr, tekFldr);
                            maket.Ext = "cdr";

                            maketList.Add(maket);
                        }
                        catch
                        {
                        }




                    }
                }


                ////Images


                string tekFldrImg = prod.Id.ToString();

                try
                {
                    string restPath = doc.GetElementbyId("prodImages").SelectSingleNode(".//a").GetAttributeValue("href", "unknown");

                    if (String.IsNullOrEmpty(restPath))
                    {
                        restPath = doc.GetElementbyId("prodImages").SelectSingleNode(".//img").GetAttributeValue("src", "unknown");
                    }

                    //string urlImg = mainUrl.Remove(mainUrl.Length - 1) + doc.GetElementbyId("prodImages").SelectSingleNode(".//a").GetAttributeValue("href", "unknown");

                    string urlImg = mainUrl.Remove(mainUrl.Length - 1) + restPath;

                    CPrestige_Images cpImage = new CPrestige_Images();

                    cpImage.Image = DownloadFile(urlImg, contImgFdr, tekFldrImg);
                    cpImage.Main = true;
                    cpImage.ProdId = prod.Id;

                    imagesList.Add(cpImage);

                }
                catch
                {
                }


                //otherFoto

                try
                {
                    var dopFotos = doc.GetElementbyId("otherPhoto").SelectNodes(".//a");


                    if (dopFotos != null && dopFotos.Count > 0)
                    {

                        foreach (var item in dopFotos)
                        {
                            try
                            {
                                string urlDopImg = mainUrl.Remove(mainUrl.Length - 1) + item.GetAttributeValue("href", "unknown");

                                CPrestige_Images cpImage1 = new CPrestige_Images();
                                cpImage1.Image = DownloadFile(urlDopImg, contImgFdr, tekFldrImg);
                                cpImage1.Main = false;
                                cpImage1.ProdId = prod.Id;

                                imagesList.Add(cpImage1);
                            }
                            catch
                            {
                            }


                        }

                    }

                }
                catch
                {
                }
                //var dopFotos1 = doc.GetElementbyId("otherPhoto").SelectSingleNode(".//a//img").GetAttributeValue("src", "unknown");

            }

            Db.CPrestige_Makety.AddRange(maketList);
            Db.CPrestige_Images.AddRange(imagesList);
            Db.SaveChanges();


        }


        private string DownloadFile(string url, string containerFldr, string tekFldr)
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            string filename = "";
            string destinationpath = String.Format("{0}/{1}", containerFldr, tekFldr);
            if (!Directory.Exists(Server.MapPath(destinationpath)))
            {
                Directory.CreateDirectory(Server.MapPath(destinationpath));
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result)
            {
                string path = response.Headers["Content-Disposition"];
                if (string.IsNullOrWhiteSpace(path))
                {
                    var uri = new Uri(url);
                    filename = Path.GetFileName(uri.LocalPath);
                }
                else
                {
                    ContentDisposition contentDisposition = new ContentDisposition(path);
                    filename = contentDisposition.FileName;

                }

                var responseStream = response.GetResponseStream();
                using (var fileStream = System.IO.File.Create(System.IO.Path.Combine(Server.MapPath(destinationpath), filename)))
                {
                    responseStream.CopyTo(fileStream);
                }
            }

            return String.Format("{0}/{1}", destinationpath, filename);
        }

        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }

        #endregion

        ////////////////private void DownLoadImgLast()
        ////////////////{
        ////////////////    EditPressRuEntities Db = new EditPressRuEntities();
        ////////////////    List<CPrestige_Products> prodList = Db.Database.SqlQuery<CPrestige_Products>(@"select * from CPrestige_Products
        ////////////////                                                                                    where Artikul in (select Article from Products
        ////////////////                                                                                    where Id not in (SELECT ProdId from ProdImages)
        ////////////////                                                                                    and VendorId = 11)").ToList(); //Db.CPrestige_Products.ToList();

        ////////////////    string rendomFldr = Guid.NewGuid().ToString();

        ////////////////    string contImgFdr = String.Format("/ImagesCPrestige/{0}", rendomFldr);

        ////////////////    List<CPrestige_Images> imagesList = new List<CPrestige_Images>();

        ////////////////    foreach (var prod in prodList)
        ////////////////    {
        ////////////////        string prodCardUrl = mainUrl.Remove(mainUrl.Length - 1) + prod.Href;

        ////////////////        var Webget = new HtmlWeb();
        ////////////////        var doc = Webget.Load(prodCardUrl);

        ////////////////        ////Images


        ////////////////        string tekFldrImg = prod.Id.ToString();

        ////////////////        try
        ////////////////        {
        ////////////////            string restPath= doc.GetElementbyId("prodImages").SelectSingleNode(".//a").GetAttributeValue("href", "unknown");

        ////////////////        if (String.IsNullOrEmpty(restPath))
        ////////////////        {
        ////////////////            restPath = doc.GetElementbyId("prodImages").SelectSingleNode(".//img").GetAttributeValue("src", "unknown");
        ////////////////        }
        ////////////////        string urlImg = mainUrl.Remove(mainUrl.Length - 1) + restPath;

        ////////////////            CPrestige_Images cpImage = new CPrestige_Images();

        ////////////////            cpImage.Image = DownloadFile(urlImg, contImgFdr, tekFldrImg);
        ////////////////            cpImage.Main = true;
        ////////////////            cpImage.ProdId = prod.Id;

        ////////////////            imagesList.Add(cpImage);

        ////////////////    }
        ////////////////        catch
        ////////////////    {
        ////////////////    }


        ////////////////    //otherFoto

        ////////////////    try
        ////////////////        {
        ////////////////            var dopFotos = doc.GetElementbyId("otherPhoto").SelectNodes(".//a");


        ////////////////            if (dopFotos != null && dopFotos.Count > 0)
        ////////////////            {

        ////////////////                foreach (var item in dopFotos)
        ////////////////                {
        ////////////////                    try
        ////////////////                    {
        ////////////////                        string urlDopImg = mainUrl.Remove(mainUrl.Length - 1) + item.GetAttributeValue("href", "unknown");

        ////////////////                        CPrestige_Images cpImage1 = new CPrestige_Images();
        ////////////////                        cpImage1.Image = DownloadFile(urlDopImg, contImgFdr, tekFldrImg);
        ////////////////                        cpImage1.Main = false;
        ////////////////                        cpImage1.ProdId = prod.Id;

        ////////////////                        imagesList.Add(cpImage1);
        ////////////////                    }
        ////////////////                    catch
        ////////////////                    {
        ////////////////                    }


        ////////////////                }

        ////////////////            }

        ////////////////        }
        ////////////////        catch
        ////////////////        {
        ////////////////        }


        ////////////////    }


        ////////////////    Db.CPrestige_Images.AddRange(imagesList);
        ////////////////    Db.SaveChanges();

        ////////////////}



    }
}