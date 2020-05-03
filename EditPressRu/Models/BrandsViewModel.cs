using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class BrandsViewModel
    {
        public PagerResult<BrandCat> ProdForPaging { get; set; }
        public PageInfo PageInfo { get; set; }
        public String Description { get; set; }

    }

    public class BrandCat
    {
        public Brands Brand { get; set; }
        public string TovarsHref { get; set; }
    }
}