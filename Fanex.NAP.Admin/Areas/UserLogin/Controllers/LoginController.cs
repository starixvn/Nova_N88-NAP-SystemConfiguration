namespace Fanex.NAP.Admin.Areas.UserLogin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Common.Extensions;
    using N88.Core.Configuration;
    using N88.Core.Criteria.User;
    using N88.Core.Model;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;
    using N88.Core.Web.Session;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

    /// <summary>
    /// Login controller.
    /// </summary>
    public class LoginController : BaseController
    {
        #region Members

        private const string LoginFormToken = "_SecureLoginFormToken";

        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ISiteService _siteService;
        private readonly IMenuItemService _menuItemService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly HashAlgorithm _hashAlgorithm;

        #endregion Members

        #region Constructors

        public LoginController()
            : this(null, null, null, null, null, null)
        {
        }

        public LoginController(
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService,
            ICurrentSession currentSession,
            IMaintenanceService maintenanceService)
        {
            _userService = userService ?? new UserService();
            _groupService = groupService ?? new GroupService();
            _siteService = siteService ?? new SiteService();
            _menuItemService = menuItemService ?? new MenuItemService();
            CurrentSession = currentSession ?? new CurrentSession();
            _maintenanceService = maintenanceService ?? new MaintenanceService();
            _hashAlgorithm = new SHA256Managed();
        }

        #endregion Constructors

        #region Properties

        public string FormToken
        {
            get
            {
                if (Session[LoginFormToken] == null)
                {
                    return string.Empty;
                }

                return Session[LoginFormToken].ToString();
            }

            set
            {
                Session[LoginFormToken] = value;
            }
        }

        #endregion Properties

        #region Sign in

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (!NapSettings.Settings.IsNapHub && !EnableDirectLogin)
            {
                // if login-directly feature is disabled then redirect users to NAP Hub
                return Redirect(NapSettings.Settings.HubUrl);
            }

            var isUM = _maintenanceService.IsUnderMaintenance();

            if (isUM)
            {
                return RedirectToAction("Offline", "Common");
            }

            GenerateToken();

            return View();
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password.</param>
        /// <param name="loginToken">The login token.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [ValidateInput(false)]
#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public ActionResult Index(string username, string password, string loginToken)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            if (EnableDirectLogin)
            {
                if (!username.IsNullOrTrimEmpty() && !password.IsNullOrTrimEmpty() && CheckValidToken(loginToken))
                {
                    var user = _userService.GetByUsername(username.Trim(), false) as User;

                    if (user != null)
                    {
                        string hashPassword = GetHash(user.Password + loginToken);

#pragma warning disable S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        if (password == hashPassword)
#pragma warning restore S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        {
                            var siteLevelList = _siteService.GetSiteLevelByUser(user.Id, Environment); // gets all sites of logging-in user

                            ClearToken();

                            if (siteLevelList.Any(x => x.Site.Id == SiteId))
                            {
                                var group = user.Group == null ? null : _groupService.GetById(user.Group.Id);

                                if ((group == null || group.IsActive)
                                    && siteLevelList.Any(x => x.Site.Id == SiteId && x.IsActive))
                                {
                                    // check-in user
                                    var criteria = new UserCheckInUserCriteria
                                    {
                                        UserId = user.Id,
                                        SiteId = SiteId,
                                        NetSessionId = Session.SessionID,
                                        Ip = CurrentContext.IP,
                                        LoginDate = DateTime.Now,
                                        BrowserName = CurrentContext.BrowserName,
                                        CountryCode = CurrentContext.CountryCode,
                                        Domain = CurrentContext.DomainName,
                                        Platform = CurrentContext.Platform
                                    };

                                    _userService.UserCheckIn(criteria);

                                    int level = _userService.GetUserSiteLevel(user.Id, SiteId);

                                    SaveUserSession(user, criteria.OnUserId, level);

                                    CreateUserMenu(user.Id);
                                    CreateAuthInfo(user, criteria.OnUserId);

                                    return Redirect(FormsAuthentication.DefaultUrl);
                                }
                                else
                                {
                                    ViewBag.Message = ViewResources.Your_account_was_suspended;
                                }
                            }
                            else
                            {
                                ViewBag.Message = ViewResources.You_have_no_authorization;
                            }
                        }
                        else
                        {
                            // wrong password
                            ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                        }
                    }
                    else
                    {
                        // user doesn't exist
                        ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                    }
                }
                else
                {
                    ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                }
            }
            else
            {
                ViewBag.Message = ViewResources.You_have_no_authorization;
            }

            GenerateToken();

            return View();
        }

        #endregion Sign in

        #region Others

        /// <summary>
        /// User log out.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult LogOut()
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = CurrentSession.OnUserId
            };

            _userService.UserLogOut(criteria);

            FormsAuthentication.SignOut();
            DeleteCookiesAndClearSession();

            return Redirect(FormsAuthentication.LoginUrl);
        }

        private void DeleteCookiesAndClearSession()
        {
            Response.Cookies.Clear();

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            authCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(authCookie);

            CurrentSession.ClearAll();
            Session.Clear();
            Session.Abandon();
        }

        /// <summary>
        /// User re-check login.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult ReCheckLogin()
        {
            var isUM = _maintenanceService.IsUnderMaintenance();

            if (isUM)
            {
                return Json("-1");
            }

            // check if not nap hub
            if (SiteId != 0)
            {
                var siteStatus = _siteService.GetStatusById(SiteId);

                if (!siteStatus)
                {
                    return Json("1");
                }
            }

            var criteria = new UserReCheckInUserCriteria
            {
                OnUserId = Convert.ToInt32(CurrentSession.OnUserId)
            };

            _userService.ReUserCheckIn(criteria);

            return Json(criteria.IsKickedOut ? "1" : "0");
        }

        #endregion Others

        #region Helpers

        /// <summary>
        /// Create user menu.
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

        /// <summary>
        /// Adds the login token to cookie.
        /// </summary>
        /// <param name="loginToken">The login token.</param>
        private void AddTokenToCookie(string loginToken)
        {
            // Create the non-persistent secure form token cookie
            var responseCookie = new HttpCookie(LoginFormToken)
            {
                HttpOnly = true,
                Value = loginToken
            };

            // If we are using SSL, the cookie should be set to secure to
            // prevent it from being sent over HTTP connections
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }

            // Add the cookie to the response
            Response.Cookies.Set(responseCookie);
        }

        /// <summary>
        /// Generates the token for login form.
        /// </summary>
        private void GenerateToken()
        {
            string loginToken = Guid.NewGuid().ToString("N");

            FormToken = loginToken;
            ViewBag.LoginToken = loginToken;
            AddTokenToCookie(loginToken);
        }

        /// <summary>
        /// Checks if the token is valid or not.
        /// </summary>
        /// <param name="loginToken">The login token.</param>
        /// <returns>Boolean value indicates if the login token is valid or not.</returns>
        private bool CheckValidToken(string loginToken)
        {
            return !string.IsNullOrWhiteSpace(loginToken)
                && FormToken == loginToken
                && Request.Cookies[LoginFormToken] != null
                && loginToken == Request.Cookies[LoginFormToken].Value;
        }

        /// <summary>
        /// Gets the hash value of input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Hash value of input.</returns>
        private string GetHash(string input)
        {
            var encrypted = string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var hashData = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLowerInvariant();
            }

            return encrypted;
        }

        /// <summary>
        /// Clears the token.
        /// </summary>
        private void ClearToken()
        {
            Session.Remove(LoginFormToken);

            if (Request.Cookies[LoginFormToken] != null)
            {
                var cookie = new HttpCookie(LoginFormToken);

                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }
        }

        #endregion Helpers
    }
}