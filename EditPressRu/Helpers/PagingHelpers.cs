using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            int preview = pageInfo.page - 1;
            int next = pageInfo.page + 1;

            if (next> pageInfo.TotalPages)
            {
                next = pageInfo.TotalPages;
            }

            if (preview<=0)
            {
                preview = 1;
            }

            StringBuilder result = new StringBuilder();
            result.Append(String.Format(@"<div class='pager'>
    <a href='{0}' class='slPager' onclick=""RenderPagingItems('1'); return false;"">1</a>
    <a href='{1}' class='lArrPager' onclick=""RenderPagingItems('{6}'); return false;""><img src='/Images/lArrowPager.jpg' /></a>
    <span class='mainPager'>{2}</span>
    <a href='{3}' class='rArrPager' onclick=""RenderPagingItems('{7}'); return false;""><img src='/Images/rArrowPager.jpg' /></a>
    <a href='{4}' class='slPager' onclick=""RenderPagingItems('{5}'); return false;"">{5}</a>
<input id='TotalPage' value='{5}' type='hidden'/>
</div>", pageUrl(1),pageUrl(preview),pageInfo.page,pageUrl(next),pageUrl(pageInfo.TotalPages),pageInfo.TotalPages, preview, next));
            
            return MvcHtmlString.Create(result.ToString());
        }
    }
}


//<a href = "em" onclick='func(); return false;'></a> - прекратить обработку дальше  RenderPagingItems   alert('Hello world!')  