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
    
    public partial class GroupColorProducts
    {
        public int ProdId { get; set; }
        public int ParentId { get; set; }
        public bool Main { get; set; }
        public string GroupArticle { get; set; }
        public string Name { get; set; }
    
        public virtual Products Products { get; set; }
    }
}
