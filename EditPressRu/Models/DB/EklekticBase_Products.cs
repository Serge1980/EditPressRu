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
    
    public partial class EklekticBase_Products
    {
        public System.Guid Id { get; set; }
        public string Artikul { get; set; }
        public string Name { get; set; }
        public Nullable<int> Stock { get; set; }
        public Nullable<int> Stock_Remote { get; set; }
        public Nullable<int> Reserv_transit { get; set; }
        public Nullable<int> Reserv_Remote { get; set; }
        public Nullable<int> TovarEklektic { get; set; }
        public Nullable<System.Guid> AnalogKode { get; set; }
        public Nullable<System.Guid> ColorGroupCode { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public Nullable<int> BazEdCod { get; set; }
        public string BazEdName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Price_opt { get; set; }
        public string FullName { get; set; }
        public string Material { get; set; }
        public string Nanesenie { get; set; }
        public string Preservativ { get; set; }
        public string Descript { get; set; }
        public string Size { get; set; }
        public Nullable<bool> PrPr { get; set; }
        public string Weight { get; set; }
        public string Otrasl { get; set; }
    }
}