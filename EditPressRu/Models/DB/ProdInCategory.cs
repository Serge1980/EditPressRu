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
    
    public partial class ProdInCategory
    {
        public long Id { get; set; }
        public int ProdId { get; set; }
        public int CatId { get; set; }
    
        public virtual Categories Categories { get; set; }
        public virtual Products Products { get; set; }
    }
}