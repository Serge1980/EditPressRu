using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Models
{
    public class EditKPViewModel
    {
        public UserProfileDetails UserInfo { get; set; }
        public KP_Store KpStore { get; set; }
        public List<Kp_Store_dop> KpStoreDopList { get; set; }
        public List<OrderProduct> ProdList { get; set; }
        //public List<ImageListItem> ImagesList { get; set; }
        public decimal Cost { get; set; }
        public decimal CostSale { get; set; }
        public decimal CostMark { get; set; }
        public decimal TotalSumm { get; set; }
        public string ImageLogo { get; set; }
        public string DateKp { get; set; }

        //public FileModel files { get; set; }
    }
   
    public class KpToSaveViewModel
    {
        public int KpId { get; set; }

        public string FoneColor { get; set; }

        public string Logo { get; set; }

        public string KpTitle { get; set; }
        [AllowHtml]
        public string KpContacts { get; set; }
        [AllowHtml]
        public string KpDescript { get; set; }

        public int ProdId { get; set; }
        public decimal Price { get; set; }
        public int Cnt { get; set; }

        // показывать-скрыть блоки(разделы) КП
        public bool ArticleP { get; set; }
        public bool UserP { get; set; }
        public bool CharacterP { get; set; }
        public bool DescrP { get; set; }
        public bool SumTirageP { get; set; }
        public bool TotalP { get; set; }

        //Всплыающееокно дополнительные сервисы
        public string Service { get; set; }
        public decimal PriceService { get; set; }
        public int Sale { get; set; }

        public List<SelectListItem> PriceArr { get; set; }
        public List<SelectListItem> CntArr { get; set; }

        [AllowHtml]
        public List<SelectListItem> DescrArr { get; set; }

        //приходит номер картинки и id из бызы
        //public int NbrImgP { get; set; }
        public string ImgBtn { get; set; }
        public int ImgProdId { get; set; }
        public long IdImg { get; set; }

    }

    public class Material
    {
        public string Name { get; set; }
    }

    public class Nanesenie
    {
        public string Name { get; set; }
    }

    public class DopImgList
    {
        public Int64 ImgId { get; set; }
        public string Image { get; set; }
        public int ProdId { get; set; }
        public bool Selected { get; set; }
        public int NumberImg { get; set; }
    }

    public class OrderProduct
    {
        public int ProdId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Article { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public int OrderID { get; set; }
        public int Cnt { get; set; }
        public double ItemSumm { get; set; }
        public List<Material> Material { get; set; }
        public List<Nanesenie> Nanesenie { get; set; }
        public string Image { get; set; }
        public List<DopImgList> DopImgList { get; set; }
    }

    public class PicterPartialModel
    {
        public int Numbr { get; set; }
        public bool? Next { get; set; }
        public List<DopImgList> DopImgList { get; set; }
        public int ProdId { get; set; }
        public int KpId { get; set; }
    }

    //public class OrderProduct
    //{
    //    public int ProdId { get; set; }
    //    public string Name { get; set; }
    //    public decimal Price { get; set; }
    //    public int Cnt { get; set; }
    //    public decimal ItemSumm { get; set; }
    //    public string Article { get; set; }
    //    public string Size { get; set; }
    //    public string Material { get; set; }
    //    public string Weight { get; set; }
    //    public string Nanesenie { get; set; }
    //    public string Description { get; set; }
    //    public int OrderId { get; set; }
    //    public string Image { get; set; }
    //    public List<ImageListItem> ListDopImg { get; set; }
    //}

    //public class ImageListItem
    //{
    //    public Int64 ImgId { get; set; }
    //    public String Image { get; set; }
    //    public int ProdId { get; set; }
    //    public bool Selected { get; set; }
    //    public int NumberImg { get; set; }
    //}
}