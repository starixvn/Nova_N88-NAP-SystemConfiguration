namespace Fanex.N88.Admin.Areas.UserControl
{
    using System.Web.Mvc;
    using System.Web.Optimization;
    using Fanex.N88.Admin.Areas.UserControl.AreaStart;

    /// <summary>
    /// User control area registration.
    /// </summary>
    public class UserControlAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(AreaName, context);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}