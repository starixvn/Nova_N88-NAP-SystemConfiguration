// --------------------------------------------------------------------------
// <copyright file="MenuItemService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.16">Created</change>
//   <change who="Edward.Nguyen" date="2012.8.30">Implement GetParentById</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.MenuItem;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Repository.Impl;
    using Fanex.NAP.Core.Repository.Interface;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Menu item service.
    /// </summary>
    public class MenuItemService : BaseService, IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService()
            : this(null)
        {
        }

        public MenuItemService(IMenuItemRepository menuItemRepository = null)
        {
            this._menuItemRepository = menuItemRepository ?? new MenuItemRepository();
        }

        /// <summary>
        /// Get menu item by ID.
        /// </summary>
        /// <param name="id">The menu item ID.</param>
        /// <returns>The menu item object.</returns>
        public IMenuItem GetById(int id)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(id, "Id");

            var criteria = new GetByIdMenuItemCriteria
            {
                MenuItemId = id
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get menu item by name and site.
        /// </summary>
        /// <param name="name">The menu item name.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>The menu item object.</returns>
        public IMenuItem GetByNameAndSite(string name, int siteId)
        {
            // check parameters
            Check.Argument.IsNotEmpty(name, "Name");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");

            var criteria = new GetByNameAndSiteMenuItemCriteria
            {
                MenuItemName = name,
                SiteId = siteId
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get all menu item.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> GetAll(string env = Constant.Uat, bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllMenuItemCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get all menu item select list item.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <param name="selectedMenuItemId">Selected menu item ID.</param>
        /// <returns>The list of select list item.</returns>
        public IList<SelectListItem> GetAllSelectListItem(
            string env = Constant.Uat,
            bool? isActive = null,
            int selectedMenuItemId = 0)
        {
            // check parameters
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllMenuItemCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IMenuItem> menuItemList = this._menuItemRepository.Fetch(criteria);

                if (menuItemList != null)
                {
                    return menuItemList.Select(s => new SelectListItem
                        {
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = s.Id == selectedMenuItemId
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get menu item by site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> GetBySite(int siteId, string env = Constant.Uat, bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteMenuItemCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get menu item select list item by site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <param name="selectedMenuItemId">Selected menu item ID.</param>
        /// <returns>The list of menu item object.</returns>
        public IList<SelectListItem> GetSelectListItemBySite(
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null,
            int selectedMenuItemId = 0)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteMenuItemCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IMenuItem> menuItemList = this._menuItemRepository.Fetch(criteria);

                if (menuItemList != null)
                {
                    return menuItemList.Select(s => new SelectListItem
                        {
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = s.Id == selectedMenuItemId
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get menu item by site in hierarchy.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> GetBySiteInHierarchy(int siteId, string env = Constant.Uat, bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteMenuItemCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IMenuItem> menuItemList = this._menuItemRepository.Fetch(criteria).OrderBy(x => x.DisplayOrder);

                return this.SortMenuItemsInHierarchy(menuItemList);
            }

            return null;
        }

        /// <summary>
        /// Get parent menu item by ID.
        /// </summary>
        /// <param name="id">The menu item ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> GetParentById(int id, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(id, "Id");

            var criteria = new GetParentByIdMenuItemCriteria
            {
                MenuItemId = id,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get parent menu item select list item by ID.
        /// </summary>
        /// <param name="id">The menu item ID.</param>
        /// <param name="selectedMenuItemId">Selected menu item ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of select list item.</returns>
        public IList<SelectListItem> GetParentSelectListItemById(
            int id, 
            int selectedMenuItemId = 0, 
            string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(id, "Id");

            var criteria = new GetParentByIdMenuItemCriteria
            {
                MenuItemId = id,
                Environment = env
            };

            if (criteria.IsValid())
            {
                IEnumerable<IMenuItem> menuItemList = this._menuItemRepository.Fetch(criteria);

                if (menuItemList != null)
                {
                    return menuItemList.Select(s => new SelectListItem
                        {
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = s.Id == selectedMenuItemId
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get menu item by user and site include group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> GetByUserAndSiteIncludeGroup(
            int userId,
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserAndSiteIncludeGroupMenuItemCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Search menu item.
        /// </summary>
        /// <param name="menuItemName">Menu item name.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of menu item object.</returns>
        public IEnumerable<IMenuItem> Search(
            string menuItemName,
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new SearchAllMenuItemCriteria
            {
                MenuItemName = menuItemName,
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._menuItemRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Insert menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Insert(InsertMenuItemCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._menuItemRepository.Insert(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Update(UpdateMenuItemCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._menuItemRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update menu item status.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateStatus(UpdateStatusMenuItemCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._menuItemRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Sort menu item in hierarchy.
        /// </summary>
        /// <param name="menuItemList">List of menu item.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>The list of menu item object.</returns>
        private IEnumerable<IMenuItem> SortMenuItemsInHierarchy(IEnumerable<IMenuItem> menuItemList, int parentId = 0)
        {
            var sortedMenuItemList = new List<IMenuItem>();

            foreach (var menuItem in menuItemList)
            {
                // top menu items || sub menu items
                if ((menuItem.Parent == null && parentId == 0)
                    || (menuItem.Parent != null && menuItem.Parent.Id == parentId))
                {
                    sortedMenuItemList.Add(menuItem);
                    sortedMenuItemList.AddRange(this.SortMenuItemsInHierarchy(menuItemList, menuItem.Id));
                }
            }

            return sortedMenuItemList;
        }
    }
}