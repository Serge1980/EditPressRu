using System.Web.Optimization;

namespace EditPressRu
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //////////////// Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            //////////////// готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            //////////////bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //////////////            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/scripts/Layout").Include(
                      "~/Scripts/jquery-3.2.1.min.js",
                      "~/Scripts/bxslider/jquery.bxslider.min.js",
                      "~/Scripts/modernizr-2.6.2.js"));

            bundles.Add(new StyleBundle("~/Layout_osn/css").Include(
                      "~/Content/Site.css",
                      "~/Content/jquery.bxslider.css",
                      "~/Content/PagerCss.css",
                      "~/Content/SliderProduct.css"));

            bundles.Add(new StyleBundle("~/Layout_dop/css").Include(
                      "~/Content/jquery.range.css",
                      "~/Content/YaxMap.css",
                      "~/Content/table-styles.css"
                      ));

            bundles.Add(new StyleBundle("~/CustomSliders/css").Include(
                     "~/Content/customSlider1.css",
                     "~/Content/customSlider2.css"
                     ));



            BundleTable.EnableOptimizations = true;
            
        }
    }
}


    
   
    //<script src = "~/scripts/jquery-3.2.1.min.js" ></ script >
    //< script src="~/scripts/bxslider/jquery.bxslider.min.js"></script>
    //<script src = "~/scripts/modernizr-2.6.2.js" ></ script >