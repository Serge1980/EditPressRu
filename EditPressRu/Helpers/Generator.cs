using System.Linq;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Linq;

namespace EditPressRu.Helpers
{
    public class Generator
    {
        EditPressRuEntities dBase;
        public Generator()
        {
            dBase = new EditPressRuEntities();
        }
        public IReadOnlyCollection<string> GetSitemapNodes1(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();

            nodes.Add("https://editpress.ru");
            nodes.Add("https://editpress.ru/cases");
            nodes.Add("https://editpress.ru/aboutcompany");
            nodes.Add("https://editpress.ru/proizvodstvo");
            nodes.Add("https://editpress.ru/deliveryterms");
            nodes.Add("https://editpress.ru/uslugi");
            nodes.Add("https://editpress.ru/contacts");
            nodes.Add("https://editpress.ru/politikakonfidencialnosty");
            nodes.Add("https://editpress.ru/bonus");
            nodes.Add("https://editpress.ru/brands");
            nodes.Add("https://editpress.ru/articles");

            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "logoapplication" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "digitprint" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "serigraphy" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "flex" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "fancywork" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "padprinting" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "ufprinting" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "decal" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "laserenrgaving" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "stamping" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "sublimationonmugs" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "volumeLabel" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "uslugi", action = "consult" }));

            foreach (string cpu in dBase.Categories.Where(x => x.Publish).Where(x=>x.Id!=304).Select(x => x.CpuPath).ToList())
            {
                nodes.Add(String.Format("https://editpress.ru/products/{0}", cpu));
            }
            return nodes;
        }

        public IReadOnlyCollection<string> GetSitemapNodes2(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id < 10000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }


        public IReadOnlyCollection<string> GetSitemapNodes3(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 10000 && x.Id<20000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }

        public IReadOnlyCollection<string> GetSitemapNodes4(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 20000 && x.Id < 30000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }

        public IReadOnlyCollection<string> GetSitemapNodes5(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 30000 && x.Id < 40000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }

        public IReadOnlyCollection<string> GetSitemapNodes6(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 40000 && x.Id < 50000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }


        public IReadOnlyCollection<string> GetSitemapNodes7(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 50000 && x.Id < 80000).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }

        public IReadOnlyCollection<string> GetSitemapNodes8(UrlHelper urlHelper)
        {
            List<string> nodes = new List<string>();
            foreach (int productId in dBase.Products.Where(x => x.Publish && x.Id >= 80000 ).Select(x => x.Id).ToList())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "products", action = "product", id = productId }));
            }

            return nodes;
        }

        public string GetSitemapDocument(IEnumerable<string> sitemapNodes)
        {
           
           
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (string sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(xmlns + "url",
                     new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Replace("http://editpress.loc", "https://editpress.ru"))

                   )
                    
                 );

               
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        //public string GetSitemapDocument(IEnumerable<string> sitemapNodes)
        //{


        //    XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        //    XElement root = new XElement(xmlns + "urlset");

        //    foreach (string sitemapNode in sitemapNodes)
        //    {
        //        string priority = "0.5";
        //        string caseVar = "";
        //        try
        //        {
        //            caseVar = sitemapNode.Split('/')[4];
        //        }
        //        catch (Exception)
        //        {


        //        }

        //        switch (caseVar)
        //        {
        //            case "":
        //                priority = "0.8";
        //                break;
        //            case "IndexProducts":
        //                priority = "0.9";
        //                break;
        //            case "Product":
        //                priority = "0.8";
        //                break;
        //            case "Uslugy":
        //                priority = "0.7";
        //                break;
        //            default:
        //                priority = "0.5";
        //                break;
        //        }

        //        XElement urlElement = new XElement(xmlns + "url",
        //             new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Replace("localhost:52014", "editpress.ru"))

        //           )
        //            ,
        //             new XElement(xmlns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
        //             new XElement(xmlns + "changefreq", "monthly"),
        //             new XElement(xmlns + "priority", priority)
        //         );


        //        root.Add(urlElement);
        //    }

        //    XDocument document = new XDocument(root);
        //    return document.ToString();
        //}

    }


    public static class UrlHelperExtensions
    {
        public static string AbsoluteRouteUrl(this UrlHelper urlHelper,
            string routeName, object routeValues = null)
        {
            string scheme = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }
    }
}