// --------------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface IUserService : IBaseService
    {
        IUser GetById(int id);

        IUser GetByUsername(string username);

        bool CheckUsernameExist(string username);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed")]
        IEnumerable<IUser> GetAll();

        IEnumerable<UserPolicy> GetByFunctionAndSite(int functionId, int siteId);

        IEnumerable<IUser> GetBySite(int siteId = 0, bool? isActive = null);

        IEnumerable<IUser> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null);

        IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

        IQueryable<UserLevel> SearchUser(SearchUserCriteria criteria);

        IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

        IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria);

        int GetUserSiteLevel(int userId, int siteId);

        int GetUserFunctionPolicy(int userId, int functionId);

        int GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey);

        bool HasPermissionUserSiteFunctionKey(int userId, int siteId, string functionKey, Constant.Policy policy);

        /// <summary>
        /// Get user list site function key policy.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="listSite">List of site.</param>
        /// <returns>Key is site id and value is policy is host.</returns>
        Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite = null);

        ICommandResult ResetPassword(IUser user);

        ICommandResult Insert(IUser user);

        ICommandResult Insert(InsertCustCriteria criteria);

        ICommandResult Update(int userId, string email, string displayName);

        ICommandResult Update(IUser user);

        ICommandResult ChangeUserSiteStatus(int userId, int siteId, bool isActive);

        ICommandResult AssignSite(AssignSiteUserCriteria criteria);

        ICommandResult UpdateSite(UpdateSiteToUserCriteria criteria);

        ICommandResult AssignFunction(AssignFunctionUserCriteria criteria);

        ICommandResult ChangePassword(ICriteria criteria);

        ICommandResult LoginOnUser(string netSessionId, int serverId);

        ICommandResult Logout();

        ICommandResult ReLogin();

        void UserCheckIn(UserCheckInUserCriteria criteria);

        void ReUserCheckIn(UserReCheckInUserCriteria criteria);

        void UserLogOut(LogOutUserCriteria criteria);

        void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria);

        ICommandResult InsertOrUpdate(ICriteria criteria);

        string UserSSOGetTime();
    }
}
