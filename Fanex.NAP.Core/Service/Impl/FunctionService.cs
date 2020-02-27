// --------------------------------------------------------------------------
// <copyright file="FunctionService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.16">Created</change>
//   <change who="Edward.Nguyen" date="2012.8.7">Implemented</change>
//   <change who="Lewis.Le" date="2012.11.01">Added ICommandResult AssignFunction(AssignFunctionToUsersCriteria criteria)</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Function;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Impl;
    using Fanex.NAP.Core.Repository.Interface;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Function service.
    /// </summary>
    public class FunctionService : BaseService, IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;

        #region Ctors

        public FunctionService() : this(null)
        {
        }

        public FunctionService(IFunctionRepository functionRepository = null)
        {
            this._functionRepository = functionRepository ?? new FunctionRepository();
        }

        #endregion

        #region Gets Function

        /// <summary>
        /// Get function by ID.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <returns>The function object.</returns>
        public IFunction GetById(int id)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(id, "Id");

            var criteria = new GetByIdFunctionCriteria
            {
                FunctionId = id
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function by name and site.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>The function object.</returns>
        public IFunction GetByNameAndSite(string name, int siteId)
        {
            // check parameters
            Check.Argument.IsNotEmpty(name, "Name");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");

            var criteria = new GetByNameAndSiteFunctionCriteria
            {
                FunctionName = name,
                SiteId = siteId
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function by key and site.
        /// </summary>
        /// <param name="key">Function key.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>The function object.</returns>
        public IFunction GetByKeyAndSite(string key, int siteId)
        {
            // check parameters
            Check.Argument.IsNotEmpty(key, "Key");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");

            var criteria = new GetByKeyAndSiteFunctionCriteria
            {
                FunctionKey = key,
                SiteId = siteId
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Get(criteria);
            }

            return null;
        }

        #endregion

        #region Get Functions

        /// <summary>
        /// Get all function.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> GetAll(string env = Constant.Uat, bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllFunctionCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get all function and group by site.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>List of function grouped by site.</returns>
        public Dictionary<ISite, IEnumerable<IFunction>> GetAllAndGroupBySite(
            string env = Constant.Uat,
            bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllFunctionCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IFunction> functionList = this._functionRepository.Fetch(criteria);

                var result = new Dictionary<ISite, IEnumerable<IFunction>>();

                var siteList = functionList.GroupBy(f => f.Site.Id)
                    .Select(s => new Site
                    {
                        Id = s.Key,
                        Name = s.Where(f => f.Site.Id == s.Key).FirstOrDefault().Site.Name
                    }).ToList();

                foreach (var site in siteList)
                {
                    var subFunctionList = functionList.Where(f => f.Site.Id == site.Id).ToList();

                    result.Add(site, subFunctionList);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Get function by site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> GetBySite(int siteId, string env = Constant.Uat, bool? isActive = null)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteFunctionCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get select list item by site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <param name="selectedFunctionId">Selected function ID.</param>
        /// <returns>List of select list item.</returns>
        public IList<SelectListItem> GetSelectListItemBySite(
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null, 
            int selectedFunctionId = 0)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteFunctionCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IFunction> functionList = this._functionRepository.Fetch(criteria);

                if (functionList != null)
                {
                    return functionList.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString(),
                        Selected = s.Id == selectedFunctionId
                    }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get function by site in hierarchy.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> GetBySiteInHierarchy(int siteId, string env = Constant.Uat, bool? isActive = null)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetBySiteFunctionCriteria
            {
                SiteId = siteId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<IFunction> functionList = this._functionRepository.Fetch(criteria).OrderBy(x => x.DisplayOrder);

                return this.SortFunctionsInHierarchy(functionList);
            }

            return null;
        }

        /// <summary>
        /// Get parent function by ID.
        /// </summary>
        /// <param name="id">The function ID.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> GetParentById(int id)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(id, "Id");

            var criteria = new GetParentByIdFunctionCriteria
            {
                FunctionId = id
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.Fetch(criteria);
            }

            return null;
        }

        /// <summary>
        /// Search function.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> Search(
            string functionName, 
            int siteId,
            string env = Constant.Uat,
            bool? isActive = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get function by parent key and site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list of function.</returns>
        public IEnumerable<IFunction> GetByParentKeyAndSite(GetByParentKeyAndSiteCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(criteria.SiteId, "SiteId");
            Check.Argument.IsNotEmpty(criteria.ParentKey, "ParentKey");
            Check.Argument.IsNotEmpty(criteria.Environment, "Environment");

            if (criteria.IsValid())
            {
                return _functionRepository.Fetch(criteria);
            }

            return null;
        }

        #endregion

        #region Gets Function Policy

        /// <summary>
        /// Get user function policy.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByUser(int userId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserFunctionCriteria
            {
                UserId = userId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFullFunctionPolicy(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function policy by user and group by site.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>List of function policy group by site.</returns>
        public Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByUserAndGroupBySite(int userId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserFunctionCriteria
            {
                UserId = userId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                IEnumerable<FunctionPolicy> functionPolicyList = this._functionRepository.FetchFullFunctionPolicy(criteria);

                var result = new Dictionary<ISite, IEnumerable<FunctionPolicy>>();

                List<Site> siteList = functionPolicyList.GroupBy(f => f.Site.Id)
                    .Select(s => new Site
                    {
                        Id = s.Key,
                        Name = s.Where(f => f.Site.Id == s.Key).FirstOrDefault().Site.Name
                    }).ToList();

                foreach (Site site in siteList)
                {
                    var subFunctionPolicyList = functionPolicyList.Where(f => f.Site.Id == site.Id).ToList();

                    result.Add(site, subFunctionPolicyList);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Get function policy by group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByGroup(int groupId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(groupId, "GroupId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByGroupFunctionCriteria
            {
                GroupId = groupId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFullFunctionPolicy(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function policy by group and site.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByGroupAndSite(int groupId, int siteId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(groupId, "GroupId");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByGroupAndSiteFunctionCriteria
            {
                GroupId = groupId,
                SiteId = siteId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFullFunctionPolicy(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function policy by group and group by site.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>List of function policy group by site.</returns>
        public Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByGroupAndGroupBySite(int groupId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(groupId, "GroupId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByGroupFunctionCriteria
            {
                GroupId = groupId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                IEnumerable<FunctionPolicy> functionPolicyList = this._functionRepository.FetchFullFunctionPolicy(criteria);

                var result = new Dictionary<ISite, IEnumerable<FunctionPolicy>>();

                List<Site> siteList = functionPolicyList.GroupBy(f => f.Site.Id)
                    .Select(s => new Site
                    {
                        Id = s.Key,
                        Name = s.Where(f => f.Site.Id == s.Key).FirstOrDefault().Site.Name
                    }).ToList();

                foreach (Site site in siteList)
                {
                    var subFunctionPolicyList = functionPolicyList.Where(f => f.Site.Id == site.Id).ToList();

                    result.Add(site, subFunctionPolicyList);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Get function policy by user site.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByUserAndSite(int userId, int siteId, string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserAndSiteFunctionCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFullFunctionPolicy(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function policy by user and site include group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(
            int userId,
            int siteId,
            string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotNegativeOrZero(siteId, "SiteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserAndSiteIncludeGroupFunctionCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFunctionPolicy(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get function policy by user and function key include group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="functionKey">The function key.</param>
        /// <param name="env">The environment.</param>
        /// <returns>The list of function policy.</returns>
        public IEnumerable<FunctionPolicy> GetByUserAndFunctionKeyIncludeGroup(
            int userId,
            string functionKey,
            string env = Constant.Uat)
        {
            // check parameters
            Check.Argument.IsNotNegativeOrZero(userId, "UserId");
            Check.Argument.IsNotEmpty(functionKey, "FunctionKey");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserAndFunctionKeyIncludeGroupFunctionCriteria
            {
                UserId = userId,
                FunctionKey = functionKey,
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._functionRepository.FetchFunctionPolicy(criteria);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Check user function policy.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="functionId">The function ID.</param>
        /// <param name="policy">The function policy.</param>
        /// <returns>True : has, false : not.</returns>
        public bool CheckUserFunctionPolicy(int userId, int functionId, int policy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Duplicate function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Duplicate(DuplicateFunctionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

                if (criteria.IsValid() && logCriteria.IsValid())
                {
                    return this._functionRepository.Duplicate(criteria, logCriteria);
                }
            }

            return null;
        }

        /// <summary>
        /// Insert function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Insert(InsertFunctionCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

                if (criteria.IsValid() && logCriteria.IsValid())
                {
                    return this._functionRepository.Insert(criteria, logCriteria);
                }
            }

            return null;
        }

        /// <summary>
        /// Update function.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Update(UpdateFunctionCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

                if (logCriteria.IsValid())
                {
                    return this._functionRepository.Update(criteria, logCriteria);
                }
            }

            return null;
        }

        /// <summary>
        /// Update function status.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateStatus(UpdateStatusFunctionCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

                if (logCriteria.IsValid())
                {
                    return this._functionRepository.Update(criteria, logCriteria);
                }
            }

            return null;
        }

        /// <summary>
        /// Assign user permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult AssignFunctionToUsers(AssignFunctionToUsersCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._functionRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Assign group permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult AssignFunctionToGroups(AssignFunctionToGroupsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._functionRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Sort function in hierarchy.
        /// </summary>
        /// <param name="functionList">List of function.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>The list of function.</returns>
        private IEnumerable<IFunction> SortFunctionsInHierarchy(IEnumerable<IFunction> functionList, int parentId = 0)
        {
            var sortedFunctionList = new List<IFunction>();

            foreach (IFunction function in functionList)
            {
                // top functions || sub functions
                if ((function.Parent == null && parentId == 0)
                    || (function.Parent != null && function.Parent.Id == parentId))
                {
                    sortedFunctionList.Add(function);
                    sortedFunctionList.AddRange(SortFunctionsInHierarchy(functionList, function.Id));
                }
            }

            return sortedFunctionList;
        }
    }
}