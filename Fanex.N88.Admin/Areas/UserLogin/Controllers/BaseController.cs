namespace Fanex.N88.Admin.Areas.UserLogin.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using N88.Core.Configuration;
    using N88.Core.Model;
    using N88.Core.Web;
    using N88.Core.Web.Auth;
    using N88.Core.Web.Context;
    using N88.Core.Web.Session;

    public class BaseController : Controller
    {
        public static bool EnableDirectLogin => N88Settings.Settings.EnableDirectLogin;

        protected ViewFlags ViewFlags => new ViewFlags
        {
            IsReCheckin = N88Settings.Settings.EnableReCheckIn
        };

        protected static int SiteId => N88Settings.Settings.SiteId;

        protected static string Environment => N88Settings.Settings.Environment;

        protected static ICurrentContext CurrentContext => new CurrentContext();

        protected ICurrentSession CurrentSession { get; set; }

        protected void CreateAuthInfo(User user, int onlineUserId)
        {
            string currentDomainPath = CurrentContext.CurrentDomain.Path;

            var authInfo = new AuthInfo
            {
                UserId = user.Id,
                OnUserId = onlineUserId
            };

            var authTicket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomainPath);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = currentDomainPath,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        protected void SaveUserSession(User user, int onlineUserId, int level)
        {
            CurrentSession.UserId = user.Id;
            CurrentSession.Username = user.Username;
            CurrentSession.OnUserId = onlineUserId;
            CurrentSession.SessionId = Session.SessionID;
            CurrentSession.Level = level;
        }
    }
}