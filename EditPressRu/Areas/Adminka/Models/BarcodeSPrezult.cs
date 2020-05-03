using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    public class BarcodeSPrezult
    {
        public long id { get; set; }
        public long shtrih { get; set; }
        public int kontr { get; set; }
        public string ean13 { get; set; }
        public string ean8 { get; set; }
        public string itf14 { get; set; }
        
    }
}