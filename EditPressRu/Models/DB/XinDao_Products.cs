//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class XinDao_Products
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string GroupArticle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Material { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Image { get; set; }
        public string Pres_Size { get; set; }
        public string Pres_Count { get; set; }
        public string Pres_Weight { get; set; }
        public bool New { get; set; }
        public bool Eko { get; set; }
    }
}