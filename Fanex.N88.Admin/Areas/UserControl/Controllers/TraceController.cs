namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using Core.Web;
    using Fanex.N88.Admin.Core.Configuration;
    using Fanex.N88.Core.Helpers;
    using Fanex.N88.Core.Web;
    using N88.Core.Criteria.Trace;
    using N88.Core.Domain;
    using N88.Core.Model;
    using N88.Core.Model.ValueObject;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web.Attribute;
    using ViewModels.Trace;

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

        [N88Auth(FunctionKey = FunctionKeys.ExecSpLog)]
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

        public PartialViewResult N88UserAddToGroup(LogExecSp log)
        {
            if (log.ActionId == (int)LogAction.N88_User_Insert)
            {
                XElement elementNew = XElement.Parse(log.Data);

                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Username,
                        NewValue = elementNew.Attribute("Username").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Group_Name,
                        NewValue = elementNew.Attribute("GroupName").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogInsert.cshtml");
        }

        public PartialViewResult N88UserInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_User_Insert)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Username,
                        NewValue = element.Attribute("Username").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Group_Name,
                        NewValue = element.Attribute("GroupName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.First_Name,
                        NewValue = element.Attribute("FirstName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Last_Name,
                        NewValue = element.Attribute("LastName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Email,
                        NewValue = element.Attribute("Email").Value
                    },

                    new LogData
                    {
                        FieldName = "Licensee User",
                        NewValue = element.Attribute("IsLicenseeUser").Value == "1" ? "true" : "false"
                    },

                     new LogData
                    {
                        FieldName = "Source Site",
                        NewValue = element.Attribute("SourceSiteId").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogInsert.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88FunctionInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_Function_Insert)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Function_Name,
                        NewValue = element.Attribute("FunctionName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        NewValue = element.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Function_Key,
                        NewValue = element.Attribute("FunctionKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        NewValue = element.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Parent,
                        NewValue = element.Attribute("ParentName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.CRUD,
                        NewValue = element.Attribute("CRUD").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Display_Order,
                        NewValue = element.Attribute("DisplayOrder").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Environment,
                        NewValue = element.Attribute("Environment").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogInsert.cshtml");
        }

        public PartialViewResult N88GroupInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_Group_Insert)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Group_Name,
                        NewValue = element.Attribute("GroupName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        NewValue = element.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                    },

                     new LogData
                    {
                        FieldName = "Licensee Group",
                        NewValue = element.Attribute("IsLicenseeGroup").Value == "1" ? "true" : "false"
                    },

                     new LogData
                    {
                        FieldName = "Source Site",
                        NewValue = element.Attribute("SourceSiteId").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogInsert.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88MenuItemInsert(LogExecSp log)
        {
            XElement element = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_MenuItem_Insert)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Menu_Item_Name,
                        NewValue = element.Attribute("MenuItemName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Function_Name,
                        NewValue = element.Attribute("FunctionName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        NewValue = element.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        NewValue = element.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Parent,
                        NewValue = element.Attribute("ParentName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Policy,
                        NewValue = element.Attribute("Policy").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.URL,
                        NewValue = element.Attribute("URL").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Display_Order,
                        NewValue = element.Attribute("DisplayOrder").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogInsert.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88SiteInsertWithSiteUrl(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_Site_InsertWithSiteURL)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        NewValue = elementNew.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Key,
                        NewValue = elementNew.Attribute("SiteKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Secret_Access_Key,
                        NewValue = elementNew.Attribute("SecretAccessKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        NewValue = elementNew.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Environment,
                        NewValue = elementNew.Attribute("Environment").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                    },

                    new LogData
                    {
                        FieldName = ViewResources.IsLicenseeSite,
                        NewValue = elementNew.Attribute("IsLicenseeSite").Value == "1" ? "true" : "false"
                    }
                };

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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Site_InsertWithSiteURL.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88MenuItemUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_MenuItem_Update)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Menu_Item_Name,
                        OldValue = elementOld.Attribute("MenuItemName").Value,
                        NewValue = elementNew.Attribute("MenuItemName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Function_Name,
                        OldValue = elementOld.Attribute("FunctionName").Value,
                        NewValue = elementNew.Attribute("FunctionName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        OldValue = elementOld.Attribute("SiteName").Value,
                        NewValue = elementNew.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        OldValue = elementOld.Attribute("Description").Value,
                        NewValue = elementNew.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Parent,
                        OldValue = elementOld.Attribute("ParentName").Value,
                        NewValue = elementNew.Attribute("ParentName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Policy,
                        OldValue = elementOld.Attribute("Policy").Value,
                        NewValue = elementNew.Attribute("Policy").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.URL,
                        OldValue = elementOld.Attribute("URL").Value,
                        NewValue = elementNew.Attribute("URL").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Display_Order,
                        OldValue = elementOld.Attribute("DisplayOrder").Value,
                        NewValue = elementNew.Attribute("DisplayOrder").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88MenuItemUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_MenuItem_UpdateStatus)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Status,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88FunctionUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Function_Update
                || log.ActionId == (int)LogAction.N88_Function_Duplicate)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Function_Name,
                        OldValue = elementOld.Attribute("FunctionName").Value,
                        NewValue = elementNew.Attribute("FunctionName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        OldValue = elementOld.Attribute("SiteName").Value,
                        NewValue = elementNew.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Function_Key,
                        OldValue = elementOld.Attribute("FunctionKey").Value,
                        NewValue = elementNew.Attribute("FunctionKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        OldValue = elementOld.Attribute("Description").Value,
                        NewValue = elementNew.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Parent,
                        OldValue = elementOld.Attribute("ParentName").Value,
                        NewValue = elementNew.Attribute("ParentName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.CRUD,
                        OldValue = elementOld.Attribute("CRUD").Value,
                        NewValue = elementNew.Attribute("CRUD").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Display_Order,
                        OldValue = elementOld.Attribute("DisplayOrder").Value,
                        NewValue = elementNew.Attribute("DisplayOrder").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Environment,
                        OldValue = elementOld.Attribute("Environment").Value,
                        NewValue = elementNew.Attribute("Environment").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88FunctionUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Function_UpdateStatus)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Status,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88SiteUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Site_Update)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Site_Name,
                        OldValue = elementOld.Attribute("SiteName").Value,
                        NewValue = elementNew.Attribute("SiteName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Site_Key,
                        OldValue = elementOld.Attribute("SiteKey").Value,
                        NewValue = elementNew.Attribute("SiteKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Secret_Access_Key,
                        OldValue = elementOld.Attribute("SecretAccessKey").Value,
                        NewValue = elementNew.Attribute("SecretAccessKey").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        OldValue = elementOld.Attribute("Description").Value,
                        NewValue = elementNew.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Environment,
                        OldValue = elementOld.Attribute("Environment").Value,
                        NewValue = elementNew.Attribute("Environment").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88SiteUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Site_UpdateStatus)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Status,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88GroupUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Group_Update)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Group_Name,
                        OldValue = elementOld.Attribute("GroupName").Value,
                        NewValue = elementNew.Attribute("GroupName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Description,
                        OldValue = elementOld.Attribute("Description").Value,
                        NewValue = elementNew.Attribute("Description").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Active,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88GroupUpdateStatus(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Group_UpdateStatus)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Status,
                        OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                        NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        public PartialViewResult N88UserUpdate(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data ?? "<ROOT/>");
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_User_Update)
            {
                var listLogData = new List<LogData>
                {
                    new LogData
                    {
                        FieldName = ViewResources.Group_Name,
                        OldValue = elementOld.Attribute("GroupName").Value,
                        NewValue = elementNew.Attribute("GroupName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.First_Name,
                        OldValue = elementOld.Attribute("FirstName").Value,
                        NewValue = elementNew.Attribute("FirstName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Last_Name,
                        OldValue = elementOld.Attribute("LastName").Value,
                        NewValue = elementNew.Attribute("LastName").Value
                    },

                    new LogData
                    {
                        FieldName = ViewResources.Email,
                        OldValue = elementOld.Attribute("Email").Value,
                        NewValue = elementNew.Attribute("Email").Value
                    }
                };

                ViewBag.LogData = listLogData;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88LogUpdate.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88SiteUrl(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            // N88_Site_UpdateSiteURL || N88_Site_InsertSiteURL || N88_Site_UpdateDefaultSiteURL
            if (log.ActionId == (int)LogAction.N88_Site_UpdateSiteURL
                || log.ActionId == (int)LogAction.N88_Site_InsertSiteURL
                || log.ActionId == (int)LogAction.N88_Site_UpdateDefaultSiteURL)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Site_Url.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed", Scope = "method")]
        public PartialViewResult N88AssignPermission(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            // N88_Group_AssignFunction || N88_User_AssignFunction
            if (log.ActionId == (int)LogAction.N88_Group_AssignFunction
                || log.ActionId == (int)LogAction.N88_User_AssignFunction
                || log.ActionId == (int)LogAction.N88_Function_CopyPermission)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Assign_Permission.cshtml");
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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88AssignSite(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_User_AssignSite)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Assign_Site.cshtml");
        }

        public PartialViewResult N88AssignNewSite(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);

            if (log.ActionId == (int)LogAction.N88_User_AssignNewSite && elementNew != null)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Assign_New_Site.cshtml");
        }

        public PartialViewResult N88UpdateSite(LogExecSp log)
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

                DateTime.TryParse(oldElement.Attribute("ExpiredDate").Value, out DateTime expiredDate);
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

                DateTime.TryParse(newElement.Attribute("ExpiredDate").Value, out DateTime expiredDate);
                newSite.ExpiredDate = expiredDate;

                ViewBag.newSite = newSite;
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Update_Site.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88FunctionAssignToUsers(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Function_AssignFunctionToUsers)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Function_AssignToUsers.cshtml");
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public PartialViewResult N88FunctionAssignFunctionToGroups(LogExecSp log)
        {
            XElement elementNew = XElement.Parse(log.Data);
            XElement elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.N88_Function_AssignFunctionToGroups)
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

            return PartialView("~/Areas/UserControl/Views/Trace/_N88_Function_AssignToGroups.cshtml");
        }

        public PartialViewResult TaskN88RevokeUserPermission(LogExecSp log)
        {
            var taskN88RevokeUserPermissionVM = new TaskN88RevokeUserPermissionVM();

            XElement element = XElement.Parse(log.OldData ?? "<ROOT/>");

            if (log.ActionId == (int)LogAction.Task_N88_RevokeUserPermission && element != null)
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

                        taskN88RevokeUserPermissionVM.RevokedSites.Add(revokedSite);
                    }
                }
            }

            return PartialView("~/Areas/UserControl/Views/Trace/_Task_N88_RevokeUserPermission.cshtml", taskN88RevokeUserPermissionVM);
        }
    }
}