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
    
    public partial class GiftBase_SubProducts
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int main_product { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string size_code { get; set; }
        public string price_price { get; set; }
        public string price_currency { get; set; }
        public string price_name { get; set; }
    }
}
