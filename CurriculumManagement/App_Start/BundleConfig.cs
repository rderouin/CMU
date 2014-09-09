using System.Web;
using System.Web.Optimization;

namespace CurriculumManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery.ui.core.js",
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jquery.ui.datepicker.js",
                        "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive-ajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    /*styles confict with UBC CLF, included from MVC
                     * "~/Content/bootstrap.css",
                    "~/Content/cmu-custom.css",
                    "~/Content/site.css"));*/
                    "~/Content/unit.css",
                    "~/Content/jquery-ui.css"));


            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                      "~/Content/jquery.ui.all.css",
                      "~/Content/jquery-ui.css"));

            bundles.IgnoreList.Clear();
            bundles.DirectoryFilter.Clear();
            bundles.IgnoreList.Ignore("bootstrap.min.css", OptimizationMode.Always);
        }
    }
}
