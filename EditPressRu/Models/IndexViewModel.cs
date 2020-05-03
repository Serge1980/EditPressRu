using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Models
{
    public class IndexViewModel
    {
        public int id { get; set; }
        public string Cpu { get; set; }

        public PagerResult<ProdList> ProdForPaging { get; set; }
        public PageInfo PageInfo { get; set; }

        public String QueryString { get; set; }
        public List<int> ListCatId { get; set; }

        public decimal MinCatPrice { get; set; }
        public decimal MaxCatPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<SelectListItem> Materials { get; set; }
        public List<SelectListItem> Nanesenie { get; set; }
        public List<SelectListItem> ColorAll { get; set; }
        public List<SelectListItem> ColorSelected { get; set; }
        public List<SelectListItem> Brand { get; set; }
        public bool Nalichie { get; set; }
        public bool DownRankOrder { get; set; } //default true
        public bool UpPriceOrder { get; set; } //default true
        //public PageAndProd SubModel { get; set; }
        public List<BrdCrmdItem> BreadCrambs { get; set; }

        public String Description { get; set; }

        public SaleHitNewModel SaleHitNew { get; set; }

        //for Page Sales
        public List<SelectListItem> CatSaleList { get; set; }

    }


    public class ProdList
    {
        public int ProdId { get; set; }
        public bool Main { get; set; }
        public bool Hit { get; set; }
        public bool New { get; set; }
        public bool Sale { get; set; }
        public bool Nalichie { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public decimal Sort { get; set; }
        public string  Name { get; set; }
        public string ImagePath { get; set; }
        public string SaleSize { get; set; }
        public int Rank { get; set; }
        public string QueryString { get; set; }
        public List<NestedProdList> GroupProduct { get; set; }
    }

    public class NestedProdList
    {
        public String Link { get; set; }
        public String Img { get; set; }
    }


    public class LineJsonModel
    {
        public int id { get; set; }
        public string Cpu { get; set; }

        public int page { get; set; }
        public int PageProductCount { get; set; }

        public String QueryString { get; set; }
        public List<int> ListCatId { get; set; }

        public int MinCatPrice { get; set; } //1.1
        public int MaxCatPrice { get; set; } //1.2
        public int MinPrice { get; set; } //1.3
        public int MaxPrice { get; set; } //1.4
        public List<String> Materials { get; set; }
        public List<String> Nanesenie { get; set; }
        public List<String> Color { get; set; }
        public List<String> Brand { get; set; }
        public bool Nalichie { get; set; }
        public bool? DownRankOrder { get; set; } //default false
        public bool? UpPriceOrder { get; set; } //default true

        public int HitCount { get; set; }
        public int SaleCount { get; set; }
        public int NewCount { get; set; }
        public bool Hit { get; set; }
        public bool New { get; set; }
        public bool Sale { get; set; }

        //введено для того, чтобы задать признак -возвращать из запроса все или только с ценой
        //false - all, true-only with prirce
        public bool SpriceZprs { get; set; }

    }

    public class SaleHitNewModel
    {
        public int HitCount { get; set; }
        public int SaleCount { get; set; }
        public int NewCount { get; set; }
        public bool Hit { get; set; }
        public bool New { get; set; }
        public bool Sale { get; set; }
    }
}