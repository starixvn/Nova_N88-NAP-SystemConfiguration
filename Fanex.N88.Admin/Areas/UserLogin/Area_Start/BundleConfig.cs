namespace Fanex.N88.Admin.Areas.UserLogin.AreaStart
{
    using System.Web.Optimization;

    /// <summary>
    /// Bundle config.
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Register bundle.
        /// </summary>
        /// <param name="bundles">Bundle collection.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/Areas/UserLogin/themes/login",
                "~/Areas/UserLogin/themes/login.less");

            bundles.Add(new ScriptBundle("~/Areas/UserLogin/js/login").Include(
                "~/Areas/UserLogin/scripts/lib/crypto/CryptoJS.js",
                "~/Areas/UserLogin/scripts/Login.js"));

            bundles.AddAndInclude(
               "~/Areas/UserLogin/themes/site",
               "~/Areas/UserLogin/themes/select-site.less",
               "~/Areas/UserLogin/scripts/lib/msgbox/msgbox-0.0.1.css");

            bundles.Add(new ScriptBundle("~/Areas/UserLogin/js/site").Include(
                "~/Areas/UserLogin/scripts/lib/jquery-1.8.0.min.js",
                "~/Areas/UserLogin/scripts/lib/msgbox/msgbox-0.0.1.js"));
        }
    }
}