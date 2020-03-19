namespace Fanex.N88.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.N88.Admin.Core.Configuration;
    using N88.Core.Business;
    using N88.Core.Command;
    using N88.Core.Criteria.MenuItem;
    using N88.Core.Domain;
    using N88.Core.Model;
    using N88.Core.Service.Impl;
    using N88.Core.Service.Interface;
    using N88.Core.Web;
    using N88.Core.Web.Attribute;

    [N88SuperAuth]
    public class MenuController : BaseController
    {
        #region Properties

        private readonly IMenuItemService _menuItemService;
        private readonly ISiteService _siteService;
        private readonly IFunctionService _functionService;

        #endregion Properties

        #region Contructors

        public MenuController()
            : this(null, null, null)
        {
        }

        public MenuController(
            IMenuItemService menuitemService,
            ISiteService siteService,
            IFunctionService functionService)
        {
            _menuItemService = menuitemService ?? new MenuItemService { ServiceContext = ServiceContext };
            _siteService = siteService ?? new SiteService { ServiceContext = ServiceContext };
            _functionService = functionService ?? new FunctionService { ServiceContext = ServiceContext };
        }

        #endregion Contructors

        #region Manage Menu item

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
        public ActionResult Manage(int? siteId)
        {
            var criteria = new GetBySiteMenuItemCriteria
            {
                SiteId = siteId ?? SiteId,
                Environment = Environment
            };

            PrepareManageData(criteria);

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
        public ActionResult Manage(GetBySiteMenuItemCriteria criteria)
        {
            PrepareManageData(criteria);

            return View(criteria);
        }

        #endregion Manage Menu item

        #region Add New Menu

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult AddNewMenuItem(int? siteId = null)
        {
            var criteria = new InsertMenuItemCriteria
            {
                Environment = Environment,
                Policy = (int)Constant.Policy.Read,
                SiteId = siteId ?? SiteId
            };

            PrepareAddNewData(criteria);

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult AddNewMenuItem(InsertMenuItemCriteria criteria)
        {
            PrepareAddNewData(criteria);

            if (ModelState.IsValid)
            {
                // check duplicate key
                criteria.MenuItemKey = ConvertToKey(criteria.MenuItemKey);
                IMenuItem menu = _menuItemService.GetByKeyAndSite(criteria.MenuItemKey, criteria.SiteId);

                if (menu != null)
                {
                    ModelState.AddModelError(nameof(criteria.MenuItemKey), string.Format(ViewResources.DuplicateMsg, criteria.MenuItemKey));
                    return View(criteria);
                }

                // Add new
                criteria.CreatedBy = CurrentSession.UserId;
                criteria.CreatedDate = DateTime.Now;
                criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
                criteria.ParentName = criteria.ParentId == 0 || criteria.ParentId == null ? null : criteria.ParentName;

                ICommandResult result = _menuItemService.Insert(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Add New Menu

        #region Edit Menu Item

        [HttpGet]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult EditMenuItem(int menuItemId)
        {
            IMenuItem menuItem = _menuItemService.GetById(menuItemId);

            if (menuItem == null)
            {
                return RedirectToAction("Manage");
            }

            var criteria = new UpdateMenuItemCriteria
            {
                MenuItemId = menuItem.Id,
                MenuItemName = menuItem.Name,
                MenuItemKey = menuItem.Key,
                Description = menuItem.Description,
                DisplayOrder = menuItem.DisplayOrder,
                FunctionId = menuItem.Function.Id,
                IsActive = menuItem.IsActive,
                ParentId = menuItem.Parent != null ? menuItem.Parent.Id : 0,
                ParentName = menuItem.Parent != null ? menuItem.Parent.Name : string.Empty,
                Policy = menuItem.Policy,
                SiteId = menuItem.Site.Id,
                SiteName = menuItem.Site.Name,
                Url = menuItem.Url,
                Environment = menuItem.Environment
            };

            ViewBag.Function = _functionService.GetSelectListItemBySite(
                criteria.SiteId,
                criteria.Environment,
                isActive: true,
                selectedFunctionId: criteria.FunctionId);

            ViewBag.Parent = criteria.ParentId == null ?
                _menuItemService.GetParentSelectListItemById(id: criteria.MenuItemId, env: criteria.Environment) :
                _menuItemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId, criteria.Environment);

            return View(criteria);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult EditMenuItem(UpdateMenuItemCriteria criteria)
        {
            ViewBag.Function = _functionService.GetSelectListItemBySite(
                criteria.SiteId,
                criteria.Environment,
                isActive: true,
                selectedFunctionId: criteria.FunctionId);

            ViewBag.Parent = criteria.ParentId == null ?
                _menuItemService.GetParentSelectListItemById(criteria.MenuItemId) :
                _menuItemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(criteria.MenuItemKey))
                {
                    criteria.MenuItemKey = ConvertToKey(criteria.MenuItemKey);
                }

                // check duplicate key
                IMenuItem menu = _menuItemService.GetByKeyAndSite(criteria.MenuItemKey, criteria.SiteId);
                if (menu != null && menu.Id != criteria.MenuItemId)
                {
                    ModelState.AddModelError(nameof(criteria.MenuItemKey), string.Format(ViewResources.DuplicateMsg, criteria.MenuItemKey));
                    return View(criteria);
                }

                // Update menu item
                criteria.ModifierId = CurrentSession.UserId;
                criteria.ModifiedDate = DateTime.Now;
                criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
                criteria.ParentName = criteria.ParentId == 0 || criteria.ParentId == null ? null
                    : criteria.ParentName;

                ICommandResult result = _menuItemService.Update(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Edit Menu Item

        #region AjaxRequest

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult ListMenuItemBySite(int? siteId, int? menuItemId, string environment = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                var menuItemList = new List<IMenuItem>();

                var environmentValue = !string.IsNullOrEmpty(environment) ? environment : Environment;
                menuItemList.Add(new MenuItem { Id = 0, Name = ViewResources.None });

                menuItemList.AddRange(menuItemId != null ?
                    _menuItemService.GetParentById((int)menuItemId, env: environmentValue).ToList() :
                    _menuItemService.GetBySite((int)siteId, environmentValue).ToList());

                return Json(menuItemList);
            }

            return Json(new List<IMenuItem>());
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult ListFunctionBySite(int siteId, string environment = EmptyString)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(_functionService.GetBySite(
                    siteId,
                    !string.IsNullOrEmpty(environment) ? environment : Environment,
                    isActive: true));
            }

            return Json(null);
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult Activate(int id, bool isActive)
        {
            if (Request.IsAjaxRequest())
            {
                var criteria = new UpdateStatusMenuItemCriteria
                {
                    IsActive = isActive,
                    MenuItemId = id,
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now
                };

                ICommandResult result = _menuItemService.UpdateStatus(criteria);
                if (result != null && result.IsSuccessful)
                {
                    return Json(Constant.Ok);
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        [N88Auth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
        public JsonResult GetMenuItemKey(int menuItemId, string menuItemName)
        {
            IMenuItem tempMenu = _menuItemService.GetById(menuItemId);
            if (object.ReferenceEquals(tempMenu, null))
            {
                return Json(new { IsSuccessful = false, Message = "Menu is not existed.", MenuItemKey = string.Empty });
            }

            // check duplicate key
            string menuItemKey = ConvertToKey(menuItemName);
            IMenuItem menu = _menuItemService.GetByKeyAndSite(menuItemKey, tempMenu.Site.Id);
            if (menu != null && menu.Id != tempMenu.Id)
            {
                return Json(new { IsSuccessful = false, Message = string.Format("{0} has already existed.", menuItemKey), MenuItemKey = string.Empty });
            }

            return Json(new { IsSuccessful = true, Message = string.Empty, MenuItemKey = menuItemKey });
        }

        #endregion AjaxRequest

        private void PrepareManageData(GetBySiteMenuItemCriteria criteria)
        {
            if (CurrentSession.Level == (int)Constant.Level.PortalAdmin)
            {
                ViewBag.Site = _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
                ViewFlags.HasEditRight = true;
            }
            else
            {
                ViewBag.Site = _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.ManageMenu,
                    criteria.Environment,
                    selectedSiteId: criteria.SiteId);
                ViewFlags.HasEditRight =
                    HasRight(FunctionKeys.ManageMenu, Constant.Policy.Host)
                    && HasRightOnSite(criteria.SiteId, FunctionKeys.ManageMenu, Constant.Policy.Host);
            }

            ViewBag.MenuItemList = _menuItemService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);
        }

        private void PrepareAddNewData(InsertMenuItemCriteria criteria)
        {
            ViewBag.Parent = _menuItemService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);
            ViewBag.Function = _functionService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);

            ViewBag.Site = CurrentSession.Level == (int)Constant.Level.PortalAdmin ?
                _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId) :
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.ManageMenu,
                    criteria.Environment,
                    policy: Constant.Policy.Host,
                    selectedSiteId: criteria.SiteId);
        }
    }
}