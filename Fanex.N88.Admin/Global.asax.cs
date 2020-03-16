namespace Fanex.N88.Admin
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AppStart;
    using Data.Configuration;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;

    /// <summary>
    /// MVC application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        /// <summary>
        /// Application the start.
        /// </summary>
        protected void Application_Start()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DbSettingProviderManager.Current.Start();

            ExceptionHandler.UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);
        }

        /// <summary>
        /// Application error event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Logging
            if (!Request.IsLocal)
            {
                ExceptionHandler.Handle(Server.GetLastError());
            }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        protected void Session_End(object sender, EventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            AuthConfig.SignOut();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }
    }
}