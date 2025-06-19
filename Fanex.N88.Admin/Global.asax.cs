namespace Fanex.N88.Admin
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AppStart;
    using Data.Configuration;
    using Fanex.Caching;
    using Fanex.Logging.Sentry.RavenClient;
    using Logging;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;
    using ONEbook.UM;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.NETFramework;
    using ONEbook.UM.Transport;
    using SharpRaven;

    /// <summary>
    ///     MVC application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        /// <summary>
        ///     Application the start.
        /// </summary>
        protected void Application_Start()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            LogManager
                .SetDefaultLogCategory(ConfigurationManager.AppSettings["Fanex.Logging-LogCategory"])
                .Use(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry-Dsn"])
                }));

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DbSettingProviderManager.Current.Start();

            ExceptionHandler.UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            var umConfig = new UMClientConfiguration().Initialize();

            UMClientManager
                .UseConfig(umConfig)
                .UseCacheService(new UMCacheService(new CacheService()))
                .UseRestClient(new UMRestClient(umConfig))
                .UseHttpContext(new HttpContextHelper());

            LogService.Info("Application is running ...");
        }

        /// <summary>
        ///     Application error event handler.
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

        /// <summary>
        ///     Application the stop.
        /// </summary>

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        protected void Application_End()
        {
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
            LogService.Info("Application is stopping ...");
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
