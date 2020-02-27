namespace Fanex.NAP.Admin.Areas.UserControl.Controllers
{
    using System.Text;
    using System.Web.Mvc;
    using NAP.Core.Business;
    using NAP.Core.Configuration;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web;
    using NAP.Core.Web.Context;
    using NAP.Core.Web.Session;

    /// <summary>
    /// Base controller.
    /// </summary>
    public class BaseController : Controller
    {
        protected const string EmptyString = "";

        protected const int NapAdminId = 1;

        private readonly int _siteId = NapSettings.Settings.SiteId;

        private readonly string _environment = NapSettings.Settings.Environment;

        private readonly ICurrentContext _currentContext;

        private readonly ICurrentSession _currentSession;

        private readonly IServiceContext _serviceContext;

        private readonly IUserService _userService;

        public BaseController()
        {
            _currentContext = new CurrentContext();
            ViewBag.Session = _currentSession = new CurrentSession();
            _serviceContext = new ServiceContext
            {
                UserId = _currentSession.UserId,
                SiteId = _siteId,
                Ip = _currentContext.IP
            };

            ViewFlags = new ViewFlags
            {
                IsLive = Environment.Equals(Constant.Live),
                IsNapAdminSite = _siteId.Equals(NapAdminId),
                HasEditRight = false,
                IsReCheckin = NapSettings.Settings.EnableReCheckIn
            };

            _userService = new UserService { ServiceContext = _serviceContext };
        }

        protected ViewFlags ViewFlags
        {
            get
            {
                return ViewBag.ViewFlags;
            }

            set
            {
                ViewBag.ViewFlags = value;
            }
        }

        protected int SiteId => _siteId;

        protected string Environment => _environment;

        protected bool ThisSiteIsNapAdmin => _siteId == NapAdminId;

        protected bool UserIsPortalAdmin => _currentSession.Level == (int)Constant.Level.PortalAdmin;

        protected bool UserIsSiteAdmin => _currentSession.Level == (int)Constant.Level.SiteAdmin;

        protected ICurrentContext CurrentContext => _currentContext;

        protected ICurrentSession CurrentSession => _currentSession;

        protected IServiceContext ServiceContext => _serviceContext;

        /// <summary>
        /// Determines whether the specified user has the right on a function.
        /// </summary>
        /// <param name="functionKey">The function key.</param>
        /// <param name="policy">The policy.</param>
        /// <returns>Whether the specified user has the right on a function.</returns>
        public bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = _userService.GetUserSiteFunctionKeyPolicy(CurrentSession.UserId, _siteId, functionKey);

            return functionPolicy >= (int)policy;
        }

        /// <summary>
        /// Determines whether the specified user has the right on a function of a site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="functionKey">The function key.</param>
        /// <param name="policy">The policy.</param>
        /// <returns>Whether the specified user has the right on a function.</returns>
        public bool HasRightOnSite(int siteId, string functionKey, Constant.Policy policy)
        {
            int functionPolicy = _userService.GetUserSiteFunctionKeyPolicy(CurrentSession.UserId, siteId, functionKey);

            return functionPolicy >= (int)policy;
        }

        /// <summary>
        /// Generate function key from function name.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <returns>Function key.</returns>
        protected static string ConvertToKey(string name)
        {
            string functionKey = name.Replace(Constant.Space, string.Empty);

            var sb = new StringBuilder();
            int length = functionKey.Length;

            for (int i = 0; i < length; i++)
            {
                char item = functionKey[i];

                if (char.IsLetterOrDigit(item))
                {
                    sb.Append(item);
                }
                else
                {
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }
    }
}