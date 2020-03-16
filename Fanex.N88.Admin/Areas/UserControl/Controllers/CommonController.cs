namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Common controller.
    /// </summary>
    public class CommonController : Controller
    {
        // GET: /UserControl/Common/
        public ActionResult Unauthorize()
        {
            return View();
        }

        /// <summary>
        /// Error page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Not found page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult NotFound()
        {
            return View();
        }
    }
}