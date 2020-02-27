namespace Fanex.NAP.Admin.Areas.UserControl.AreaStart
{
    using System.Web.Mvc;

    /// <summary>
    /// Route config.
    /// </summary>
    public static class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            context.MapRoute(
                areaName + "_Default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}