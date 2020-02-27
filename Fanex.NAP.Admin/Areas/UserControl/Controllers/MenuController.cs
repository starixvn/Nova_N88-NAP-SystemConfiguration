namespace Fanex.NAP.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Configuration;
    using NAP.Core.Business;
    using NAP.Core.Command;
    using NAP.Core.Criteria.MenuItem;
    using NAP.Core.Domain;
    using NAP.Core.Model;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web;
    using NAP.Core.Web.Attribute;

    /// <summary>
    /// Menu controller.
    /// </summary>
    [NapSuperAuth]
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

        /// <summary>
        /// Manage menu.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
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

        /// <summary>
        /// Manage menu.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
        public ActionResult Manage(GetBySiteMenuItemCriteria criteria)
        {
            PrepareManageData(criteria);

            return View(criteria);
        }

        #endregion Manage Menu item

        #region Add New Menu

        /// <summary>
        /// Add new menu item.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
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

        /// <summary>
        /// Add new menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult AddNewMenuItem(InsertMenuItemCriteria criteria)
        {
            PrepareAddNewData(criteria);

            if (ModelState.IsValid)
            {
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

        /// <summary>
        /// Update menu item.
        /// </summary>
        /// <param name="menuItemId">Menu item ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
        public ActionResult EditMenuItem(int menuItemId)
        {
            IMenuItem menuItem = _menuItemService.GetById(menuItemId);

            if (menuItem == null)
            {
                return RedirectToAction("Manage");
            }

            var criteria = new UpdateMenuItemCriteria();

            criteria.MenuItemId = menuItem.Id;
            criteria.MenuItemName = menuItem.Name;
            criteria.Description = menuItem.Description;
            criteria.DisplayOrder = menuItem.DisplayOrder;
            criteria.FunctionId = menuItem.Function.Id;
            criteria.IsActive = menuItem.IsActive;
            criteria.ParentId = menuItem.Parent != null ? menuItem.Parent.Id : 0;
            criteria.ParentName = menuItem.Parent != null ? menuItem.Parent.Name : string.Empty;
            criteria.Policy = menuItem.Policy;
            criteria.SiteId = menuItem.Site.Id;
            criteria.SiteName = menuItem.Site.Name;
            criteria.Url = menuItem.Url;
            criteria.Environment = menuItem.Environment;

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

        /// <summary>
        /// Update menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
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

        /// <summary>
        /// Get list menu item by site id.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="menuItemId">The menu item ID.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
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

        /// <summary>
        /// Get list function by site id.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
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

        /// <summary>
        /// Update status menu.
        /// </summary>
        /// <param name="id">Menu item ID.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
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

        #endregion AjaxRequest

        /// <summary>
        /// Prepare manage data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void PrepareManageData(GetBySiteMenuItemCriteria criteria)
        {
            if (CurrentSession.Level == (int)Constant.Level.PortalAdmin)
            {
                ViewBag.Site = _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
                ViewFlags.HasEditRight = true;
            }
            else
            {
                ViewBag.Site =
                    _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
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