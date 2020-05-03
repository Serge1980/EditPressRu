using EditPressRu.Helpers;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class VendorAdminIndexViewModel
    {
        public String Title { get; set; }
        public String LastDatUpdate { get; set; }
        public PagerResult<VendorListStocks> ProdForPaging { get; set; }
        public PageInfo PagInfo { get; set; }
        public List<CatWithSubcat> CatList { get; set; } //перевести все на ListCategories и убрать
        //experiment
        public List<Categories> ListCategories { get; set; }
        public int CatId { get; set; }

        //for search
        public String Artikle { get; set; }
        public String Name { get; set; }
    }
}