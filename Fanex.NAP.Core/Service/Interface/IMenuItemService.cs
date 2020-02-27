// --------------------------------------------------------------------------
// <copyright file="IMenuItemService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.MenuItem;
    using Fanex.NAP.Core.Domain;

    public interface IMenuItemService : IBaseService
    {
        IMenuItem GetById(int id);

        IMenuItem GetByNameAndSite(string name, int siteId);

        IEnumerable<IMenuItem> GetAll(string env = Constant.Uat, bool? isActive = null);

        IList<SelectListItem> GetAllSelectListItem(
            string env = Constant.Uat,
            bool? isActive = null,
            int selectedMenuItemId = 0);

        IEnumerable<IMenuItem> GetBySite(int siteId, string env = Constant.Uat, bool? isActive = null);

        IList<SelectListItem> GetSelectListItemBySite(
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null,
            int selectedMenuItemId = 0);

        IEnumerable<IMenuItem> GetBySiteInHierarchy(int siteId, string env = Constant.Uat, bool? isActive = null);

        IEnumerable<IMenuItem> GetByUserAndSiteIncludeGroup(
            int userId,
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null);

        IEnumerable<IMenuItem> Search(
            string menuItemName,
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null);

        IEnumerable<IMenuItem> GetParentById(int id, string env = Constant.Uat);

        IList<SelectListItem> GetParentSelectListItemById(int id, int selectedMenuItemId = 0, string env = Constant.Uat);

        ICommandResult Insert(InsertMenuItemCriteria criteria);

        ICommandResult Update(UpdateMenuItemCriteria criteria);

        ICommandResult UpdateStatus(UpdateStatusMenuItemCriteria criteria);
    }
}