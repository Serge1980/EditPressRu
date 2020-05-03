using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System.Web.Optimization;
using log4net;
using EditPressRu.Controllers;
using static EditPressRu.Areas.Adminka.Controllers.OasisAdminController;
using static EditPressRu.Areas.Adminka.Controllers.GiftAdminController;
using static EditPressRu.Areas.Adminka.Controllers.OceanAdminController;
using static EditPressRu.Areas.Adminka.Controllers.HGadminController;
using static EditPressRu.Areas.Adminka.Controllers.EbazarController;
using static EditPressRu.Areas.Adminka.Controllers.EklekticController;
using static EditPressRu.Areas.Adminka.Controllers.XindaoController;

namespace EditPressRu
{
    public class Global : HttpApplication
    {
        //public object FilterConfig { get; private set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            // GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //loggers
            log4net.Config.XmlConfigurator.Configure();

            // запуск выполнения работы
            OasisScheduler.Start();
            GiftScheduler.Start();
            OceanScheduler.Start();
            HGiftScheduler.Start();
            EbazarScheduler.Start();
            EklekticScheduler.Start();
            XindaoScheduler.Start();
        }



        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 1;
        }



        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{

        //    Здесь альтернатива GZip против включения в настройках IIS
        //HttpContext context = HttpContext.Current;
        //    context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
        //    HttpContext.Current.Response.AppendHeader("Content-encoding", "gzip");
        //    HttpContext.Current.Response.Cache.VaryByHeaders["Accept-encoding"] = true;

        //    Здесь альтернатива редиректа против urlRewriter модуля и web.config
        //    //if (!Request.IsLocal && !Request.Url.IsLoopback)
        //    //{
        //    //    switch (Request.Url.Scheme)
        //    //    {
        //    //        case "https":
        //    //            Response.AddHeader("Strict-Transport-Security", "max-age=300");
        //    //            break;
        //    //        case "http":
        //    //            var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
        //    //            Response.Status = "301 Moved Permanently";
        //    //            Response.AddHeader("Location", path);
        //    //            break;
        //    //    }
        //    //}

        //    if (Request.Url.Host.StartsWith("www.") && !Request.Url.IsLoopback)
        //    {
        //        UriBuilder builder = new UriBuilder(Request.Url);
        //        builder.Host = Request.Url.Host.Replace("www.", "");
        //        Response.StatusCode = 301;
        //        Response.AddHeader("Location", builder.ToString());
        //        Response.End();
        //    }


        //}

        //обработка 404 
        protected void Application_Error()
        {
            ILog log = log4net.LogManager.GetLogger(typeof(Global));

            Exception exception = Server.GetLastError();
            log.Error(null, exception);

            if (exception is HttpException)
            {
                HttpException ex = exception as HttpException;
                if (ex.GetHttpCode() == 404)
                {


                    var url = HttpContext.Current.Request.RawUrl;
                    string newLink = "/errorpage/error/404";
                    var redirect = FlatFileAccess.PreparedKeyParLink();
                    Redirect linkPayr = redirect.FirstOrDefault(x => x.OldIink.ToLower().Trim() == url.ToLower().Trim());
                    if (linkPayr != null) //нашел пару
                    {
                        // Clear the error
                        Response.Clear();
                        Server.ClearError();
                        // 301 it to the new url
                        newLink = linkPayr.NewLink;
                        Response.RedirectPermanent(newLink);
                    }

                    else if (url.ToLower().Contains("/products/allproducts"))
                    {
                        // Clear the error
                        Response.Clear();
                        Server.ClearError();
                        newLink = url.ToLower().Replace("/products/allproducts", "/products").Replace("pagenumber", "page");
                        Response.RedirectPermanent(newLink);
                    }
                    else if (url.ToLower().Contains("/products/indexproducts"))
                    {
                        // Clear the error
                        Response.Clear();
                        Server.ClearError();
                        newLink = url.ToLower().Replace("/products/indexproducts", "/products");
                        Response.RedirectPermanent(newLink);
                    }
                    else if (url.ToLower().Contains("/products/") && url.ToLower().Contains("?page="))
                    {
                        // Clear the error

                        newLink = url.ToLower().Split('?')[0];
                        Redirect links = redirect.FirstOrDefault(x => x.OldIink.ToLower().Trim() == newLink.Trim());
                        if (links != null) //нашел пару
                        {
                            // Clear the error
                            Response.Clear();
                            Server.ClearError();
                            // 301 it to the new url
                            newLink = links.NewLink;
                            Response.RedirectPermanent(newLink);
                            return;
                        }
                        else
                        {

                            Server.ClearError();

                            var routeData = new RouteData();
                            routeData.Values.Add("controller", "ErrorPage");
                            routeData.Values.Add("action", "Error");
                            routeData.Values.Add("exception", exception);
                            routeData.Values.Add("statusCode", 404);
                            Response.TrySkipIisCustomErrors = true;
                            IController controller = new ErrorPageController();
                            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                            Response.End();
                            return;
                        }

                    }
                    else
                    {
                        //////Server.ClearError();
                        //////Response.Clear();
                        //////Response.Status = "404 Not Found";
                        //////Response.StatusCode = 404;

                        //////newLink = "/errorpage/error/404";

                        //////Response.Redirect(newLink);
                        /////////////Server.Transfer("~/views/shared/Error404.html");
                        /////////////Server.Transfer("~/404.html");

                        Server.ClearError();

                        var routeData = new RouteData();
                        routeData.Values.Add("controller", "ErrorPage");
                        routeData.Values.Add("action", "Error");
                        routeData.Values.Add("exception", exception);
                        routeData.Values.Add("statusCode", 404);
                        Response.TrySkipIisCustomErrors = true;
                        IController controller = new ErrorPageController();
                        controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                        Response.End();

                        return;
                    }
                }


                else
                {
                    //////////// Clear the error
                    ////////////Response.Clear();
                    ////////////Server.ClearError();
                    ////////////Response.StatusCode = 404;
                    ////////////Response.Redirect("/errorpage/error/404");

                    Server.ClearError();

                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "ErrorPage");
                    routeData.Values.Add("action", "Error");
                    routeData.Values.Add("exception", exception);
                    routeData.Values.Add("statusCode", 404);
                    Response.TrySkipIisCustomErrors = true;
                    IController controller = new ErrorPageController();
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();
                }

            }
            else
            {
                Server.ClearError();

                var routeData = new RouteData();
                routeData.Values.Add("controller", "ErrorPage");
                routeData.Values.Add("action", "Error");
                routeData.Values.Add("exception", exception);
                routeData.Values.Add("statusCode", 404);
                Response.TrySkipIisCustomErrors = true;
                IController controller = new ErrorPageController();
                controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                Response.End();
            }
        }

    }
}