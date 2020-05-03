using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class CatWithSubcat
    {
        public Categories Category { get; set; }
        public List<Categories> ListSubcat { get; set; }
    }
}