namespace Fanex.N88.Admin.Controllers
{
    using System.Web.Mvc;
    using Fanex.N88.Admin.Areas.UserControl.Controllers;

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