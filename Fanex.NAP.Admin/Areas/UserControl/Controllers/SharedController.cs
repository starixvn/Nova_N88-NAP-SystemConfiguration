namespace Fanex.NAP.Admin.Areas.UserControl.Controllers
{
    using System.Web.Mvc;
    using N88.Core.Web.Attribute;

    /// <summary>
    /// Shared controller.
    /// </summary>
    [NapSuperAuth]
    public class SharedController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            ViewBag.MenuItemList = CurrentSession.MenuItemList;

            return PartialView("~/Areas/UserControl/Views/Shared/_Menu.cshtml");
        }
    }
}