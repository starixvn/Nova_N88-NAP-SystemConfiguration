namespace Fanex.N88.Admin.Areas.UserLogin.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

    /// <summary>
    /// Common controller.
    /// </summary>
    public class CommonController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        /// <summary>
        /// Offline page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Offline()
        {
            var downTime = _maintenanceService.GetUnderMaintenanceTime();

            if (!downTime.IsUnderMaintenanceTime)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View();
        }
    }
}