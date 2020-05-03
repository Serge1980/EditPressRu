using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class UserOrdersViewModel
    {
        public string UserName { get; set; }
        public OrderDetailsView ListOrders { get; set; }
       
    }
}