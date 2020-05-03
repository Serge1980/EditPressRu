using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class ProductsIndexViewModel
    {
        public List<Products> ListItems { get; set; }
        public PageInfo Pager { get; set; }
    }
}