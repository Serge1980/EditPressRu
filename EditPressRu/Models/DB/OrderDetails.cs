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
    
    public partial class OrderDetails
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductId { get; set; }
        public decimal ItemPrice { get; set; }
        public int Count { get; set; }
        public string Descripts { get; set; }
        public string Img { get; set; }
    
        public virtual Orders Orders { get; set; }
    }
}
