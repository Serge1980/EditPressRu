using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class IndexProductsViewModel
    {
        public List<IndexEntity> Ruchki { get; set; }
        public List<IndexEntity> Fleshki { get; set; }
        public List<IndexEntity> Umbrellas { get; set; }
        public List<IndexEntity> Tematich { get; set; }
        public List<IndexEntity> Gifts { get; set; }
        public List<IndexEntity> Sladosty { get; set; }
        public List<IndexEntity> Sezonnie { get; set; }
        public List<IndexEntity> Promo { get; set; }
        public List<IndexEntity> Posuda { get; set; }
        public List<IndexEntity> Odegda { get; set; }
        public List<IndexEntity> Kalendar { get; set; }
        public List<IndexEntity> Gadgety { get; set; }
        public List<IndexEntity> Prazdnik { get; set; }
        public List<IndexEntity> Vip { get; set; }
        public List<IndexEntity> Chasy { get; set; }
        public List<IndexEntity> Brelok { get; set; }
        public List<IndexEntity> Egednevnik { get; set; }
        public List<IndexEntity> Games { get; set; }
        public List<IndexEntity> Office { get; set; }
        public List<IndexEntity> Dom { get; set; }
        public List<IndexEntity> Sad { get; set; }
        public List<IndexEntity> Sumky { get; set; }
        public List<IndexEntity> Turizm { get; set; }

        public List<ProdForSlider> ListSaleProducts { get; set; }
        public List<ProdListCarusel> ListProdCarusel { get; set; }
    }

    public class IndexEntity
    {
        public String CpuPath { get; set; }
        public String Name { get; set; }
    }

    public class ProdForSlider
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string ImgPath { get; set; }
        public string SaleSize { get; set; }
        public decimal Price { get; set; }
        public decimal PriceOld { get; set; }
        public string Description { get; set; }
    }

    public class ProdListCarusel
    {
        public int ProdId { get; set; }
        public int ParentId { get; set; }
        public bool Hit { get; set; }
        public bool New { get; set; }
        public bool Sale { get; set; }
        public bool Nalichie { get; set; }
        public string Price { get; set; }
        public decimal OldPrice { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string SaleSize { get; set; }
        public String Link { get; set; }
        public String Img { get; set; }
    }
   
}