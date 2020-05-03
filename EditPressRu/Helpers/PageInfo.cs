using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Helpers
{
    public class PageInfo
    {
        public int page { get; set; } // номер текущей страницы
        public int PageSize { get; set; } // кол-во объектов на странице
        public int TotalItems { get; set; } // всего объектов
        public int TotalPages  // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }

    }
}