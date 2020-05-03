using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class OrdersNewViewModel
    {
        public Orders OrdersCart { get; set; }
        public Orders OrdersProt { get; set; }
        public Orders OrdersMaket { get; set; }
    }
}