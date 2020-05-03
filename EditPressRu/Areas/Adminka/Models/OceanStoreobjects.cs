using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class OceanStoreobjects
    {
        public class Stores
        {
            public List<StoreCount> remains { get; set; }
            public List<StoreCount> reserves { get; set; }
        }

        public class StoreCount
        {
            public string store_code { get; set; }
            public int count { get; set; }
        }



        public class OceanRootObject
        {
            public int product_id { get; set; }
            public string article { get; set; }
            public double price { get; set; }
            public string currency { get; set; }
            public Stores stores { get; set; }
            public List<StoreCount> supplies { get; set; }
        }
    }
}