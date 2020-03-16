namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System.Web.Mvc;
    using N88.Core.Web.Attribute;

    /// <summary>
    /// Shared controller.
    /// </summary>
    [N88SuperAuth]
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