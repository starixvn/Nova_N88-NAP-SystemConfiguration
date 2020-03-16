namespace Fanex.N88.Admin.AppStart
{
    using System.Web;
    using System.Web.Security;
    using Fanex.N88.Core.Criteria.User;
    using Fanex.N88.Core.Service.Impl;
    using Fanex.N88.Core.Web.Auth;
    using Fanex.N88.Core.Web.Session;

    /// <summary>
    /// Authorize config.
    /// </summary>
    public static class AuthConfig
    {
        /// <summary>
        /// Register authorize data.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        public static void RegisterAuth(HttpContext context)
        {
            var user = HttpContext.Current.User;

            if (user != null && user.Identity is FormsIdentity && user.Identity.IsAuthenticated)
            {
                HttpCookie httpCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (httpCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(httpCookie.Value);

                    if (ticket == null || ticket.Expired || string.IsNullOrEmpty(ticket.UserData))
                    {
                        return;
                    }

                    var identity = new FormsIdentity(ticket);
                    AuthInfo udata = AuthInfo.CreateUserData(ticket.UserData);

                    context.User = new N88Principal(identity, udata);
                }
            }
        }

        /// <summary>
        /// User sign out.
        /// </summary>
        public static void SignOut()
        {
            var _currentSession = new CurrentSession();
            var _userService = new UserService();
            var logOutUserCriteria = new LogOutUserCriteria();

            logOutUserCriteria.OnUserId = _currentSession.OnUserId;
            _userService.UserLogOut(logOutUserCriteria);
            _currentSession.ClearAll();
        }
    }
}