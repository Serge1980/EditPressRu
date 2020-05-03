using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class AdmOrderDetailsViewModel
    {
        public Orders Order { get; set; }
        public List<OrdersItems> OrdItmList { get; set; }
        public UserProfileDetails Customer { get; set; }
    }

    public class OrdersItems
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public Guid ItemID { get; set; }
        public decimal ItemPrice { get; set; }
        public int Count { get; set; }
        public decimal ItemSumm { get; set; }
        public string ItemName { get; set; }
        
    }
}