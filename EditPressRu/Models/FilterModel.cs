using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Models
{
    public class FilterModel
    {
        public List<SelectListItem> Materials { get; set; }
        public List<SelectListItem> Nanesenie { get; set; }
        public List<SelectListItem> Brands { get; set; }
        public List<SelectListItem> ColorsAll { get; set; }
        public List<SelectListItem> ColorsSelected { get; set; }
    }
}