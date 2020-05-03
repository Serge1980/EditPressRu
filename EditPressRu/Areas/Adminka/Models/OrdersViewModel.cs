using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;

namespace EditPressRu.Areas.Adminka.Models
{
    public class OrdersViewModel
    {
        public PageInfo Pager { get; set; }
        public List<OrderUser> OrderUserList { get; set; }
        //search
        public string  User { get; set; }
        public string  Dat { get; set; }
        public int  Number { get; set; }
        public string  Pr { get; set; }
        //sort
        public string PrSort { get; set; }

    }

    public class OrderUser
    {
        //public Orders Order { get; set; }
        //public UserProfile User { get; set; }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        public string  StatusDescr { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}