using System.Web.Optimization;

namespace OfficeSpace
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/slickgrid-simple")
                .Include(
                    "~/Scripts/jquery.event.drag.js",
                    "~/Vendors/SlickGrid/slick.core.js",
                    "~/Vendors/SlickGrid/slick.grid.js"));

            bundles.Add(new ScriptBundle("~/bundles/slickgrid-basic")
                .Include(
                    "~/Scripts/jquery.event.drag.js",
                    "~/Vendors/SlickGrid/slick.core.js",
                    "~/Vendors/SlickGrid/slick.grid.js",
                    "~/Vendors/SlickGrid/slick.dataview.js",
                    "~/Vendors/SlickGrid/slick.editor.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css/slickgrid")
                .Include(
                    "~/Vendors/SlickGrid/slick.grid.css",
                    "~/Vendors/SlickGrid/slick-default-theme.css",
                    "~/Vendors/SlickGrid/controls/slick.columnpicker.css",
                    "~/Vendors/SlickGrid/controls/slick.pager.css"));
        }
    }
}