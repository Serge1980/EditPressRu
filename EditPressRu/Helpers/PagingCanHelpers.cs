using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Helpers
{
    public static class PagingCanHelpers
    {
        public static MvcHtmlString PageLinks2(this HtmlHelper html,
            PageInfo pageInfo,string url0, Func<int, string> pageUrl)
        {
            int preview = pageInfo.page - 1;
            int next = pageInfo.page + 1;
            string url1 = pageUrl(preview);

            if (next> pageInfo.TotalPages)
            {
                next = pageInfo.TotalPages;
            }

            if (preview<=0)
            {
                preview = 1;
            }

            if (preview == 1)
            {
                url1 = url0;
            }

            StringBuilder result = new StringBuilder();
            result.Append(String.Format(@"<div class='pager'>
    <a href='{0}' class='slPager'>1</a>
    <a href='{1}' class='lArrPager'><img src='/Images/lArrowPager.jpg' /></a>
    <span class='mainPager'>{2}</span>
    <a href='{3}' class='rArrPager'><img src='/Images/rArrowPager.jpg' /></a>
    <a href='{4}' class='slPager'>{5}</a>
<input id='TotalPage' value='{5}' type='hidden'/>
</div>", url0, url1, pageInfo.page,pageUrl(next),pageUrl(pageInfo.TotalPages),pageInfo.TotalPages, preview, next));
            
            return MvcHtmlString.Create(result.ToString());
        }
    }
}


//<a href = "em" onclick='func(); return false;'></a> - прекратить обработку дальше  RenderPagingItems   alert('Hello world!')  