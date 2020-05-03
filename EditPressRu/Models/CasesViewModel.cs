using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class CasesViewModel
    {
        public List<string> ListFiles { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
    }
}