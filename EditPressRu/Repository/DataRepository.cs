using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Repository
{
    public partial class DataRepository
    {
        private EditPressRuEntities db = new EditPressRuEntities();

        public virtual List<SelectListItem> GetCityes()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4,6,7, 10, 11, 12, 13, 18, 19, 20, 21, 23, 24, 25, 27, 30, 34, 35, 38, 39, 40, 43, 45, 46, 48, 50, 51, 53 };
            return db.Cityes.Where(x=>list.Contains(x.Id)).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public virtual List<SelectListItem> GetCategories(List<int> list = null)
        {
            List<SelectListItem> rez = new List<SelectListItem>();
            //IQueryable<Categories> Cats = db.Categories.Where(x => x.Publish == true && x.ParentId == 0 && x.Id < 200);
            IQueryable<Categories> Cats = db.Categories.Where(x => x.Publish == true && x.ParentId == 0);

            if (list == null)
            {
                rez = Cats.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = true,Group=new SelectListGroup { Name=x.CpuPath} }).ToList();
            }
            if (list != null && list.Count > 0)
            {
                rez = Cats.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = false, Group = new SelectListGroup { Name = x.CpuPath } }).ToList();
                foreach (var item in list)
                {
                    rez.Where(x => x.Value == item.ToString()).Select(x => { x.Selected = true; return x; }).ToList();
                }
            }

            return rez;
        }


        public virtual ProductViewModel GetTovarModel(int id,string cpu="")
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = db.Products.SingleOrDefault(x => x.Id == id);
            if (model.Product==null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            //model.Atribute = model.Product.Attributes;
            try
            {
                model.Brand = model.Product.Brands.Name;
            }
            catch
            {

                model.Brand=null;
            }
            
            model.ListPictures = model.Product.ProdImages.ToList();
            model.ListVideos = model.Product.ProdVideos.ToList();

            int catId = model.Product.ProdInCategory.FirstOrDefault().Categories.Id;  

            model.SomeProducts = db.Products.Where(x => x.ProdInCategory.Any(c => c.CatId == catId)
            && Math.Abs(x.Price - model.Product.Price) <= 1000 && !String.IsNullOrEmpty(x.ProdImages.FirstOrDefault(c => c.Main).ThumbNail) && x.Id != model.Product.Id).Take(20).OrderBy(x => x.Price).ToList();
            model.ListTopSales = GetTopSales(id);

            List<String> listMaterials = model.Product.Prods_Materials.Select(x => x.Materials_spr.Name).ToList();
            foreach (var item in listMaterials)
            {
                model.Material = String.Format("{0}; ", item);
            }

            List<String> listNanesenie = model.Product.Prods_Nanesenie.Select(x => x.Nanesenie_spr.Name).ToList();
            foreach (var item in listNanesenie)
            {
                model.Nanesenie = model.Nanesenie+String.Format("{0}; ", item);
            }

            List<String> listColors = model.Product.Prods_Colors.Select(x => x.Colors_spr.Name).ToList();
            foreach (var item in listColors)
            {
                model.Color = model.Color + String.Format("{0}; ", item);
            }

            ////Get model.ColorListProducts
            int prodMainId = 0;
            try
            {
                prodMainId = db.GroupColorProducts.FirstOrDefault(x =>x.ProdId==id && x.ParentId==0).ProdId;
            }
            catch
            {
            }

            if (prodMainId==0)
            {
                try
                {
                    prodMainId = db.GroupColorProducts.FirstOrDefault(x => x.ProdId == id && x.ParentId > 0).ParentId;
                }
                catch 
                {
                }
            }

            if (prodMainId>0)
            {
                List<int> prdIdsList = db.GroupColorProducts.Where(x => x.ParentId == prodMainId).Select(x => x.ProdId).ToList();
                prdIdsList.Add(prodMainId);
                List<ColorListItem> listRGB = db.Prods_Colors.Where(x => prdIdsList.Contains(x.ProdId)).
                    Select(x => new ColorListItem { ProdId = x.ProdId, RGB = x.Colors_spr.RGB, ProdLink = "/products/product/" + x.ProdId.ToString(), Img = x.Products.ProdImages.FirstOrDefault(z => z.Main).ThumbNail }).ToList();

                listRGB = listRGB.GroupBy(x => x.RGB).Select(x => x.First()).ToList();

                model.ColorListProducts = listRGB.GroupBy(x => x.ProdId).Select(x => x.First()).ToList();

            }
            else
            {
                model.ColorListProducts = null;
            }

            //Get BreadCrambs
            SqlParameter prodId = new SqlParameter("@prodId", id);
            SqlParameter cpuPath = new SqlParameter("@cpu", cpu);

            model.BreadCrambs = db.Database.SqlQuery<BrdCrmdItem>("EXEC GetBreadCrambsForProd @prodId,@cpu", prodId, cpuPath).ToList();

            //get Makets
            List<Makety> makets = db.Makety.Where(x => x.ProdId == id).ToList();

            model.PdfFiles = makets.Where(x=>x.Ext=="pdf").Select(x=>x.FilePath).ToList();
            model.CdrFiles= makets.Where(x => x.Ext == "cdr").Select(x => x.FilePath).ToList();

            //get Stock

            return model;
        }


        public virtual List<BrdCrmdItem> GetBredCrambs(int catId=0)
        {
            List<BrdCrmdItem> rezult = new List<BrdCrmdItem>();
            string rezTest = TestSubcatOrCat(catId);
            Categories category = db.Categories.FirstOrDefault(x => x.Id == catId);
            int parentId = category.ParentId;

            if (rezTest == "Cat")
            {
               
                string NameCat = category.Name;
                string cpu = category.CpuPath;
                List<Categories> listSubCat = db.Categories.Where(x => x.ParentId == catId).Select(x => x).ToList();

                BrdCrmdItem item0 = new BrdCrmdItem();
                item0.NameItem = "Главная";
                item0.HrefItem = "/";
                item0.Level = 0;

                rezult.Add(item0);

                BrdCrmdItem item1 = new BrdCrmdItem();
                item1.NameItem = NameCat;
                item1.HrefItem = String.Format("/products/{0}", cpu);
                item1.Level = 1;

                rezult.Add(item1);

                foreach (var item in listSubCat)
                {
                    BrdCrmdItem item2 = new BrdCrmdItem();
                    item2.NameItem = item.Name;
                    item2.HrefItem = String.Format("/products/{0}", item.CpuPath);
                    item2.Level = 2;
                    rezult.Add(item2);
                }
            }
            else
            {
                string NameSubCat= category.Name;
                int CatId = category.ParentId;
                string cpuSubCat = category.CpuPath;
                Categories Cat = db.Categories.FirstOrDefault(x => x.ParentId == 0 && x.Id == CatId);
                string NameCat = Cat.Name;
                string  cpuCat = Cat.CpuPath;

                BrdCrmdItem item0 = new BrdCrmdItem();
                item0.NameItem = "Главная";
                item0.HrefItem = "/";
                item0.Level = 0;

                
                BrdCrmdItem item1 = new BrdCrmdItem();
                BrdCrmdItem item2 = new BrdCrmdItem();
                item1.NameItem = NameCat;
                if (parentId==304)
                {
                    item1.HrefItem = "/brands";
                }
                else
                {
                    item1.HrefItem = String.Format("/products/{0}", cpuCat);
                }
                
                item1.Level = 1;

                item2.NameItem = NameSubCat;
                item2.HrefItem = String.Format("/products/{0}", cpuSubCat);
                item2.Level = 1;

                rezult.Add(item0);
                rezult.Add(item1);
                rezult.Add(item2);
            }
           
            return rezult;
        }

        public virtual string TestSubcatOrCat(int id)
        {
            List<int> ListId = db.Categories.Where(x => x.Publish && x.ParentId == 0).Select(x => x.Id).ToList();
            string rez = "";
            if (ListId.Contains(id))
            {
                rez = "Cat";
            }
            else
            {
                rez = "SubCat";
            }
            return rez;
        }

        //public virtual List<Products> GetTopSales(int id = 5)
        //{
        //    return db.ProdInCategory.Where(x => x.CatId == id).Select(x => x.Products).Where(x => x.TopSales).Take(3).OrderBy(x => x.Price).ToList();
        //}

        public virtual List<Products> GetTopSales(int prodId)
        {
            int catId = db.ProdInCategory.FirstOrDefault(x => x.ProdId == prodId).CatId;
            List<Products> rezList = new List<Products>();
            rezList= db.ProdInCategory.Where(x => x.CatId == catId).
                Select(x => x.Products)
                .Where(x => x.Publish && !String.IsNullOrEmpty(x.Descript) && x.Price > 50 && x.ProdImages.Count>0).Take(3).OrderBy(x => x.Price).ToList();
            if (rezList==null || rezList.Count==0)
            {
                rezList= db.ProdInCategory.Where(x => x.CatId == 5).Select(x => x.Products).Where(x => x.TopSales).Take(3).OrderBy(x => x.Price).ToList();
            }
            return rezList;
        }

        public virtual async Task<List<Products>> GetTopSalesListForContacts()
        {
            List<Products> list = new List<Products>();
            list = db.Products.Where(x => x.Publish == true && x.Hit == true).Take(5).ToList();
            return await Task.FromResult(list);
        }


        public virtual int GetCartOrderId(int userId,string Sid)
        {
            int orderId = 0;
            
            //1) Проверить есть ли открытая корзина
            if (userId > 0)
            {
                orderId = db.OrderDetails.FirstOrDefault(x => x.Orders.UserID == userId && x.Orders.StatusId == 7).OrderID;
            }
            else
            {
                orderId = db.OrderDetails.FirstOrDefault(x => x.Orders.Sid == Sid && x.Orders.StatusId == 7).OrderID;
            }

            return orderId;
        }

        public virtual int GetCurrentUserIdbyName(string userName)
        {
            return db.UserProfile.FirstOrDefault(x => x.UserName.Equals(userName)).UserId;
        }
        

    }
}