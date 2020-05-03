using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class PagerResult<T>
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
        public int minPrice { get; set; }
        public int maxPrice { get; set; }
        public List<int> ListProdId { get; set; }
        public String QueryString { get; set; }
    }
}