namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System.Web.Mvc;

    public class CommonController : BaseController
    {
        public ActionResult Unauthorize()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}