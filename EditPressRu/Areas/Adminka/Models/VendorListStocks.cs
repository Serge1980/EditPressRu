using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class VendorListStocks
    {
        public int ProdId { get; set; }
        public Products Product { get; set; }
        public String ImgPth { get; set; }
        public List<string> PdfFiles { get; set; }
        public List<string> CdrFiles { get; set; }
    }
}