// --------------------------------------------------------------------------
// <copyright file="GroupService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.15">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Group;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Impl;
    using Fanex.NAP.Core.Repository.Interface;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Group service.
    /// </summary>
    public class GroupService : BaseService, IGroupService
    {
        private IGroupRepository _groupRepository;

        public GroupService()
            : this(null)
        {
        }

        public GroupService(IGroupRepository groupRepository)
        {
            this._groupRepository = groupRepository ?? new GroupRepository();
        }

        /// <summary>
        /// Get user group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The group object.</returns>
        public IGroup GetByUser(int userId)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");

            var criteria = new GetByUserGroupCriteria
            {
                UserId = userId
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get group by ID.
        /// </summary>
        /// <param name="id">The group ID.</param>
        /// <returns>The group object.</returns>
        public IGroup GetById(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");

            var criteria = new GetByIdGroupCriteria
            {
                GroupId = id
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get group by name.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The group object.</returns>
        public IGroup GetByName(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");

            var criteria = new GetByNameGroupCriteria
            {
                GroupName = name
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get all group.
        /// </summary>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of group object.</returns>
        public IEnumerable<IGroup> GetAll(bool? isActive = null)
        {
            var criteria = new GetAllGroupCriteria
            {
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Fetch(criteria).OrderBy(g => g.Name);
            }

            return null;
        }

        /// <summary>
        /// Get all group select list item.
        /// </summary>
        /// <param name="isActive">Is active or not.</param>
        /// <param name="selectedGroupId">Selected group ID.</param>
        /// <returns>The list of select list item.</returns>
        public IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0)
        {
            var criteria = new GetAllGroupCriteria
            {
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IGroup> groupList = this._groupRepository.Fetch(criteria).OrderBy(g => g.Name);

                if (groupList != null)
                {
                    return groupList.Select(g => new SelectListItem
                        {
                            Text = g.Name,
                            Value = g.Id.ToString(),
                            Selected = g.Id == selectedGroupId
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get all group with number of user.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of group object.</returns>
        public IEnumerable<IGroup> GetAllWithNumOfUser(string name, string env = Constant.Uat, bool? isActive = null)
        {
            var criteria = new GetAllWithNumOfUserGroupCriteria
            {
                IsActive = isActive,
                GroupName = name,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Fetch(criteria).OrderBy(g => g.Name);
            }

            return null;
        }

        /// <summary>
        /// Assign group permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult AssignFunction(AssignFunctionGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._groupRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update group status.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateStatus(UpdateStatusGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._groupRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Insert group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Insert(InsertGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._groupRepository.Insert(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Update(UpdateGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._groupRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Get group by function and site.
        /// </summary>
        /// <param name="functionId">The function ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>List of group policy.</returns>
        public IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(functionId, "functionid");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteid");

            var criteria = new GetByFunctionAndSiteGroupCriteria
            {
                FunctionId = functionId,
                SiteId = siteId
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.GetGroupPolicy(criteria).OrderBy(g => g.Group.Name);
            }

            return null;
        }

        /// <summary>
        /// Search group exclude function.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="functionId">The function ID.</param>
        /// <returns>The list of group object.</returns>
        public IEnumerable<IGroup> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0)
        {
            var criteria = new SearchGroupExcludeFunctionCriteria
            {
                GroupName = groupName,
                SiteId = siteId,
                FunctionId = functionId
            };

            if (criteria.IsValid())
            {
                return this._groupRepository.Fetch(criteria).OrderBy(g => g.Name);
            }

            return null;
        }
    }
}