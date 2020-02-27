// --------------------------------------------------------------------------
// <copyright file="SiteService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
//   <change who="Lewis.Le" date="2012.10.5">
//      Added GetSiteUrlByUser
//      Renamed GetByUser to GetSiteLevelByUser
//   </change>
//   <change who="Lewis.Le" date="2012.10.8">Added GetSiteUrlById</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Function;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Criteria.Site;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Impl;
    using Fanex.NAP.Core.Repository.Interface;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Site service.
    /// </summary>
    public class SiteService : BaseService, ISiteService
    {
        #region members

        private readonly ISiteRepository _siteRepository;
        private readonly IFunctionRepository _functionRepository;

        #endregion

        #region ctors

        public SiteService()
            : this(null, null)
        {
        }

        public SiteService(ISiteRepository siteRepository, IFunctionRepository functionRepository)
        {
            this._siteRepository = siteRepository ?? new SiteRepository();
            this._functionRepository = functionRepository ?? new FunctionRepository();
        }

        #endregion

        #region get

        /// <summary>
        /// Get site by ID.
        /// </summary>
        /// <param name="id">The site ID.</param>
        /// <returns>The site object.</returns>
        public ISite GetById(int id)
        {
            // check parammeters
            Check.Argument.IsNotNegativeOrZero(id, "id");

            var criteria = new GetByIdSiteCriteria
            {
                SiteId = id
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get site by name.
        /// </summary>
        /// <param name="name">The site name.</param>
        /// <returns>The site object.</returns>
        public ISite GetByName(string name)
        {
            // check parammeters
            Check.Argument.IsNotEmpty(name, "name");

            var criteria = new GetByNameSiteCriteria
            {
                SiteName = name
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Get(criteria);
            }

            return null;
        }

        public ISite GetByKey(string key)
        {
            // check parammeters
            Check.Argument.IsNotEmpty(key, "key");

            var criteria = new GetBySiteKeyCriteria
            {
                SiteKey = key
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Get(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get site status by ID.
        /// </summary>
        /// <param name="id">The site ID.</param>
        /// <returns>True : active, false : inactive.</returns>
        public bool GetStatusById(int id)
        {
            // check parammeters
            Check.Argument.IsNotNegativeOrZero(id, "id");

            bool isActive = false;

            var criteria = new GetByIdSiteCriteria
            {
                SiteId = id
            };

            if (criteria.IsValid())
            {
                ISite site = this._siteRepository.Get(criteria);

                if (site != null)
                {
                    isActive = site.IsActive;
                }
            }

            return isActive;
        }

        /// <summary>
        /// Get site level by user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>List of site level.</returns>
        public IEnumerable<SiteLevel> GetSiteLevelByUser(int userId, string env = Constant.Uat, bool? isActive = null)
        {
            // check parammeters
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetByUserSiteCriteria
            {
                UserId = userId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.FetchSiteLevel(criteria).OrderBy(sl => sl.Site.Name);
            }

            return null;
        }

        /// <summary>
        /// Get available sites for user.
        /// </summary>
        /// <param name="userId">The user identity.</param>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list site.</returns>
        public IEnumerable<ISite> GetAvailableSitesForUser(int userId, string env = Constant.Uat, bool? isActive = null)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");

            var criteria = new GetAvailableSitesCriteria
            {
                UserId = userId,
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Fetch(criteria).OrderBy(site => site.Name);
            }

            return null;
        }

        /// <summary>
        /// Get all site.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of site object.</returns>
        public IEnumerable<ISite> GetAll(string env = Constant.Uat, bool? isActive = null)
        {
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllSiteCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Fetch(criteria).OrderBy(s => s.Name);
            }

            return null;
        }

        /// <summary>
        /// Get all site select list item.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <param name="selectedSiteId">Selected site ID.</param>
        /// <returns>The list of select list item.</returns>
        public IList<SelectListItem> GetAllSelectListItem(
            string env = Constant.Uat,
            bool? isActive = null,
            int selectedSiteId = 0)
        {
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllSiteCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                IEnumerable<ISite> siteList = this._siteRepository.Fetch(criteria).OrderBy(s => s.Name);

                if (siteList != null)
                {
                    return siteList.Select(s => new SelectListItem
                        {
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = s.Id == selectedSiteId
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get all site with number of users.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>The list of site object.</returns>
        public IEnumerable<ISite> GetAllWithNumOfUsers(string env = Constant.Uat, bool? isActive = null)
        {
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllSiteWithNumOfUserCriteria
            {
                Environment = env,
                IsActive = isActive
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.Fetch(criteria).OrderBy(s => s.Name);
            }

            return null;
        }

        /// <summary>
        /// Get site by user and function key include group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="functionKey">The function key.</param>
        /// <param name="env">The environment.</param>
        /// <param name="policy">The function policy.</param>
        /// <returns>The list of site object.</returns>
        public IEnumerable<ISite> GetByUserAndFunctionKeyIncludeGroup(
            int userId,
            string functionKey,
            string env = Constant.Uat,
            Constant.Policy policy = Constant.Policy.Read)
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
                IEnumerable<FunctionPolicy> functionPolicyList = this._functionRepository.FetchFunctionPolicy(criteria).OrderBy(s => s.Site.Name);

                if (functionPolicyList != null)
                {
                    return functionPolicyList.Where(f => f.Policy >= (int)policy)
                        .Select(s => new Site
                        {
                            Id = s.Site.Id,
                            Name = s.Site.Name
                        }).ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get site select list item by user and function key include group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="functionKey">The function key.</param>
        /// <param name="env">The environment.</param>
        /// <param name="policy">The function policy.</param>
        /// <param name="selectedSiteId">Selected site ID.</param>
        /// <returns>The list of select list item.</returns>
        public IList<SelectListItem> GetSelectListItemByUserAndFunctionKeyIncludeGroup(
            int userId,
            string functionKey,
            string env = Constant.Uat,
            Constant.Policy policy = Constant.Policy.Read,
            int selectedSiteId = 0)
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
                IEnumerable<FunctionPolicy> functionPolicyList = this._functionRepository.FetchFunctionPolicy(criteria).OrderBy(s => s.Site.Name);

                if (functionPolicyList != null)
                {
                    return functionPolicyList.Where(f => f.Policy >= (int)policy)
                        .Select(s => new SelectListItem
                        {
                            Text = s.Site.Name,
                            Value = s.Site.Id.ToString(),
                            Selected = s.Site.Id == selectedSiteId
                        }).ToList();
                }
            }

            return null;
        }

        #endregion

        #region insert/update

        /// <summary>
        /// Insert site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Insert(InsertSiteCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.Insert(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult Update(UpdateSiteCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update site status.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateStatus(UpdateStatusSiteCriteria criteria)
        {
            // check parameters
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        #endregion

        #region SiteLevel services

        /// <summary>
        /// Get user site level.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>User site level.</returns>
        public SiteLevel GetUserSiteLevel(int userId, int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");

            var criteria = new GetUserSiteLevelSiteCriteria
            {
                UserId = userId,
                SiteId = siteId
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.FetchSiteLevel(criteria).FirstOrDefault<SiteLevel>();
            }

            return null;
        }

        #endregion

        #region SiteUrl services

        /// <summary>
        /// Get all site URL.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <returns>List of site URL.</returns>
        public IEnumerable<SiteUrl> GetAllSiteUrl(string env)
        {
            Check.Argument.IsNotEmpty(env, "Environment");

            var criteria = new GetAllSiteUrlCriteria
            {
                Environment = env
            };

            if (criteria.IsValid())
            {
                return this._siteRepository.FetchSiteUrl(criteria);
            }

            return null;
        }

        /// <summary>
        /// Get site URL by user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>List of site URL.</returns>
        public IEnumerable<SiteUrl> GetSiteUrlByUser(int userId, string env)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var result = new List<SiteUrl>();

            var criteria = new GetSiteUrlByUserCriteria
            {
                UserId = userId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                result = this._siteRepository.FetchSiteUrl(criteria).OrderBy(su => su.Site.Name).ToList();
            }

            return result;
        }

        /// <summary>
        /// Get site URL by site ID.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="env">The environment.</param>
        /// <returns>List of site URL.</returns>
        public IEnumerable<SiteUrl> GetSiteUrlBySiteId(int siteId, string env)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(env, "Environment");

            var result = new List<SiteUrl>();

            var criteria = new GetSiteUrlBySiteIdCriteria
            {
                SiteId = siteId,
                Environment = env
            };

            if (criteria.IsValid())
            {
                result = this._siteRepository.FetchSiteUrl(criteria).OrderBy(su => su.Site.Name).ToList();
            }

            return result;
        }

        /// <summary>
        /// Gets a SiteUrl by its SiteUrlId.
        /// </summary>
        /// <param name="siteUrlId">Site URL ID.</param>
        /// <returns>Site URL object.</returns>
        public SiteUrl GetSiteUrlById(int siteUrlId)
        {
            Check.Argument.IsNotNegativeOrZero(siteUrlId, "siteUrlId");

            SiteUrl result = null;

            var criteria = new GetSiteUrlByIdCriteria
            {
                SiteUrlId = siteUrlId
            };

            if (criteria.IsValid())
            {
                result = this._siteRepository.GetSiteUrl(criteria);
            }

            return result;
        }

        /// <summary>
        /// Update site URL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateSiteUrl(UpdateSiteUrlCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Update default site URL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult UpdateDefaultSiteUrl(UpdateDefaultSiteUrlCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.Update(criteria, logCriteria);
            }

            return null;
        }

        /// <summary>
        /// Insert site URL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            LogExecSpCriteria logCriteria = GetLogExecSpCriteria();

            if (criteria.IsValid() && logCriteria.IsValid())
            {
                return this._siteRepository.InsertSiteUrl(criteria, logCriteria);
            }

            return null;
        }

        #endregion
    }
}