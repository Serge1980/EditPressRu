using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class OasisStokJson
    {
        //get API Products
        public string id { get; set; }
        public string article { get; set; }
        public string discount_price { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string brand_id { get; set; }
        public int? rating { get; set; }
        public string discount_group_id { get; set; }
        public string old_price { get; set; }

        //get API Stock
        public int? stock { get; set; }
        public int? stock_remote { get; set; }
        public string price { get; set; }

        public int? reserve { get; set; }// - резерв на московском складе
        public int? reserve_remote  { get; set; }//- резерв на удаленном складе
        public int? reserve_transit { get; set; } //- резерв в пути
        public int? reserve_local  { get; set; }//- резерв на локальном складе
    }
}