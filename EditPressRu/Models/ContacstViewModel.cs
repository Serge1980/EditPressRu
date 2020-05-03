using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class ContacstViewModel
    {
        public List<Products> TopSalesList { get; set; }
        public String FIO  { get; set; }
        public String Email { get; set; }
        public String Body{ get; set; }
    }
}