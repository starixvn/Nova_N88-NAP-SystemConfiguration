namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.N88.Admin.Core.Configuration;
    using Fanex.N88.Core.Business;
    using Fanex.N88.Core.Criteria.Function;
    using Fanex.N88.Core.Web.Attribute;
    using Fanex.N88.Core.Helpers;
    using N88.Core.Command;
    using N88.Core.Criteria.User;
    using N88.Core.Domain;
    using N88.Core.Model;
    using N88.Core.Model.ValueObject;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;

    /// <summary>
    /// Function controller.
    /// </summary>
    [N88SuperAuth]
    public class FunctionController : BaseController
    {
        #region Members

        private readonly IFunctionService _functionService;
        private readonly ISiteService _siteService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        #endregion Members

        #region Constructors

        public FunctionController()
            : this(null, null, null, null)
        {
        }

        public FunctionController(
            IFunctionService functionService,
            ISiteService siteService,
            IUserService userService,
            IGroupService groupService)
        {
            _functionService = functionService ?? new FunctionService { ServiceContext = ServiceContext };
            _siteService = siteService ?? new SiteService { ServiceContext = ServiceContext };
            _userService = userService ?? new UserService { ServiceContext = ServiceContext };
            _groupService = groupService ?? new GroupService { ServiceContext = ServiceContext };
        }

        #endregion Constructors

        #region Manage

        /// <summary>
        /// Manage function.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
        public ActionResult Manage(int? siteId)
        {
            var criteria = new GetBySiteFunctionCriteria
            {
                SiteId = siteId ?? SiteId,
                Environment = Environment
            };

            PrepareManageData(criteria);

            return View(criteria);
        }

        /// <summary>
        /// Manage function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
        public ActionResult Manage(GetBySiteFunctionCriteria criteria)
        {
            PrepareManageData(criteria);

            return View(criteria);
        }

        #endregion Manage

        #region Add new

        /// <summary>
        /// Add new function.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult AddNew(int? siteId = null)
        {
            var criteria = new InsertFunctionCriteria
            {
                SiteId = siteId ?? SiteId,
                Environment = Environment
            };

            GetSitesAndFunctionsForAddNew(criteria);

            return View(criteria);
        }

        /// <summary>
        /// Add new function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult AddNew(InsertFunctionCriteria criteria)
        {
            GetSitesAndFunctionsForAddNew(criteria);

            if (ModelState.IsValid)
            {
                // checks duplicate function name
                criteria.FunctionName = criteria.FunctionName.Trim();
                IFunction function = _functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

                if (function != null)
                {
                    ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                    return View(criteria);
                }

                // checks duplicate function key
                criteria.FunctionKey = ConvertToKey(criteria.FunctionName);
                function = _functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

                if (function != null)
                {
                    ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                    return View(criteria);
                }

                // adds function
                criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
                criteria.ParentName = (criteria.ParentId == null || criteria.ParentId == 0) ? null : criteria.ParentName;
                criteria.CreatedBy = CurrentSession.UserId;
                criteria.CreatedDate = DateTime.Now;

                // inserts function and assigns it to all site-admins of the current site and all portal admins
                ICommandResult result = _functionService.Insert(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Add new

        #region Duplicate Function

        /// <summary>
        /// Duplicate function.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult Duplicate(int id, int? siteId = null)
        {
            IFunction function = _functionService.GetById(id);

            if (function == null)
            {
                return HttpNotFound();
            }

            ViewBag.OriginFunctionName = function.Name;

            ISite site = _siteService.GetById(function.Site.Id);

            var criteria = new DuplicateFunctionCriteria
            {
                FunctionId = function.Id,
                FunctionName = function.Name + " Copy",
                Description = function.Description,
                SiteId = SiteId,
                SiteName = site.Name,
                Readonly = function.Readonly,
                ParentId = function.Parent != null ? (int?)function.Parent.Id : null,
                DisplayOrder = function.DisplayOrder,
                IsActive = function.IsActive,
                Environment = function.Environment,
                ReturnSiteId = siteId ?? function.Site.Id
            };

            GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

            return View(criteria);
        }

        /// <summary>
        /// Duplicate function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult Duplicate(DuplicateFunctionCriteria criteria)
        {
            var function = _functionService.GetById(criteria.FunctionId);

            if (function == null)
            {
                return HttpNotFound();
            }

            ViewBag.OriginFunctionName = function.Name;
            criteria.SiteId = function.Site.Id;
            GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

            if (ModelState.IsValid)
            {
                // checks duplicate function name
                criteria.FunctionName = criteria.FunctionName.Trim();

                if (_functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId) != null)
                {
                    ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                    return View(criteria);
                }

                // checks duplicate function key
                criteria.FunctionKey = ConvertToKey(criteria.FunctionName);

                if (_functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId) != null)
                {
                    ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                    return View(criteria);
                }

                // duplicates function
                criteria.DuplicateFunctionId = function.Id;
                criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
                criteria.ParentName = (criteria.ParentId == null || criteria.ParentId == 0) ? null : criteria.ParentName;
                criteria.CreatedBy = CurrentSession.UserId;
                criteria.CreatedDate = DateTime.Now;

                ICommandResult result = _functionService.Duplicate(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Duplicate Function

        #region Copy Permisison

        /// <summary>
        /// Copy permission.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult CopyPermission(int id, int? siteId = null)
        {
            IFunction function = _functionService.GetById(id);

            if (function == null)
            {
                return HttpNotFound();
            }

            var criteria = new CopyPermissionCriteria
            {
                ToFunctionId = function.Id,
                ToFunctionName = function.Name,
                SiteId = siteId ?? function.Site.Id,
                Environment = function.Environment,
                ReturnSiteId = siteId ?? function.Site.Id
            };

            ViewBag.FunctionList =
                this._functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

            return View(criteria);
        }

        /// <summary>
        /// Copy permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult CopyPermission(CopyPermissionCriteria criteria)
        {
            IFunction function = _functionService.GetById(criteria.ToFunctionId);

            if (function == null)
            {
                return HttpNotFound();
            }

            ViewBag.FunctionList =
                this._functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

            if (ModelState.IsValid)
            {
                criteria.ModifiedBy = CurrentSession.UserId;
                criteria.ModifiedDate = DateTime.Now;

                ICommandResult result = _functionService.CopyPermission(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Copy Permisison

        #region Update

        /// <summary>
        /// Update function.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            IFunction function = _functionService.GetById(id);

            if (object.ReferenceEquals(function, null))
            {
                return RedirectToAction("Manage");
            }

            var criteria = new UpdateFunctionCriteria
            {
                FunctionId = function.Id,
                SiteId = function.Site.Id,
                SiteName = function.Site.Name,
                FunctionName = function.Name,
                FunctionKey = function.Key,
                Description = function.Description,
                ParentId = function.Parent == null ? null : (int?)function.Parent.Id,
                ParentName = function.Parent == null ? null : function.Parent.Name,
                Readonly = function.Readonly,
                DisplayOrder = function.DisplayOrder,
                IsActive = function.IsActive,
                Environment = function.Environment
            };

            // get parent function list
            IEnumerable<IFunction> functionList = _functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

            ViewBag.ParentFunctionItemList = functionList
                .Where(f => f.Id != id && (f.Parent == null || f.Parent.Id != id))
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                    Selected = function.Parent != null && s.Id == function.Parent.Id
                }).ToList();

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
#pragma warning disable S1541 // Methods and properties should not be too complex
        public ActionResult Edit(UpdateFunctionCriteria criteria)
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            IFunction currentFunction = _functionService.GetById(criteria.FunctionId);

            if (ReferenceEquals(currentFunction, null))
            {
                return RedirectToAction("Manage");
            }

            IEnumerable<IFunction> functionList = _functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

            ViewBag.ParentFunctionItemList = functionList.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
                Selected = criteria.ParentId != null && s.Id == criteria.ParentId
            }).ToList();

            if (ModelState.IsValid)
            {
                // checks Function Name duplication
                criteria.FunctionName = criteria.FunctionName.Trim();
                IFunction function = _functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

                if (function != null && function.Id != criteria.FunctionId)
                {
                    criteria.FunctionKey = currentFunction.Key;

                    ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));

                    return View(criteria);
                }

                // checks Function Key duplication
                if (criteria.FunctionKey != null)
                {
                    function = _functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

                    if (function != null && function.Id != criteria.FunctionId)
                    {
                        ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));

                        return View(criteria);
                    }
                }
                else
                {
                    criteria.FunctionKey = ConvertToKey(criteria.FunctionName);
                }

                // check for all user,group has permission on this function when CRUD change
                if (criteria.Crud < currentFunction.Crud &&
                    CurrentSession.Level != (int)Constant.Level.SiteAdmin &&
                    CurrentSession.Level != (int)Constant.Level.PortalAdmin)
                {
                    // list user has host permission in this function
                    var listUsers = _userService.GetByFunctionAndSite(currentFunction.Id, currentFunction.Site.Id)
                                                    .Where(u => u.Policy == (int)Constant.Policy.Host).ToList();

                    // list group has host permission in this function
                    var listGroups = _groupService.GetByFunctionAndSite(currentFunction.Id, currentFunction.Site.Id)
                                                    .Where(u => u.Policy == (int)Constant.Policy.Host).ToList();

                    if (listUsers.Count > 0 || listGroups.Count > 0)
                    {
                        ModelState.AddModelError("Readonly", ViewResources.Privileged_User_Group_Message);

                        return View(criteria);
                    }
                }

                // update function
                criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
                criteria.ParentName = criteria.ParentId == null || criteria.ParentId == 0 ? null : criteria.ParentName;
                criteria.ModifiedBy = CurrentSession.UserId;
                criteria.ModifiedDate = DateTime.Now;

                ICommandResult result = _functionService.Update(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Update

        #region Activation

        /// <summary>
        /// Update function status.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public ActionResult Activate(int id, bool isActive)
        {
            if (Request.IsAjaxRequest())
            {
                var criteria = new UpdateStatusFunctionCriteria
                {
                    FunctionId = id,
                    IsActive = isActive,
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now
                };

                ICommandResult result = _functionService.UpdateStatus(criteria);

                if (result != null && result.IsSuccessful)
                {
                    return Json(Constant.Ok);
                }
            }

            return new EmptyResult();
        }

        #endregion Activation

        #region Privileged Groups And Users

        /// <summary>
        /// Get privileged group and user.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
        public ActionResult PrivilegedGroupsAndUsers(int id = 0)
        {
            var function = _functionService.GetById(id);

            if (function == null)
            {
                return HttpNotFound();
            }

            int siteId = function.Site.Id;

            var users = _userService.GetByFunctionAndSite(id, siteId).ToList();
            var userSiteLevels = users.Select(u => u.User.Id)
                .Distinct()
                .ToDictionary(u => u, u => _userService.GetUserSiteLevel(u, siteId));

            ViewBag.Function = function;
            ViewBag.Users = users;
            ViewBag.Groups = _groupService.GetByFunctionAndSite(id, siteId).ToList();
            ViewBag.UserSiteLevels = userSiteLevels;

            // gets logged-in-user privileges
            ViewFlags.HasAssignFunctionToUserRight =
                HasRightOnSite(siteId, FunctionKeys.AssignFunctionToUser, Constant.Policy.Host);
            ViewFlags.HasAssignFunctionToGroupRight =
                HasRightOnSite(siteId, FunctionKeys.AssignFunctionToGroup, Constant.Policy.Host);

            ViewBag.GroupSelectList = _groupService.GetAllSelectListItem();
            ViewBag.SiteId = siteId.ToString();

            return View();
        }

        /// <summary>
        /// Manage privileged user.
        /// </summary>
        /// <param name="id">The page index.</param>
        /// <param name="size">The page size.</param>
        /// <param name="fid">The function ID.</param>
        /// <param name="gid">The group ID.</param>
        /// <param name="q">The username.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
        public ActionResult PrivilegedUsers(
            int id = 1,
            int size = 10,
            int fid = 0,
            int gid = 0,
            string q = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                IFunction function = _functionService.GetById(fid);

                if (function == null)
                {
                    return HttpNotFound();
                }

                var criteria = new SearchUserExcludeFunctionCriteria
                {
                    PageIndex = id,
                    PageSize = size,
                    Username = q.Trim(),
                    GroupId = gid,
                    SiteId = function.Site.Id,
                    FunctionId = fid,
                    TotalRows = 0
                };

                IQueryable<UserLevel> users = _userService.SearchUserExcludeFunction(criteria);

                var userList = new PagerList<UserLevel>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, users);

                // checks currently-logged-in user's policy on the function
                int adminPolicy = _userService.GetUserFunctionPolicy(CurrentSession.UserId, fid);
                ViewBag.IsReadonlyAssign = adminPolicy < (int)Constant.Policy.Host;

                return View(userList);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Add privileged user.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <param name="readList">List of function which has read permission.</param>
        /// <param name="hostList">List of function which has host permission.</param>
        /// <returns>Result string.</returns>
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
        public string AddPrivilegedUsers(int id = 0, string readList = EmptyString, string hostList = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                int functionId = id;
                var function = _functionService.GetById(functionId);

                if (function == null)
                {
                    throw new ArgumentException(string.Format("Function (id={0}) not found", id));
                }

                var criteria = new AssignFunctionToUsersCriteria
                {
                    FunctionId = id,
                    SiteId = function.Site.Id,
                    UserRead = readList.Split(',').ToList(),
                    UserHost = hostList.Split(',').ToList(),
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now
                };

                ICommandResult result = _functionService.AssignFunctionToUsers(criteria);

                if (result != null && result.IsSuccessful)
                {
                    return Constant.Ok;
                }
            }

            return Constant.Error;
        }

        /// <summary>
        /// Manage privileged group.
        /// </summary>
        /// <param name="fid">The function ID.</param>
        /// <param name="q">The group name.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
        public ActionResult PrivilegedGroups(int fid = 0, string q = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                int functionId = fid;
                string groupName = q.Trim();

                IFunction function = _functionService.GetById(functionId);

                if (function == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<IGroup> group = _groupService.SearchGroupExcludeFunction(groupName, function.Site.Id, fid);

                // checks currently-logged-in user's policy on the function
                var adminPolicy = _userService.GetUserFunctionPolicy(CurrentSession.UserId, fid);
                ViewBag.IsReadonlyAssign = adminPolicy < (int)Constant.Policy.Host;

                return View(group);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Add privileged group.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <param name="readList">List of function which has read permission.</param>
        /// <param name="hostList">List of function which has host permission.</param>
        /// <returns>Result string.</returns>
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
        public string AddPrivilegedGroups(int id = 0, string readList = EmptyString, string hostList = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                int functionId = id;
                IFunction function = _functionService.GetById(functionId);

                if (function == null)
                {
                    throw new ArgumentException(string.Format("Function (id={0}) not found", id));
                }

                var criteria = new AssignFunctionToGroupsCriteria
                {
                    FunctionId = id,
                    SiteId = function.Site.Id,
                    GroupRead = readList.Split(',').ToList(),
                    GroupHost = hostList.Split(',').ToList(),
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now
                };

                ICommandResult result = _functionService.AssignFunctionToGroups(criteria);

                if (result != null && result.IsSuccessful)
                {
                    return Constant.Ok;
                }
            }

            return Constant.Error;
        }

        #endregion Privileged Groups And Users

        #region Ajax Request

        /// <summary>
        /// Get function by site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>JSON result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
        public JsonResult GetFunctionBySite(int siteId)
        {
            if (Request.IsAjaxRequest())
            {
                List<IFunction> functionList = _functionService.GetBySite(siteId, Environment, true).ToList();

                functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                return Json(functionList);
            }

            return Json(null);
        }

        /// <summary>
        /// Get parent function by environment.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="functionId">The function ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>JSON result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public JsonResult GetFunctionParentByEnvironment(int siteId = 0, int functionId = 0, string env = Constant.Uat)
        {
            if (Request.IsAjaxRequest())
            {
                if (functionId != 0)
                {
                    IFunction function = _functionService.GetById(functionId);
                    List<IFunction> functionList = _functionService.GetBySite(function.Site.Id, env, true).ToList();

                    functionList = functionList.Where(f => f.Id != functionId && (f.Parent == null
                                                || f.Parent.Id != functionId)).ToList();

                    functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                    return Json(functionList);
                }
                else
                {
                    var functionList = _functionService.GetBySite(siteId, env, true).ToList();

                    functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                    return Json(functionList);
                }
            }

            return Json(null);
        }

        /// <summary>
        /// Get function key.
        /// </summary>
        /// <param name="functionId">The function ID.</param>
        /// <param name="functionName">The function name.</param>
        /// <returns>JSON result.</returns>
        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public JsonResult GetFunctionKey(int functionId, string functionName)
        {
            IFunction tempFunction = _functionService.GetById(functionId);

            if (object.ReferenceEquals(tempFunction, null))
            {
                return Json(-1);
            }

            // checks Function Name duplication
            IFunction function = _functionService.GetByNameAndSite(functionName, tempFunction.Site.Id);

            if (function != null && function.Id != tempFunction.Id)
            {
                return Json(0);
            }

            string functionKey = ConvertToKey(functionName);

            function = _functionService.GetByKeyAndSite(functionKey, tempFunction.Site.Id);

            if (function != null && function.Id != tempFunction.Id)
            {
                return Json(-2);
            }

            return Json(functionKey);
        }

        #endregion Ajax Request

        #region Helpers

        /// <summary>
        /// Insert child function.
        /// </summary>
        /// <param name="orderedFunctionList">Ordered function list.</param>
        /// <param name="functionList">List of function.</param>
        /// <param name="parent">Parent function.</param>
        private void InsertChildFunction(ref List<Function> orderedFunctionList, List<Function> functionList, Function parent)
        {
            var childFunctionList = functionList.Where(f => f.Parent != null && f.Parent.Id == parent.Id)
                .OrderBy(f => f.DisplayOrder).ToList();

            foreach (Function childFunction in childFunctionList)
            {
                orderedFunctionList.Add(childFunction);
                InsertChildFunction(ref orderedFunctionList, functionList, childFunction);
            }
        }

        #endregion Helpers

        /// <summary>
        /// Get site and functions for add new.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="getSiteList">Get site list or not.</param>
        private void GetSitesAndFunctionsForAddNew(dynamic criteria, bool getSiteList = true)
        {
            // Gets site list
            if (getSiteList)
            {
                // portal admin
                ViewBag.SiteItemList = UserIsPortalAdmin ?
                    this._siteService.GetAllSelectListItem(criteria.Environment, isActive: true, selectedSiteId: criteria.SiteId) :
                    this._siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                            CurrentSession.UserId,
                            FunctionKeys.ManageFunction,
                            criteria.Environment,
                            policy: Constant.Policy.Host,
                            selectedSiteId: criteria.SiteId);
            }

            // Gets function list
            ViewBag.FunctionList = criteria.ParentId == null ?
                this._functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true) :
                this._functionService.GetSelectListItemBySite(
                    criteria.SiteId,
                    criteria.Environment,
                    isActive: true,
                    selectedFunctionId: (int)criteria.ParentId);
        }

        /// <summary>
        /// Prepare manage data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void PrepareManageData(GetBySiteFunctionCriteria criteria)
        {
            ViewBag.FunctionList = _functionService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);

            if (UserIsPortalAdmin)
            {
                ViewBag.SiteItemList = _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
                ViewBag.ViewFlags.HasEditRight = true;
            }
            else
            {
                ViewBag.SiteItemList =
                    _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                        CurrentSession.UserId,
                        FunctionKeys.ManageFunction,
                        criteria.Environment,
                        selectedSiteId: criteria.SiteId);

                ViewFlags.HasEditRight =
                    HasRight(FunctionKeys.ManageFunction, Constant.Policy.Host)
                    && HasRightOnSite(criteria.SiteId, FunctionKeys.ManageFunction, Constant.Policy.Host);
            }
        }
    }
}