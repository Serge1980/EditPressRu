using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Helpers
{
    public static class PagingProdHelpers
    {
        public static MvcHtmlString PageLinks1(this HtmlHelper html,
            PageInfo pageInfo, string pageUrl)
        {
            int preview = pageInfo.page - 1;
            int next = pageInfo.page + 1;
            string urlPreview = pageUrl; 

            if (next> pageInfo.TotalPages)
            {
                next = pageInfo.TotalPages;
            }

            if (preview<=0)
            {
                preview = 1;
                urlPreview = pageUrl; 

            }
            else
            {
                if (preview == 1)
                {
                    urlPreview = pageUrl;
                }
                else
                {
                    urlPreview = String.Format("{0}/{1}", pageUrl, preview);
                }
                
            }

            StringBuilder result = new StringBuilder();
            result.Append(String.Format(@"<div class='pager'>
    <a href='{0}' class='slPager' onclick=""RenderPagingItems('1'); return false;"">1</a>
    <a href='{1}' class='lArrPager' onclick=""RenderPagingItems('{6}'); return false;""><img src='/Images/lArrowPager.jpg' /></a>
    <span class='mainPager'>{2}</span>
    <a href='{3}' class='rArrPager' onclick=""RenderPagingItems('{7}'); return false;""><img src='/Images/rArrowPager.jpg' /></a>
    <a href='{4}' class='slPager' onclick=""RenderPagingItems('{5}'); return false;"">{5}</a>
<input id='TotalPage' value='{5}' type='hidden'/>
</div>", pageUrl, urlPreview, pageInfo.page, String.Format("{0}/{1}", pageUrl, next)
, String.Format("{0}/{1}", pageUrl, pageInfo.TotalPages),pageInfo.TotalPages, preview, next));
            
            return MvcHtmlString.Create(result.ToString());
        }
    }
}


//<a href = "em" onclick='func(); return false;'></a> - прекратить обработку дальше  RenderPagingItems   alert('Hello world!')  