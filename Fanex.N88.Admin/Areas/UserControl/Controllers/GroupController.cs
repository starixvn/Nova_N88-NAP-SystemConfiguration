namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Common.Extensions;
    using Fanex.N88.Admin.Areas.UserControl.ViewModels.Group;
    using Fanex.N88.Admin.Configurations;
    using Fanex.N88.Admin.Core.Configuration;
    using Fanex.N88.Admin.Core.Resources;
    using N88.Core.Business;
    using N88.Core.Command;
    using N88.Core.Criteria.Group;
    using N88.Core.Domain;
    using N88.Core.Model.ValueObject;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;
    using N88.Core.Web.Attribute;

    [N88SuperAuth]
    public class GroupController : BaseController
    {
        #region Members

        private readonly IGroupService _groupService;
        private readonly ISiteService _siteService;
        private readonly IFunctionService _functionService;
        private readonly IUserService _userService;
        private readonly string GroupNamePrefix = AppSettings.GetConfigValue("CoreSiteGroupNamePrefix") ?? string.Empty;

        #endregion Members

        #region Ctors

        public GroupController()
            : this(null, null, null, null)
        {
        }

        public GroupController(
            IGroupService groupService,
            ISiteService siteService,
            IFunctionService functionService,
            IUserService userService)
        {
            _groupService = groupService ?? new GroupService { ServiceContext = ServiceContext };
            _siteService = siteService ?? new SiteService { ServiceContext = ServiceContext };
            _functionService = functionService ?? new FunctionService { ServiceContext = ServiceContext };
            _userService = userService ?? new UserService { ServiceContext = ServiceContext };
        }

        #endregion Ctors

        #region Manage

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
        public ActionResult Manage()
        {
            ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageGroup, Constant.Policy.Host);
            ViewFlags.HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToGroup, Constant.Policy.Read);
            ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, Constant.Policy.Read);

            ViewBag.GroupList = _groupService.GetAllWithNumOfUser(string.Empty, Environment);

            return View(new GetAllWithNumOfUserGroupCriteria { IsLicenseeGroup = true });
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
        public ActionResult Manage(GetAllWithNumOfUserGroupCriteria criteria)
        {
            criteria.Environment = Environment;

            ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageGroup, Constant.Policy.Host);
            ViewFlags.HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToGroup, Constant.Policy.Read);
            ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, Constant.Policy.Read);

            ViewBag.GroupList = _groupService.GetAllWithNumOfUser(criteria.GroupName, isLicenseeGroup: criteria.IsLicenseeGroup);

            return View(criteria);
        }

        #endregion Manage

        #region Add New Group

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
        public ActionResult AddNew()
        {
            var sites = _siteService.GetAll(isActive: true);
            var viewModel = new InsertGroupVM
            {
                IsLicenseeGroup = null,
                SiteId = null,
                SiteList = sites.Select(site => new SelectListItem { Value = site.Id.ToString(), Text = site.Name }).ToList(),
            };

            ViewBag.GroupNamePrefix = GroupNamePrefix;
            ViewBag.GroupTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "false", Text = ViewLang.InternalGroup},
                new SelectListItem { Value = "true", Text = ViewLang.LicenseeGroup}
            };
            return View(viewModel);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
        public ActionResult AddNew(InsertGroupVM viewModel)
        {
            CheckValidGroupToInsert(viewModel);

            if (ModelState.IsValid)
            {
                var groupName = (viewModel.IsLicenseeGroup != null && viewModel.IsLicenseeGroup == true) ?
                    viewModel.GroupName : string.Format("{0}{1}", GroupNamePrefix, viewModel.GroupName.Trim());
                var criteria = new InsertGroupCriteria
                {
                    GroupName = groupName,
                    Description = viewModel.Description,
                    IsLicenseeGroup = viewModel.IsLicenseeGroup != null && viewModel.IsLicenseeGroup == true,
                    SiteId = viewModel.SiteId,
                    IsActive = viewModel.IsActive,
                    CreatedBy = CurrentSession.UserId,
                    CreatedDate = DateTime.Now,
                };
                ICommandResult result = _groupService.Insert(criteria);

                var sites = _siteService.GetAll(isActive: true);

                viewModel.SiteList = sites.Select(site => new SelectListItem { Value = site.Id.ToString(), Text = site.Name }).ToList();
                ViewBag.GroupTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "false", Text = ViewLang.InternalGroup},
                    new SelectListItem { Value = "true", Text = ViewLang.LicenseeGroup}
                };

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.GroupName));
                    return View(viewModel);
                }

                ViewBag.Success = true;
            }

            return View(viewModel);
        }

        private void CheckValidGroupToInsert(InsertGroupVM viewModel)
        {
            if (viewModel.IsLicenseeGroup == null)
            {
                ModelState.AddModelError("IsLicenseeGroup", string.Format(ViewLang.FieldRequired, ViewLang.GroupType));
            }

            if (viewModel.IsLicenseeGroup != null && viewModel.IsLicenseeGroup == true && (viewModel.SiteId == null || viewModel.SiteId == 0))
            {
                ModelState.AddModelError("SiteId", string.Format(ViewLang.FieldRequired, ViewLang.Site));
            }
        }

        #endregion Add New Group

        #region Edit Group

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            IGroup group = _groupService.GetById(id);

            if (object.ReferenceEquals(group, null))
            {
                return RedirectToAction("Manage");
            }

            var criteria = new UpdateGroupCriteria
            {
                GroupId = group.Id,
                Description = group.Description,
                GroupName = group.Name,
                IsActive = group.IsActive
            };

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
        public ActionResult Edit(UpdateGroupCriteria criteria)
        {
            if (ModelState.IsValid)
            {
                var currentGroup = _groupService.GetById(criteria.GroupId);
                criteria.ModifiedBy = CurrentSession.UserId;
                criteria.ModifiedDate = DateTime.Now;
                criteria.IsLicenseeGroup = currentGroup.IsLicenseeGroup;
                criteria.SiteId = currentGroup.SourceSiteId;

                ICommandResult result = _groupService.Update(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
                else
                {
                    ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.GroupName));

                    return View(criteria);
                }
            }

            return View(criteria);
        }

        #endregion Edit Group

        #region Activation

        /// <summary>
        /// Update group status.
        /// </summary>
        /// <param name="id">The group ID.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
        public ActionResult Activate(int id, bool isActive)
        {
            if (Request.IsAjaxRequest())
            {
                var criteria = new UpdateStatusGroupCriteria
                {
                    GroupId = id,
                    IsActive = isActive,
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now
                };

                ICommandResult result = _groupService.UpdateStatus(criteria);

                if (result != null && result.IsSuccessful)
                {
                    return Json(Constant.Ok);
                }
            }

            return new EmptyResult();
        }

        #endregion Activation

        #region Assign Permission

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Read)]
        public ActionResult AssignPermission(int id, int siteId = 0)
        {
            var siteIdValue = siteId == 0 || SiteId != N88AdminId ? SiteId : siteId;

            var group = _groupService.GetById(id);
            var site = _siteService.GetById(siteIdValue);

            if (group == null || site == null)
            {
                return RedirectToAction("Manage");
            }

            var criteria = new AssignFunctionGroupCriteria
            {
                GroupId = group.Id,
                GroupName = group.Name,
                SiteId = site.Id
            };

            ViewFlags.HasAssignFunctionRight =
                HasRightOnSite(criteria.SiteId, FunctionKeys.AssignFunctionToGroup, Constant.Policy.Host);

            var siteList = new List<SelectListItem>();
            if (group.IsLicenseeGroup)
            {
                siteList.Add(new SelectListItem { Value = site.Id.ToString(), Text = site.Name, Selected = true });
            }
            else
            {
                siteList = CurrentSession.Level == (int)Constant.Level.PortalAdmin ?
                               _siteService.GetAllSelectListItem(Environment, true, criteria.SiteId).ToList() :
                               _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(CurrentSession.UserId,
                                                                                               FunctionKeys.AssignFunctionToGroup,
                                                                                               Environment,
                                                                                               selectedSiteId: criteria.SiteId).ToList();
            }

            ViewBag.SiteSelectList = siteList;

            ViewBag.FunctionList = _functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true);
            ViewBag.GroupFuncPolicyList =
                _functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment);
            ViewBag.UserFuncPolicyList =
                _functionService.GetByUserAndSiteIncludeGroup(CurrentSession.UserId, criteria.SiteId, Environment);
            ViewBag.UserLevel = CurrentSession.Level;

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
#pragma warning disable S1541 // Methods and properties should not be too complex
        public ActionResult AssignPermission(AssignFunctionGroupCriteria criteria)
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            IGroup group = _groupService.GetById(criteria.GroupId);
            ISite site = _siteService.GetById(criteria.SiteId);

            if (group == null || site == null)
            {
                return RedirectToAction("Manage");
            }

            // check policy
            int policy = _userService.GetUserSiteFunctionKeyPolicy(
                CurrentSession.UserId,
                criteria.SiteId,
                FunctionKeys.AssignFunctionToGroup);

            if (policy < (int)Constant.Policy.Host)
            {
                return RedirectToAction("Unauthorize", "Common");
            }

            ViewFlags.HasAssignFunctionRight = policy >= (int)Constant.Policy.Host;

            criteria.ModifiedBy = CurrentSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            // list function policy
            List<IFunction> listFunc =
                _functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();

            // list group function policy
            List<FunctionPolicy> listGroupFuncPolicy =
                _functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment).ToList();

            // list function will be remove by current user login
            var listGroupFuncRemove = new List<FunctionPolicy>();
            listGroupFuncRemove.AddRange(listGroupFuncPolicy);

            // list function policy of current user login
            List<FunctionPolicy> listCurrentUserFuncPolicy =
                _functionService.GetByUserAndSiteIncludeGroup(CurrentSession.UserId, criteria.SiteId, Environment).ToList();

            // check function host,contains in list function
            if (!criteria.FuncHost.IsNullOrDBNull())
            {
                foreach (int itemFunc in criteria.FuncHost)
                {
                    // function find from list function, policy is host
                    if (!listFunc.Any(f => f.Id == itemFunc))
                    {
                        return RedirectToAction("Error", "Common");
                    }

                    listGroupFuncRemove.Remove(f => f.Function.Id == itemFunc);
                }
            }

            // check function read, with current user login have permission
            if (!criteria.FuncRead.IsNullOrDBNull())
            {
                foreach (int itemFunc in criteria.FuncRead)
                {
                    listGroupFuncRemove.Remove(f => f.Function.Id == itemFunc);
                }
            }

            // check permission current user login with group be assign permission
            // check for function policy will be remove ( set policy = 0 )
            listGroupFuncRemove.Remove(f => f.Policy > 0);
            foreach (FunctionPolicy itemFunc in listGroupFuncRemove)
            {
                if (!listCurrentUserFuncPolicy.Any(f => f.Function.Id == itemFunc.Function.Id && f.Policy >= itemFunc.Policy))
                {
                    return RedirectToAction("Error", "Common");
                }
            }

            // assign permission
            ICommandResult result = _groupService.AssignFunction(criteria);

            if (result != null && result.IsSuccessful)
            {
                ViewBag.Success = true;

                listGroupFuncPolicy =
                    _functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment).ToList();
            }

            ViewBag.SiteSelectList = UserIsPortalAdmin ?
                _siteService.GetAllSelectListItem(Environment, true, criteria.SiteId) :
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignFunctionToGroup,
                    Environment,
                    selectedSiteId: criteria.SiteId);

            ViewBag.FunctionList = listFunc;
            ViewBag.GroupFuncPolicyList = listGroupFuncPolicy;
            ViewBag.UserFuncPolicyList = listCurrentUserFuncPolicy;
            ViewBag.UserLevel = CurrentSession.Level;

            return View(criteria);
        }

        #endregion Assign Permission

        #region UnassignPermission

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
        public ActionResult UnassignPermission(int groupId, int functionId)
        {
            // check group and site
            var function = _functionService.GetById(functionId);
            IGroup group = _groupService.GetById(groupId);
            ISite site = function.Site;
            List<FunctionPolicy> listGroupFuncPolicy = _functionService.GetByGroupAndSite(groupId, site.Id, Environment).ToList();

            var criteria = new AssignFunctionGroupCriteria
            {
                FuncHost = listGroupFuncPolicy.Where(p => p.Policy == 2 && p.Function.Id != functionId).Select(p => (int)p.Function.Id).ToArray(),
                FuncRead = listGroupFuncPolicy.Where(p => p.Policy == 1 && p.Function.Id != functionId).Select(p => (int)p.Function.Id).ToArray(),
                ModifiedBy = CurrentSession.UserId,
                ModifiedDate = DateTime.Now,
                GroupId = groupId,
                GroupName = group.Name,
                SiteId = site.Id
            };

            ICommandResult result = _groupService.AssignFunction(criteria);

            return Json(result);
        }

        #endregion UnassignPermission

        #region Group Info

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
        public ActionResult GroupInfo(int id)
        {
            return View(_groupService.GetById(id));
        }

        #endregion Group Info

        [HttpGet]
        public ActionResult GetGroupByTypeAndSite(bool isLicensee, int? siteId)
        {
            var groups = _groupService.GetAll(isLicenseeGroup: isLicensee, siteId: siteId);
            return Json(groups, JsonRequestBehavior.AllowGet);
        }
    }
}