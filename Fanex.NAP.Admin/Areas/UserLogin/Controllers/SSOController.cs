namespace Fanex.NAP.Admin.Areas.UserLogin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Security;
    using Common;
    using N88.Core.Criteria.User;
    using N88.Core.Model;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web.Session;

    /// <summary>
    /// SSO controller.
    /// </summary>
    public class SSOController : BaseController
    {
        private const int TimeExpiration = 30;

        private readonly IUserService _userService;
        private readonly IMenuItemService _menuItemService;

        public SSOController()
            : this(null, null, null)
        {
        }

        public SSOController(IUserService userService, IMenuItemService menuItemService, ICurrentSession currentSession)
        {
            _userService = userService ?? new UserService();
            _menuItemService = menuItemService ?? new MenuItemService();
            CurrentSession = currentSession ?? new CurrentSession();
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="onUserId">The online user ID.</param>
        /// <param name="time">The SSO time.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string userId, string onUserId, string time, Uri redirectUrl = null)
        {
            var userIdValue = Cryptography.CfsDeCrypt(userId.Replace(" ", "+"));
            var onUserIdValue = Cryptography.CfsDeCrypt(onUserId.Replace(" ", "+"));
            var timeValue = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
            string currentTime = _userService.UserSSOGetTime();

            TimeSpan expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(timeValue);

            if (expiration.TotalSeconds <= TimeExpiration)
            {
                // do SSO check-in
                var criteria = new UserSSOCheckInUserCriteria
                {
                    UserId = Convert.ToInt32(userIdValue),
                    SiteId = SiteId,
                    CheckOnUserId = Convert.ToInt32(onUserIdValue),
                    OnUserId = 0,
                    IP = CurrentContext.IP,
                    NetSessionId = Session.SessionID,
                    LoginDate = DateTime.Now,
                    BrowserName = CurrentContext.BrowserName,
                    CountryCode = CurrentContext.CountryCode,
                    Domain = CurrentContext.DomainName,
                    Platform = CurrentContext.Platform
                };

                _userService.UserSSOCheckIn(criteria);

                if (criteria.OnUserId != 0)
                {
                    var user = _userService.GetById(criteria.UserId) as User;
                    int level = _userService.GetUserSiteLevel(user.Id, SiteId);

                    SaveUserSession(user, criteria.OnUserId, level);

                    if (SiteId != 0)
                    {
                        CreateUserMenu(user.Id);
                    }

                    CreateAuthInfo(user, criteria.OnUserId);

                    return redirectUrl != null ? Redirect(redirectUrl.ToString()) : Redirect(FormsAuthentication.DefaultUrl);
                }
            }

            return Redirect(FormsAuthentication.LoginUrl);
        }

        /// <summary>
        /// Creates the user menu.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        private void CreateUserMenu(int userId)
        {
            CurrentSession.MenuItemList = (IList<MenuItem>)_menuItemService.GetByUserAndSiteIncludeGroup(
                userId,
                SiteId,
                Environment,
                true);
        }
    }
}