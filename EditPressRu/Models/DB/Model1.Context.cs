﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EditPressRu.Models.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EditPressRuEntities : DbContext
    {
        public EditPressRuEntities()
            : base("name=EditPressRuEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ArtiklesIndexes> ArtiklesIndexes { get; set; }
        public virtual DbSet<Brands> Brands { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Cityes> Cityes { get; set; }
        public virtual DbSet<Colors_spr> Colors_spr { get; set; }
        public virtual DbSet<Discounts> Discounts { get; set; }
        public virtual DbSet<EbazarBase_Categories> EbazarBase_Categories { get; set; }
        public virtual DbSet<EbazarBase_CatIntercept> EbazarBase_CatIntercept { get; set; }
        public virtual DbSet<EbazarBase_ImagesAdvanced> EbazarBase_ImagesAdvanced { get; set; }
        public virtual DbSet<EbazarBase_Products> EbazarBase_Products { get; set; }
        public virtual DbSet<EklekticBase_Categories> EklekticBase_Categories { get; set; }
        public virtual DbSet<EklekticBase_CatIntercept> EklekticBase_CatIntercept { get; set; }
        public virtual DbSet<EklekticBase_ProdCat> EklekticBase_ProdCat { get; set; }
        public virtual DbSet<EklekticBase_Products> EklekticBase_Products { get; set; }
        public virtual DbSet<GiftBase_Attachments> GiftBase_Attachments { get; set; }
        public virtual DbSet<GiftBase_Catalog> GiftBase_Catalog { get; set; }
        public virtual DbSet<GiftBase_GiftEditCatIntersept> GiftBase_GiftEditCatIntersept { get; set; }
        public virtual DbSet<GiftBase_Prints> GiftBase_Prints { get; set; }
        public virtual DbSet<GiftBase_Prod_Cat> GiftBase_Prod_Cat { get; set; }
        public virtual DbSet<GiftBase_Products> GiftBase_Products { get; set; }
        public virtual DbSet<GiftBase_SubProducts> GiftBase_SubProducts { get; set; }
        public virtual DbSet<GroupColorProducts> GroupColorProducts { get; set; }
        public virtual DbSet<GroupSizeProducts> GroupSizeProducts { get; set; }
        public virtual DbSet<KP_Store> KP_Store { get; set; }
        public virtual DbSet<Kp_Store_dop> Kp_Store_dop { get; set; }
        public virtual DbSet<Makety> Makety { get; set; }
        public virtual DbSet<Materials_spr> Materials_spr { get; set; }
        public virtual DbSet<Nanesenie_spr> Nanesenie_spr { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<ProdImages> ProdImages { get; set; }
        public virtual DbSet<ProdInCategory> ProdInCategory { get; set; }
        public virtual DbSet<Prods_Colors> Prods_Colors { get; set; }
        public virtual DbSet<Prods_Materials> Prods_Materials { get; set; }
        public virtual DbSet<Prods_Nanesenie> Prods_Nanesenie { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProdVideos> ProdVideos { get; set; }
        public virtual DbSet<Redirect> Redirect { get; set; }
        public virtual DbSet<ShluseForVendors> ShluseForVendors { get; set; }
        public virtual DbSet<StatusPlace_spr> StatusPlace_spr { get; set; }
        public virtual DbSet<subcription> subcription { get; set; }
        public virtual DbSet<ToEbazar> ToEbazar { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserProfileDetails> UserProfileDetails { get; set; }
        public virtual DbSet<Vendors> Vendors { get; set; }
        public virtual DbSet<XinDao_Categories> XinDao_Categories { get; set; }
        public virtual DbSet<XinDao_Foto> XinDao_Foto { get; set; }
        public virtual DbSet<XinDao_Nanesenie> XinDao_Nanesenie { get; set; }
        public virtual DbSet<XinDao_ProdInCat> XinDao_ProdInCat { get; set; }
        public virtual DbSet<XinDao_Products> XinDao_Products { get; set; }
        public virtual DbSet<Oasis_Categories> Oasis_Categories { get; set; }
        public virtual DbSet<Oasis_Materials> Oasis_Materials { get; set; }
        public virtual DbSet<Oasis_ProdColors> Oasis_ProdColors { get; set; }
        public virtual DbSet<Oasis_ProdImages> Oasis_ProdImages { get; set; }
        public virtual DbSet<Oasis_Products> Oasis_Products { get; set; }
        public virtual DbSet<Oasis_ProdInCategory> Oasis_ProdInCategory { get; set; }
        public virtual DbSet<Oasis_ProdInCategoryFull> Oasis_ProdInCategoryFull { get; set; }
        public virtual DbSet<Oasis_Packages> Oasis_Packages { get; set; }
        public virtual DbSet<OrdersStatus_spr> OrdersStatus_spr { get; set; }
        public virtual DbSet<CPrestige_Categories> CPrestige_Categories { get; set; }
        public virtual DbSet<CPrestige_CatProd> CPrestige_CatProd { get; set; }
        public virtual DbSet<CPrestige_Images> CPrestige_Images { get; set; }
        public virtual DbSet<CPrestige_Makety> CPrestige_Makety { get; set; }
        public virtual DbSet<CPrestige_Products> CPrestige_Products { get; set; }
    }
}
