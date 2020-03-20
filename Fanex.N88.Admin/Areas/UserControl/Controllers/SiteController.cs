namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Web.Mvc;
    using Fanex.N88.Admin.Core.Configuration;
    using N88.Core.Business;
    using N88.Core.Command;
    using N88.Core.Criteria.Site;
    using N88.Core.Domain;
    using N88.Core.Model.ValueObject;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;
    using N88.Core.Web.Attribute;

    [N88SuperAuth]
    public class SiteController : BaseController
    {
        #region Members

        private readonly ISiteService _siteService;
        private readonly IUserService _userService;

        #endregion Members

        #region Constructors

        public SiteController()
            : this(null, null)
        {
        }

        public SiteController(ISiteService siteService, IUserService userService)
        {
            _siteService = siteService ?? new SiteService { ServiceContext = ServiceContext };
            _userService = userService ?? new UserService { ServiceContext = ServiceContext };
        }

        #endregion Constructors

        #region Manage

        /// <summary>
        /// Manage site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Read)]
        public ActionResult Manage(GetAllSiteCriteria criteria)
        {
            ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageSite, Constant.Policy.Host);
            ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, Constant.Policy.Read);

            ViewBag.Site = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                                                    CurrentSession.UserId,
                                                    FunctionKeys.ManageUser,
                                                    Environment).Select(s => s.Id).ToList();
            ViewBag.SiteList = _siteService.GetAllWithNumOfUsers(Environment).OrderBy(s => s.Id).ToList();
            ViewBag.SiteUrlList = _siteService.GetAllSiteUrl(Environment).ToList();

            return View(criteria);
        }

        #endregion Manage

        #region Add New

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult AddNew()
        {
            var criteria = new InsertSiteCriteria
            {
                Environment = Environment,
                Urls = new List<SiteUrl>
                {
                    new SiteUrl
                    {
                        IsActive = true,
                        IsDefault = true
                    }
                }
            };

            return View(criteria);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
#pragma warning disable S1541 // Methods and properties should not be too complex
        public ActionResult AddNew(InsertSiteCriteria criteria)
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            if (!ModelState.IsValid)
            {
                return View(criteria);
            }

            // check url
            if (!ReferenceEquals(criteria.Urls, null))
            {
                foreach (SiteUrl itemUrl in criteria.Urls)
                {
                    if (!Uri.IsWellFormedUriString(itemUrl.Url, UriKind.Absolute))
                    {
                        ModelState.AddModelError("SiteUrls", string.Format(ViewResources.InvalidURLValidationMsg, criteria.SiteUrls));

                        return View(criteria);
                    }
                }
            }

            // checks SiteName duplication
            criteria.SiteName = criteria.SiteName.Trim();
            var site = _siteService.GetByName(criteria.SiteName);

            if (site != null)
            {
                ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.SiteName));

                return View(criteria);
            }

            // checks duplicate function key
            criteria.SiteKey = ConvertToKey(criteria.SiteName);
            site = _siteService.GetByKey(criteria.SiteKey);

            if (site != null)
            {
                ModelState.AddModelError("SiteKey", string.Format(ViewResources.DuplicateMsg, criteria.SiteKey));

                return View(criteria);
            }

            // checks url duplication
            criteria.Urls = criteria.Urls.Where(x => !string.IsNullOrWhiteSpace(x.Url)).ToList();

            if (criteria.Urls != null)
            {
                int distinctCount = criteria.Urls.Select(x => x.Url).Distinct().Count();

                if (distinctCount != criteria.Urls.Count)
                {
                    ModelState.AddModelError("Urls", ViewResources.The_Urls_must_be_unique);

                    return View(criteria);
                }
            }

            // add new site
            criteria.Environment = Environment;
            criteria.SecretAccessKey = GenerateAPPKey();

            ICommandResult result = _siteService.Insert(criteria);

            if (result != null && result.IsSuccessful)
            {
                ViewBag.Success = true;
            }

            return View(criteria);
        }

        #endregion Add New

        #region Edit

        /// <summary>
        /// Update site.
        /// </summary>
        /// <param name="id">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            var site = _siteService.GetById(id);

            if (site == null)
            {
                return RedirectToAction("NotFound", "Common");
            }

            var criteria = new UpdateSiteCriteria
            {
                SiteId = site.Id,
                SiteName = site.Name,
                SiteKey = site.SiteKey,
                SecretAccessKey = site.SecretAccessKey,
                Description = site.Description,
                IsActive = site.IsActive,
                Environment = site.Environment
            };

            return View(criteria);
        }

        /// <summary>
        /// Update site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult Edit(UpdateSiteCriteria criteria)
        {
            if (ModelState.IsValid)
            {
                // checks SiteName duplication
                criteria.SiteName = criteria.SiteName.Trim();
                var site = _siteService.GetByName(criteria.SiteName);

                if (site != null && site.Id != criteria.SiteId)
                {
                    ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.SiteName));

                    return View(criteria);
                }

                // checks duplicate function key
                if (!string.IsNullOrEmpty(criteria.SiteKey))
                {
                    site = _siteService.GetByKey(criteria.SiteKey);

                    if (site != null && site.Id != criteria.SiteId)
                    {
                        ModelState.AddModelError("SiteKey", string.Format(ViewResources.DuplicateMsg, criteria.SiteKey));
                        return View(criteria);
                    }
                }

                // updates site
                ICommandResult result = _siteService.Update(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Edit

        #region SiteUrl

        /// <summary>
        /// Update site URL.
        /// </summary>
        /// <param name="id">The site URL ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Read)]
        public ActionResult EditUrl(int id)
        {
            ISite site = _siteService.GetById(id);

            if (site == null)
            {
                return HttpNotFound();
            }

            ViewBag.SiteId = site.Id;
            ViewBag.SiteName = site.Name;
            ViewBag.Urls = _siteService.GetSiteUrlBySiteId(id, string.Empty).ToList();
            ViewFlags.SetEditRight(_userService.GetUserSiteFunctionKeyPolicy(
                                                                    CurrentSession.UserId,
                                                                    SiteId,
                                                                    FunctionKeys.ManageSite));

            return View();
        }

        /// <summary>
        /// Update site URL.
        /// </summary>
        /// <param name="id">The site URL ID.</param>
        /// <param name="url">The site URL.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult UpdateSiteUrl(int id, Uri url, bool isActive, string siteUrlEnvironment)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            SiteUrl siteUrl = _siteService.GetSiteUrlById(id);

            if (siteUrl == null)
            {
                return HttpNotFound();
            }

            string result = Constant.Ok;

            if (url == null || !url.IsAbsoluteUri)
            {
                result = "invalid";
            }
            else
            {
                // checks url duplication
                IEnumerable<SiteUrl> altUrls = _siteService.GetSiteUrlBySiteId(siteUrl.Site.Id, siteUrl.Environment);

                if (altUrls.Any(x => x.SiteUrlId != id && x.Url == url.OriginalString))
                {
                    result = "duplicate";
                }
                else
                {
                    // updates site url
                    var criteria = new UpdateSiteUrlCriteria
                    {
                        SiteUrlId = id,
                        SiteId = siteUrl.Site.Id,
                        Url = url.OriginalString,
                        Environment = siteUrlEnvironment,
                        IsActive = isActive,
                        IsDefault = siteUrl.IsDefault
                    };

                    ICommandResult cmdResult = _siteService.UpdateSiteUrl(criteria);

                    if (cmdResult == null || !cmdResult.IsSuccessful)
                    {
                        result = "error";
                    }
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Set default site URL.
        /// </summary>
        /// <param name="id">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult SetDefaultSiteUrl(int id, string siteUrlEnvironment)
        {
            if (Request.IsAjaxRequest())
            {
                SiteUrl siteUrl = _siteService.GetSiteUrlById(id);

                if (siteUrl != null)
                {
                    var criteria = new UpdateDefaultSiteUrlCriteria
                    {
                        SiteUrlId = id,
                        SiteId = siteUrl.Site.Id,
                        Environment = siteUrlEnvironment
                    };

                    ICommandResult cmdResult = _siteService.UpdateDefaultSiteUrl(criteria);

                    if (cmdResult == null || !cmdResult.IsSuccessful)
                    {
                        return Json("error");
                    }

                    return Json(Constant.Ok);
                }
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Add site URL.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="url">The site URL.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult AddSiteUrl(int siteId, Uri url, bool isActive, string siteUrlEnvironment)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            ISite site = _siteService.GetById(siteId);

            if (site == null)
            {
                return HttpNotFound();
            }
            string result = string.Empty;

            // check url
            if (url == null || !url.IsAbsoluteUri)
            {
                result = "invalid";
            }
            else
            {
                // checks url duplication
                var siteUrls = _siteService.GetSiteUrlBySiteId(site.Id, Environment);

                if (siteUrls.Any(x => x.Url == url.OriginalString))
                {
                    result = "duplicate";
                }
                else
                {
                    // add site url
                    var criteria = new InsertSiteUrlCriteria
                    {
                        SiteId = siteId,
                        Url = url.OriginalString,
                        Environment = siteUrlEnvironment,
                        IsActive = isActive,
                        IsDefault = false
                    };

                    ICommandResult cmdResult = _siteService.InsertSiteUrl(criteria);

                    result = cmdResult != null && cmdResult.IsSuccessful ?
                        criteria.SiteUrlId.ToString() :
                        "error";
                }
            }

            return Json(result);
        }

        #endregion SiteUrl

        #region Activation

        /// <summary>
        /// Update site status.
        /// </summary>
        /// <param name="id">The site ID.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public ActionResult Activate(int id, bool isActive)
        {
            if (Request.IsAjaxRequest())
            {
                ICommandResult result = _siteService.UpdateStatus(new UpdateStatusSiteCriteria
                {
                    SiteId = id,
                    IsActive = isActive
                });

                if (result != null && result.IsSuccessful)
                {
                    return Json(Constant.Ok);
                }
            }

            return new EmptyResult();
        }

        #endregion Activation

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public JsonResult GetSiteKey(int siteId, string siteName)
        {
            ISite site = _siteService.GetById(siteId);

            if (ReferenceEquals(site, null))
            {
                return Json(-1);
            }

            // checks Site Name duplication
            site = _siteService.GetByName(siteName);

            if (site != null && site.Id != siteId)
            {
                return Json(0);
            }

            string siteKey = ConvertToKey(siteName);

            site = _siteService.GetByKey(siteKey);

            if (site != null && site.Id != siteId)
            {
                return Json(-2);
            }

            return Json(siteKey);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
        public JsonResult GetSecretAccessKey()
        {
            return Json(GenerateAPPKey());
        }

        [HttpGet]
        public ActionResult GetSitesByType(bool isLicensee)
        {
            var criteria = new GetSiteByTypeCriteria
            {
                IsLicenseeSite = isLicensee,
                IsActive = true
            };
            var sites = _siteService.GetAllSiteByType(criteria);

            return Json(sites, JsonRequestBehavior.AllowGet);
        }

        private static string GenerateAPPKey()
        {
            var apiKey = string.Empty;

            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                // 256 bit
                byte[] secretKeyByteArray = new byte[32];

                cryptoProvider.GetBytes(secretKeyByteArray);
                apiKey = Convert.ToBase64String(secretKeyByteArray);
            }

            return apiKey;
        }
    }
}