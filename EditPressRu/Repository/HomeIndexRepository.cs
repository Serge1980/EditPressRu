using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Repository
{
    public partial class DataRepository
    {
        //*********************************************************************************************//
        #region GetSubcatList and Products


        public virtual IndexProductsViewModel GetIndexProductsViewModel()
        {
            IndexProductsViewModel model = new IndexProductsViewModel();

            model.Fleshki = db.Categories.Where(x => x.Publish && x.ParentId == 5).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Ruchki = db.Categories.Where(x => x.Publish && x.ParentId == 4).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name}).ToList();
            model.Egednevnik = db.Categories.Where(x => x.Publish && x.ParentId == 6).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Gadgety = db.Categories.Where(x => x.Publish && x.ParentId == 20).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Umbrellas = db.Categories.Where(x => x.Publish && x.ParentId == 14).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Tematich = db.Categories.Where(x => x.Publish && x.ParentId == 3).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Gifts = db.Categories.Where(x => x.Publish && x.ParentId == 7).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Office = db.Categories.Where(x => x.Publish && x.ParentId == 10).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Brelok = db.Categories.Where(x => x.Publish && x.ParentId == 11).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Prazdnik = db.Categories.Where(x => x.Publish && x.ParentId == 2).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Sladosty = db.Categories.Where(x => x.Publish && x.ParentId == 22).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Posuda = db.Categories.Where(x => x.Publish && x.ParentId == 13).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Promo = db.Categories.Where(x => x.Publish && x.ParentId == 9).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Vip = db.Categories.Where(x => x.Publish && x.ParentId == 8).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Games = db.Categories.Where(x => x.Publish && x.ParentId == 16).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Chasy = db.Categories.Where(x => x.Publish && x.ParentId == 12).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Odegda = db.Categories.Where(x => x.Publish && x.ParentId == 15).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Sezonnie = db.Categories.Where(x => x.Publish && x.ParentId == 1).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Dom = db.Categories.Where(x => x.Publish && x.ParentId == 17).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Sad = db.Categories.Where(x => x.Publish && x.ParentId == 19).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Sumky = db.Categories.Where(x => x.Publish && x.ParentId == 18).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();
            model.Turizm = db.Categories.Where(x => x.Publish && x.ParentId == 21).Select(x => new IndexEntity { CpuPath =x.CpuPath, Name = x.Name }).ToList();

            model.ListSaleProducts = db.Database.SqlQuery<ProdForSlider>("exec GetProductsToSaleSlider").ToList();

            model.ListProdCarusel = db.Database.SqlQuery<ProdListCarusel>("exec GetProductsForCrusel").ToList();

            return model;
        }

        

        #endregion
        //-------------------------------------------------------------------------------------------------//
    }
}