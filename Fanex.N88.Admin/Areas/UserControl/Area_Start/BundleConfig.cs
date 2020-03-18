namespace Fanex.N88.Admin.Areas.UserControl.AreaStart
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddAndInclude(
               "~/Areas/UserControl/themes/styles",
               "~/Areas/UserControl/themes/bootstrap/css/bootstrap.css",
               "~/Areas/UserControl/themes/reset.css",
                "~/Areas/UserControl/themes/jquery-ui-1.8.23.css",
                "~/Areas/UserControl/scripts/lib/tipsy/tipsy-1.0.0.css",
                "~/Areas/UserControl/scripts/lib/msgbox/msgbox-0.0.1.css");

            bundles.AddAndInclude(
              "~/Areas/UserControl/themes/BasicStyles",
              "~/Areas/UserControl/themes/nap/style.less");

            RegisterN88AdminCssBundles(ref bundles);
            RegisterN88AdminJsBundles(ref bundles);

            bundles.Add(new ScriptBundle("~/Areas/UserControl/jquery").Include(
                "~/Areas/UserControl/scripts/lib/html5shiv.js",
                "~/Areas/UserControl/scripts/lib/jquery-1.8.2.min.js"));

            bundles.Add(new ScriptBundle("~/Areas/UserControl/bootstrap").Include(
                "~/Areas/UserControl/themes/bootstrap/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/Areas/UserControl/js").Include(
                "~/Areas/UserControl/scripts/lib/jquery.easing.1.3.js",
                "~/Areas/UserControl/scripts/lib/jquery-ui-1.8.24.js",
                "~/Areas/UserControl/scripts/lib/jquery.unobtrusive-ajax.js",
                "~/Areas/UserControl/scripts/lib/jquery.validate*",
                "~/Areas/UserControl/scripts/lib/tipsy/tipsy-1.0.0.js",
                "~/Areas/UserControl/scripts/lib/msgbox/msgbox-0.0.1.js",
                "~/Areas/UserControl/scripts/lib/jquery.cookie.js",
                "~/Areas/UserControl/scripts/NAP.js",
                "~/Areas/UserControl/scripts/_Layout.js",
                "~/Areas/UserLogin/Script/ReCheckIn.js",
                "~/Areas/UserControl/scripts/lib/respond.min.js"));
        }

        private static void RegisterN88AdminCssBundles(ref BundleCollection bundles)
        {
            bundles.AddAndInclude(
               "~/Areas/UserControl/user",
               "~/Areas/UserControl/themes/nap/user/manage.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/Site/manage",
                "~/Areas/UserControl/themes/nap/site/manage.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/Site/EditUrl",
                "~/Areas/UserControl/themes/nap/site/edit-url.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/Site/AddNew",
                "~/Areas/UserControl/themes/nap/site/add-new.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/Group/AssignPermission",
                "~/Areas/UserControl/themes/nap/group/assign-permission.less");

            bundles.AddAndInclude(
               "~/Areas/UserControl/themes/AssignSite",
               "~/Areas/UserControl/themes/nap/user/assign-site.less");

            bundles.AddAndInclude("~/Areas/UserControl/themes/AssignSiteFunction",
                "~/Areas/UserControl/scripts/User/assign-site-function/dist/css/main.css",
                "~/Areas/UserControl/themes/nap/user/assign-site.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/EditFunction",
                "~/Areas/UserControl/themes/nap/function/edit.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/EditSite",
                "~/Areas/UserControl/themes/nap/site/edit.less");

            bundles.AddAndInclude(
                 "~/Areas/UserControl/themes/Function/PrivilegedGroupsAndUsers",
                "~/Areas/UserControl/themes/nap/function/PrivilegedGroupsAndUsers.less");

            bundles.AddAndInclude(
                "~/Areas/UserControl/themes/Trace/logexecsp",
                "~/Areas/UserControl/themes/nap/trace/logexecsp.less");
        }

        private static void RegisterN88AdminJsBundles(ref BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Areas/UserControl/js/User/ChangePassword").Include(
                "~/Areas/UserControl/scripts/lib/crypto/CryptoJS.js",
                "~/Areas/UserControl/scripts/User/ChangePassword.js"));

            bundles.Add(new ScriptBundle("~/Areas/UserControl/js/User/ResetPassword").Include(
                "~/Areas/UserControl/scripts/lib/crypto/CryptoJS.js",
                "~/Areas/UserControl/scripts/User/ResetPassword.js"));

            bundles.Add(new ScriptBundle("~/Areas/UserControl/js/User/AssignSiteFunction").Include(
                "~/Areas/UserControl/scripts/User/assign-site-function/dist/main.js"));
        }
    }
}