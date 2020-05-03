using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Models
{
    public class AutoComplSearchViewModel
    {
        public List<SelectListItem> ListCat { get; set; }
        public List<SelectListItem> ListTovar { get; set; }
    }

   
}