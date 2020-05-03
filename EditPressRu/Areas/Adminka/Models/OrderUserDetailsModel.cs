using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class OrderUserDetailsModel
    {
        public UserProfile OrderUser { get; set; }
        public UserProfileDetails UserInfo { get; set; }
        public decimal? TotalSumm { get; set; }
        public int TotalCount { get; set; }
    }
}