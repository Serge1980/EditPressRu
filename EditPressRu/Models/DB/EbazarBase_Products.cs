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
    
    public partial class EbazarBase_Products
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string articul { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<int> quantity_free { get; set; }
        public Nullable<int> category_id { get; set; }
        public string brand { get; set; }
        public string collection { get; set; }
        public string description { get; set; }
        public string manufacturer { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string small_image { get; set; }
        public string middle_image { get; set; }
        public string big_image { get; set; }
    }
}
