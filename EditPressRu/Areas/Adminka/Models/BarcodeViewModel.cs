using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Areas.Adminka.Models
{
    public class BarcodeViewModel
    {
        public long NumberCode { get; set; }
        public int CountCode { get; set; }
        public string PathToFile { get; set; }
        public List<BarcodeSPrezult> SpListRez { get; set; }
        public List<SelectListItem> BarcodeType { get; set; }
        public int SelectedType { get; set; }
    }
}