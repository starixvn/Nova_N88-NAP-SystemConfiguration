namespace Fanex.N88.Admin.Areas.UserLogin
{
    using System.Web.Mvc;
    using System.Web.Optimization;
    using Fanex.N88.Admin.Areas.UserLogin.AreaStart;

    /// <summary>
    /// User login area registration.
    /// </summary>
    public class UserLoginAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserLogin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName + "_DefaultAreas",
                "UserLogin/{controller}/{action}/{id}",
                new { controller = "Login", action = "Index", id = UrlParameter.Optional });

            context.MapRoute(AreaName + "_Themes", "UserLogin/Themes/{resourceName}");
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}