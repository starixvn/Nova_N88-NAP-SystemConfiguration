namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System.Text;
    using System.Web.Mvc;
    using N88.Core.Business;
    using N88.Core.Configuration;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;
    using N88.Core.Web.Context;
    using N88.Core.Web.Session;

    /// <summary>
    /// Base controller.
    /// </summary>
    public class BaseController : Controller
    {
        protected const string EmptyString = "";

        protected const int N88AdminId = 1;
        private readonly int _siteId = N88Settings.Settings.SiteId;
        private readonly string _environment = N88Settings.Settings.Environment;
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
                IsN88AdminSite = _siteId.Equals(N88AdminId),
                HasEditRight = false,
                IsReCheckin = N88Settings.Settings.EnableReCheckIn
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

        protected bool ThisSiteIsN88Admin => _siteId == N88AdminId;

        protected bool UserIsPortalAdmin => _currentSession.Level == (int)Constant.Level.PortalAdmin;

        protected bool UserIsSiteAdmin => _currentSession.Level == (int)Constant.Level.SiteAdmin;

        protected ICurrentContext CurrentContext => _currentContext;

        protected ICurrentSession CurrentSession => _currentSession;

        protected IServiceContext ServiceContext => _serviceContext;

        public bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = _userService.GetUserSiteFunctionKeyPolicy(CurrentSession.UserId, _siteId, functionKey);

            return functionPolicy >= (int)policy;
        }

        public bool HasRightOnSite(int siteId, string functionKey, Constant.Policy policy)
        {
            int functionPolicy = _userService.GetUserSiteFunctionKeyPolicy(CurrentSession.UserId, siteId, functionKey);

            return functionPolicy >= (int)policy;
        }

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