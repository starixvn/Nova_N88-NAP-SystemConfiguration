namespace Fanex.NAP.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using Core.Configuration;
    using Core.Web;
    using NAP.Core.Criteria.Trace;
    using NAP.Core.Domain;
    using NAP.Core.Helpers;
    using NAP.Core.Model;
    using NAP.Core.Model.ValueObject;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web;
    using NAP.Core.Web.Attribute;
    using ViewModels.Trace;

    /// <summary>
    /// Trace controller.
    /// </summary>
    public class TraceController : BaseController
    {
        private readonly ITraceService _traceService;
        private readonly IFunctionService _functionService;
        private readonly ISiteService _siteService;

        public TraceController()
            : this(null, null, null)
        {
        }

        public TraceController(ITraceService traceService, IFunctionService functionService, ISiteService siteService)
        {
            _traceService = traceService ?? new TraceService();
            _functionService = functionService ?? new FunctionService();
            _siteService = siteService ?? new SiteService();
        }

        #region Log Exec Sp

        /// <summary>
        /// Manage execute SP log.
        /// </summary>
        /// <param name="id">The page index.</param>
        /// <param name="size">The page size.</param>
        /// <param name="q">The item name.</param>
        /// <param name="username">The username.</param>
        /// <param name="from">From date value.</param>
        /// <param name="to">To date value.</param>
        /// <param name="filterBy">Filter by option.</param>
        /// <returns>Action result.</returns>
        [NapAuth(FunctionKey = FunctionKeys.ExecSpLog)]
        [HttpGet]
        public ActionResult LogExecSp(
            int id = 1,
            int size = 20,
            string q = EmptyString,
            string username = EmptyString,
            DateTime? from = null,
            DateTime? to = null,
            int filterBy = 1)
        {
            var criteria = new GetAllLogCriteria
            {
                PageIndex = id,
                PageSize = size,
                Name = q ?? string.Empty,
                Username = username,
                From = from ?? DateTime.Now,
                To = to ?? DateTime.Now.AddDays(1),
                FilterType = filterBy,
                TotalRows = 0
            };

            IQueryable<LogExecSp> listLog = _traceService.SearchExecSp(criteria);
            ViewBag.ListLog = new PagerList<LogExecSp>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, listLog);

            return View(criteria);
        }

        /// <summary>
        /// Manage execute SP log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult LogExecSp(GetAllLogCriteria criteria)
        {
            return RedirectToAction(
                 "LogExecSp",
                 new
                 {
                     id = criteria.PageIndex,
                     size = criteria.PageSize,
                     q = criteria.Name,
                     username = criteria.Username,
                     from = criteria.From.ToString("MMM dd,yyyy"),
                     to = criteria.To.ToString("MMM dd,yyyy"),
                     filterBy = criteria.FilterType
                 });
        }

        #endregion Log Exec Sp

        #region Log User Login

        /// <summary>
        /// Manage user login log.
        /// </summary>
        /// <param name="id">The page index.</param>
        /// <param name="size">The page size.</param>
        /// <param name="q">The username.</param>
        /// <param name="from">From date value.</param>
        /// <param name="to">To date value.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult LogUserLogin(
            int id = 1,
            int size = 20,
            string q = EmptyString,
            DateTime? from = null,
            DateTime? to = null,
            int siteId = 0)
        {
            var criteria = new GetAllLogUserLoginCriteria
            {
                PageIndex = id,
                PageSize = size,
                Name = q ?? string.Empty,
                From = from ?? DateTime.Now,
                To = to ?? DateTime.Now.AddDays(1),
                SiteId = siteId,
                TotalRows = 0
            };

            IQueryable<LogUserLogin> listLog = _traceService.SearchUserLogin(criteria);

            var listSite = _siteService.GetAll().ToList();
            listSite.Add(new Site
            {
                Id = 0,
                Name = ViewResources.NAP_Hub
            });

            ViewBag.ListSite = listSite.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToList();

            ViewBag.ListLog = new PagerList<LogUserLogin>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, listLog);

            return View(criteria);
        }

        /// <summary>
        /// Manage user login log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult LogUserLogin(GetAllLogUserLoginCriteria criteria)
        {
            return RedirectToAction(
                "LogUserLogin",
                new
                {
                    id = criteria.PageIndex,
                    size = criteria.PageSize,
                    q = criteria.Name,
                    from = criteria.From.ToString("MMM dd,yyyy"),
                    to = criteria.To.ToString("MMM dd,yyyy"),
                    siteId = criteria.SiteId
                });
        }

        #endregion Log User Login

        #region Ajax

        /// <summary>
        /// Get log insert detail.
        /// </summary>
        /// <param name="id">The log ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult GetLogInsertDetail(int id)
        {
            if (Request.IsAjaxRequest())
            {
                var log = _traceService.GetById(id);

                return View(log);
            }

            return View(new LogExecSp());
        }

        #endregion Ajax

        #region View log

        /// <summary>
        /// Get user insert log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPUserInsert(LogExecSp log)
        {
            if (log.ActionId == (int)LogAction.NAP_User_Insert)
            {
                XElement elementNew = XElement.Parse(log.Data);

                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Username,
                    NewValue = elementNew.Attribute("Username").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = elementNew.Attribute("GroupName").Value
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogInsert.cshtml");
        }

        /// <summary>
        /// Get customer insert log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPCustInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_Cust_Insert)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Username,
                    NewValue = element.Attribute("Username").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = element.Attribute("GroupName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.First_Name,
                    NewValue = element.Attribute("FirstName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Last_Name,
                    NewValue = element.Attribute("LastName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Email,
                    NewValue = element.Attribute("Email").Value
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogInsert.cshtml");
        }

        /// <summary>
        /// Get function insert log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPFunctionInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_Function_Insert)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Name,
                    NewValue = element.Attribute("FunctionName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = element.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Key,
                    NewValue = element.Attribute("FunctionKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Parent,
                    NewValue = element.Attribute("ParentName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.CRUD,
                    NewValue = element.Attribute("CRUD").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Display_Order,
                    NewValue = element.Attribute("DisplayOrder").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Environment,
                    NewValue = element.Attribute("Environment").Value
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogInsert.cshtml");
        }

        /// <summary>
        /// Get group insert log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPGroupInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_Group_Insert)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = element.Attribute("GroupName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogInsert.cshtml");
        }

        /// <summary>
        /// Get menu item insert log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPMenuItemInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_MenuItem_Insert)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Menu_Item_Name,
                    NewValue = element.Attribute("MenuItemName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Name,
                    NewValue = element.Attribute("FunctionName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = element.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Parent,
                    NewValue = element.Attribute("ParentName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Policy,
                    NewValue = element.Attribute("Policy").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.URL,
                    NewValue = element.Attribute("URL").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Display_Order,
                    NewValue = element.Attribute("DisplayOrder").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogInsert.cshtml");
        }

        /// <summary>
        /// Get insert site log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPSiteInsertWithSiteUrl(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_Site_InsertWithSiteURL)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = elementNew.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Key,
                    NewValue = elementNew.Attribute("SiteKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Secret_Access_Key,
                    NewValue = elementNew.Attribute("SecretAccessKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    NewValue = elementNew.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Environment,
                    NewValue = elementNew.Attribute("Environment").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                string strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var listSiteUrl = new List<SiteUrl>();
                XDocument documentSiteUrl = XDocument.Parse(strUrl);
                IEnumerable<XElement> siteUrl = documentSiteUrl.Descendants("ITEM");

                foreach (XElement item in siteUrl)
                {
                    var objSiteUrl = new SiteUrl
                    {
                        Url = item.Attribute("URL").Value,
                        Environment = item.Attribute("Environment").Value,
                        IsDefault = item.Attribute("IsDefault").Value == "1",
                        IsActive = item.Attribute("IsActive").Value == "1",
                    };
                    listSiteUrl.Add(objSiteUrl);
                }

                ViewBag.LogData = listLogData;
                ViewBag.SiteUrl = listSiteUrl;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Site_InsertWithSiteURL.cshtml");
        }

        /// <summary>
        /// Get update menu item log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPMenuItemUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_MenuItem_Update)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Menu_Item_Name,
                    OldValue = elementOld.Attribute("MenuItemName").Value,
                    NewValue = elementNew.Attribute("MenuItemName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Name,
                    OldValue = elementOld.Attribute("FunctionName").Value,
                    NewValue = elementNew.Attribute("FunctionName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Parent,
                    OldValue = elementOld.Attribute("ParentName").Value,
                    NewValue = elementNew.Attribute("ParentName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Policy,
                    OldValue = elementOld.Attribute("Policy").Value,
                    NewValue = elementNew.Attribute("Policy").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.URL,
                    OldValue = elementOld.Attribute("URL").Value,
                    NewValue = elementNew.Attribute("URL").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Display_Order,
                    OldValue = elementOld.Attribute("DisplayOrder").Value,
                    NewValue = elementNew.Attribute("DisplayOrder").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update menu item status log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPMenuItemUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_MenuItem_UpdateStatus)
            {
                var listLogData = new List<LogData>();

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update function log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPFunctionUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Function_Update
                || log.ActionId == (int)LogAction.NAP_Function_Duplicate)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Name,
                    OldValue = elementOld.Attribute("FunctionName").Value,
                    NewValue = elementNew.Attribute("FunctionName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Function_Key,
                    OldValue = elementOld.Attribute("FunctionKey").Value,
                    NewValue = elementNew.Attribute("FunctionKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Parent,
                    OldValue = elementOld.Attribute("ParentName").Value,
                    NewValue = elementNew.Attribute("ParentName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.CRUD,
                    OldValue = elementOld.Attribute("CRUD").Value,
                    NewValue = elementNew.Attribute("CRUD").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Display_Order,
                    OldValue = elementOld.Attribute("DisplayOrder").Value,
                    NewValue = elementNew.Attribute("DisplayOrder").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Environment,
                    OldValue = elementOld.Attribute("Environment").Value,
                    NewValue = elementNew.Attribute("Environment").Value
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update function status log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPFunctionUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Function_UpdateStatus)
            {
                var listLogData = new List<LogData>();

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update site log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPSiteUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Site_Update)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Site_Key,
                    OldValue = elementOld.Attribute("SiteKey").Value,
                    NewValue = elementNew.Attribute("SiteKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Secret_Access_Key,
                    OldValue = elementOld.Attribute("SecretAccessKey").Value,
                    NewValue = elementNew.Attribute("SecretAccessKey").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Environment,
                    OldValue = elementOld.Attribute("Environment").Value,
                    NewValue = elementNew.Attribute("Environment").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update site status log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPSiteUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Site_UpdateStatus)
            {
                var listLogData = new List<LogData>();

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update group log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPGroupUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Group_Update)
            {
                var listLogData = new List<LogData>();
                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Group_Name,
                    OldValue = elementOld.Attribute("GroupName").Value,
                    NewValue = elementNew.Attribute("GroupName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update group status log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPGroupUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Group_UpdateStatus)
            {
                var listLogData = new List<LogData>();

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get update customer log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        public PartialViewResult NAPCustUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data ?? "<ROOT/>");
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Cust_Update)
            {
                var listLogData = new List<LogData>();

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Group_Name,
                    OldValue = elementOld.Attribute("GroupName").Value,
                    NewValue = elementNew.Attribute("GroupName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.First_Name,
                    OldValue = elementOld.Attribute("FirstName").Value,
                    NewValue = elementNew.Attribute("FirstName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Last_Name,
                    OldValue = elementOld.Attribute("LastName").Value,
                    NewValue = elementNew.Attribute("LastName").Value
                });

                listLogData.Add(new LogData
                {
                    FieldName = ViewResources.Email,
                    OldValue = elementOld.Attribute("Email").Value,
                    NewValue = elementNew.Attribute("Email").Value
                });

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAPLogUpdate.cshtml");
        }

        /// <summary>
        /// Get site URL log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPSiteUrl(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            // NAP_Site_UpdateSiteURL || NAP_Site_InsertSiteURL || NAP_Site_UpdateDefaultSiteURL
            if (log.ActionId == (int)LogAction.NAP_Site_UpdateSiteURL
                || log.ActionId == (int)LogAction.NAP_Site_InsertSiteURL
                || log.ActionId == (int)LogAction.NAP_Site_UpdateDefaultSiteURL)
            {
                // Old data
                string strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                var listSiteUrlOld = new List<SiteUrl>();
                XDocument documentSiteUrlOld = XDocument.Parse(strUrlOld);
                var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

                foreach (var item in siteUrlOld)
                {
                    var objSiteUrl = new SiteUrl
                    {
                        Url = item.Attribute("URL").Value,
                        Environment = item.Attribute("Environment").Value,
                        IsDefault = item.Attribute("IsDefault").Value == "1",
                        IsActive = item.Attribute("IsActive").Value == "1",
                    };
                    listSiteUrlOld.Add(objSiteUrl);
                }

                ViewBag.SiteUrlOld = listSiteUrlOld;

                // New data
                string strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var listSiteUrl = new List<SiteUrl>();
                XDocument documentSiteUrl = XDocument.Parse(strUrl);
                var siteUrl = documentSiteUrl.Descendants("ITEM");

                foreach (var item in siteUrl)
                {
                    var objSiteUrl = new SiteUrl
                    {
                        Url = item.Attribute("URL").Value,
                        Environment = item.Attribute("Environment").Value,
                        IsDefault = item.Attribute("IsDefault").Value == "1",
                        IsActive = item.Attribute("IsActive").Value == "1",
                    };
                    listSiteUrl.Add(objSiteUrl);
                }

                ViewBag.SiteUrl = listSiteUrl;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Site_Url.cshtml");
        }

        /// <summary>
        /// Get assign permission log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed", Scope = "method")]
        public PartialViewResult NAPAssignPermission(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            // NAP_Group_AssignFunction || NAP_User_AssignFunction
            if (log.ActionId == (int)LogAction.NAP_Group_AssignFunction
                || log.ActionId == (int)LogAction.NAP_User_AssignFunction
                || log.ActionId == (int)LogAction.NAP_Function_CopyPermission)
            {
                // Get function list
                int siteId = Convert.ToInt32(elementNew.Attribute("SiteId").Value);

                ViewBag.FunctionList = _functionService.GetBySiteInHierarchy(siteId, Environment, true);

                // Old data

                if (elementOld.Attribute("Value") != null)
                {
                    var strFunctionPolicy = elementOld.Attribute("Value").Value.DecodeXMLValue();

                    if (!string.IsNullOrWhiteSpace(strFunctionPolicy))
                    {
                        ViewBag.ListOldData = GetOldFunctionPolicyList(strFunctionPolicy);
                    }
                }

                // New data
                if (elementNew.Attribute("Value") != null)
                {
                    var strFunctionPolicy = elementNew.Attribute("Value").Value.DecodeXMLValue();

                    if (!string.IsNullOrWhiteSpace(strFunctionPolicy))
                    {
                        ViewBag.ListNewData = GetNewFunctionPolicyList(strFunctionPolicy);
                    }
                }

                ISite site = _siteService.GetById(siteId);

                ViewBag.SiteName = site.Name;
            }

            #endregion View log

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Assign_Permission.cshtml");
        }

        private static Hashtable GetNewFunctionPolicyList(string strFunctionPolicy)
        {
            var hashNewPolicy = new Hashtable();

            XDocument documentFunctionPolicy = XDocument.Parse(strFunctionPolicy);
            IEnumerable<XElement> functionPolicy = documentFunctionPolicy.Descendants("ITEM");

            foreach (XElement item in functionPolicy)
            {
                short id = Convert.ToInt16(item.Attribute("FunctionId").Value);
                int policy = Convert.ToInt32(item.Attribute("Policy").Value);

                if (!hashNewPolicy.Contains(id))
                {
                    hashNewPolicy.Add(id, policy);
                }
            }

            return hashNewPolicy;
        }

        private List<FunctionPolicy> GetOldFunctionPolicyList(string strFunctionPolicy)
        {
            var listFunctionPolicyOld = new List<FunctionPolicy>();
            var documentFunctionPolicy = XDocument.Parse(strFunctionPolicy);
            IEnumerable<XElement> functionPolicy = documentFunctionPolicy.Descendants("ITEM");

            foreach (XElement item in functionPolicy)
            {
                var objFunctionPolicy = new FunctionPolicy
                {
                    Function = new Function
                    {
                        Id = Convert.ToInt16(item.Attribute("FunctionId").Value)
                    },
                    Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                };

                listFunctionPolicyOld.Add(objFunctionPolicy);
            }

            return listFunctionPolicyOld;
        }

        /// <summary>
        /// Get assign site log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPAssignSite(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_User_AssignSite)
            {
                // Old data
                if (elementOld.Attribute("Value") != null
                    && !string.IsNullOrWhiteSpace(elementOld.Attribute("Value").Value))
                {
                    string strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                    var listSiteUrlOld = new List<SiteLevel>();
                    XDocument documentSiteUrlOld = XDocument.Parse(strUrlOld);
                    var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

                    foreach (var item in siteUrlOld)
                    {
                        var objSiteUrl = new SiteLevel
                        {
                            Site = new Site
                            {
                                Id = Convert.ToByte(item.Attribute("SiteId").Value),
                                Name = item.Attribute("SiteName").Value
                            },
                            Level = Convert.ToInt32(item.Attribute("Level").Value),
                            IsActive = item.Attribute("IsActive").Value == "1"
                        };
                        listSiteUrlOld.Add(objSiteUrl);
                    }

                    ViewBag.SiteUrlOld = listSiteUrlOld;
                }

                // New data
                if (elementNew.Attribute("Value") != null)
                {
                    string strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                    var listSiteUrl = new List<SiteLevel>();
                    XDocument documentSiteUrl = XDocument.Parse(strUrl);
                    var siteUrl = documentSiteUrl.Descendants("ITEM");
                    foreach (var item in siteUrl)
                    {
                        var objSiteUrl = new SiteLevel
                        {
                            Site = new Site
                            {
                                Id = Convert.ToByte(item.Attribute("SiteId").Value),
                                Name = item.Attribute("SiteName").Value
                            },
                            Level = Convert.ToInt32(item.Attribute("Level").Value),
                            IsActive = item.Attribute("IsActive").Value == "1"
                        };
                        listSiteUrl.Add(objSiteUrl);
                    }

                    ViewBag.SiteUrl = listSiteUrl;
                }
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Assign_Site.cshtml");
        }

        /// <summary>
        /// Assign new site.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>The partial view result.</returns>
        public PartialViewResult NAPAssignNewSite(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.NAP_User_AssignNewSite && elementNew != null)
            {
                var newSite = new SiteLevel
                {
                    Site = new Site
                    {
                        Id = Convert.ToByte(elementNew.Attribute("SiteId").Value),
                        Name = Convert.ToString(elementNew.Attribute("SiteName").Value)
                    },
                    Level = Convert.ToInt32(elementNew.Attribute("Level").Value),
                    IsActive = elementNew.Attribute("IsActive").Value == "1"
                };

                ViewBag.NewLog = newSite;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Assign_New_Site.cshtml");
        }

        /// <summary>
        /// Update site.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>The partial view result.</returns>
        public PartialViewResult NAPUpdateSite(LogExecSp log)
        {
            XElement newElement = XElement.Parse(log.Data);
            XElement oldElement = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (oldElement.Attribute("UserId") != null)
            {
                var oldSite = new SiteLevel
                {
                    Site = new Site
                    {
                        Id = Convert.ToByte(oldElement.Attribute("SiteId").Value),
                        Name = Convert.ToString(oldElement.Attribute("SiteName").Value)
                    },
                    Level = Convert.ToInt32(oldElement.Attribute("Level").Value),
                    IsActive = oldElement.Attribute("IsActive").Value == "1",
                    IsNeverExpired = oldElement.Attribute("IsNeverExpired").Value == "1"
                };

                DateTime expiredDate;
                DateTime.TryParse(oldElement.Attribute("ExpiredDate").Value, out expiredDate);
                oldSite.ExpiredDate = expiredDate;

                ViewBag.oldSite = oldSite;
            }

            if (newElement.Attribute("UserId") != null)
            {
                var newSite = new SiteLevel
                {
                    Site = new Site
                    {
                        Id = Convert.ToByte(newElement.Attribute("SiteId").Value),
                        Name = Convert.ToString(newElement.Attribute("SiteName").Value)
                    },
                    Level = Convert.ToInt32(newElement.Attribute("Level").Value),
                    IsActive = newElement.Attribute("IsActive").Value == "1",
                    IsNeverExpired = newElement.Attribute("IsNeverExpired").Value == "1"
                };

                DateTime expiredDate;
                DateTime.TryParse(newElement.Attribute("ExpiredDate").Value, out expiredDate);
                newSite.ExpiredDate = expiredDate;

                ViewBag.newSite = newSite;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Update_Site.cshtml");
        }

        /// <summary>
        /// Get assign permission to users log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPFunctionAssignToUsers(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToUsers)
            {
                // Old data
                string strUserPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                var listUserPolicyOld = new List<UserPolicy>();
                XDocument documentUserPolicyOld = XDocument.Parse(strUserPolicyOld);
                var userPolicyOld = documentUserPolicyOld.Descendants("ITEM");

                foreach (var item in userPolicyOld)
                {
                    var userPolicy = new UserPolicy
                    {
                        User = new User
                        {
                            Username = item.Attribute("UserName").Value
                        },
                        Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                    };
                    listUserPolicyOld.Add(userPolicy);
                }

                ViewBag.UserPolicyOld = listUserPolicyOld;

                // New data
                string strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var listUserPolicy = new List<UserPolicy>();
                XDocument documentUserPolicy = XDocument.Parse(strUrl);
                var userPolicyNew = documentUserPolicy.Descendants("ITEM");

                foreach (var item in userPolicyNew)
                {
                    var userPolicy = new UserPolicy
                    {
                        User = new User
                        {
                            Username = item.Attribute("UserName").Value
                        },
                        Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                    };
                    listUserPolicy.Add(userPolicy);
                }

                ViewBag.UserPolicyNew = listUserPolicy;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Function_AssignToUsers.cshtml");
        }

        /// <summary>
        /// Get assign permission to groups log detail.
        /// </summary>
        /// <param name="log">Execute SP log.</param>
        /// <returns>Partial view result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult NAPFunctionAssignFunctionToGroups(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToGroups)
            {
                // Old data
                string strGroupPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                var listGroupPolicyOld = new List<GroupPolicy>();
                XDocument documentGroupPolicyOld = XDocument.Parse(strGroupPolicyOld);
                var groupPolicyOld = documentGroupPolicyOld.Descendants("ITEM");

                foreach (var item in groupPolicyOld)
                {
                    var groupPolicy = new GroupPolicy
                    {
                        Group = new Group
                        {
                            Name = item.Attribute("GroupName").Value
                        },
                        Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                    };
                    listGroupPolicyOld.Add(groupPolicy);
                }

                ViewBag.GroupPolicyOld = listGroupPolicyOld;

                // New data
                string strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var listGroupPolicy = new List<GroupPolicy>();
                XDocument documentGroupPolicy = XDocument.Parse(strUrl);
                var groupPolicyNew = documentGroupPolicy.Descendants("ITEM");

                foreach (var item in groupPolicyNew)
                {
                    var groupPolicy = new GroupPolicy
                    {
                        Group = new Group
                        {
                            Name = item.Attribute("GroupName").Value
                        },
                        Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                    };
                    listGroupPolicy.Add(groupPolicy);
                }

                ViewBag.GroupPolicyNew = listGroupPolicy;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_NAP_Function_AssignToGroups.cshtml");
        }

        /// <summary>
        /// Get revoked sites when run job revoke user permission
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public PartialViewResult TaskNAPRevokeUserPermission(LogExecSp log)
        {
            var taskNAPRevokeUserPermissionVM = new TaskNAPRevokeUserPermissionVM();

            XElement element = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.Task_NAP_RevokeUserPermission && element != null)
            {
                var elementValue = element.Attribute("Value").Value.DecodeXMLValue();

                if (!string.IsNullOrWhiteSpace(elementValue))
                {
                    IEnumerable<XElement> items = XDocument.Parse(elementValue).Descendants("ITEM");

                    foreach (XElement item in items)
                    {
                        var revokedSite = new RevokedSite
                        {
                            SiteId = Convert.ToInt32(item.Attribute("SiteId").Value),
                            SiteName = item.Attribute("SiteName").Value
                        };

                        taskNAPRevokeUserPermissionVM.RevokedSites.Add(revokedSite);
                    }
                }
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_Task_NAP_RevokeUserPermission.cshtml", taskNAPRevokeUserPermissionVM);
        }
    }
}