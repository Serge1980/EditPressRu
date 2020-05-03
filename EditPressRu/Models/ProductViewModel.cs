using EditPressRu.Models.DB;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EditPressRu.Models
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public List<Products> ListTopSales { get; set; }
        public List<Products> SomeProducts { get; set; }
        public List<ProdImages> ListPictures { get; set; }
        public List<ProdVideos> ListVideos { get; set; }
        public List<BrdCrmdItem> BreadCrambs { get; set; }
        public List<ColorListItem> ColorListProducts { get; set; }
        public List<string> PdfFiles { get; set; }
        public List<string> CdrFiles { get; set; }
        public string Nanesenie { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }
        public string Brand { get; set; }

    }

    public class BrdCrmdItem
    {
        public string HrefItem { get; set; }
        public string NameItem { get; set; }
        public int Level { get; set; }
    }

    public class ColorListItem
    {
        public int ProdId { get; set; }
        public string ProdLink { get; set; }
        public string RGB { get; set; }
        public string Img { get; set; }
    }

    public class ProdRGB
    {
        public int ProdID { get; set; }
        public string  RGB { get; set; }
    }
}