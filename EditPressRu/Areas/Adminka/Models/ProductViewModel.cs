using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        //public Attributes Atribut { get; set; }
        public List<ProdInCategory> ProdCat { get; set; }
    }
}