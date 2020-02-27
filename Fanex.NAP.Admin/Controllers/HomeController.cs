namespace Fanex.NAP.Admin.Controllers
{
    using System.Web.Mvc;
    using Fanex.NAP.Admin.Areas.UserControl.Controllers;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
