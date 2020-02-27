// --------------------------------------------------------------------------
// <copyright file="SiteRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.13">Created</change>
//   <change who="Lewis.Le" date="2012.10.5">Added FetchSiteUrl</change>
// </history>

namespace Fanex.NAP.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Criteria.Site;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Interface;

    /// <summary>
    /// Site repository.
    /// </summary>
    public class SiteRepository : BaseRepository<ISite>, ISiteRepository
    {
        /// <summary>
        /// Gets site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The site object.</returns>
        public override ISite Get(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Site>(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Get site URL .
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Site URL object.</returns>
        public SiteUrl GetSiteUrl(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<SiteUrl, Site, SiteUrl>(
                    (su, s) =>
                    {
                        su.Site = s;
                        return su;
                    },
                    "Id",
                    criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Count site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Number of site.</returns>
        public override int Count(ICriteria criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetch site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of site.</returns>
        public override IEnumerable<ISite> Fetch(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Site>(criteria.GetSpParams()).OrderBy(s => s.Name).ToList<Site>();
            }
        }

        /// <summary>
        /// Fetch site level.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of site level.</returns>
        public IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<SiteLevel, Site, User, User, SiteLevel>(
                        (sl, s, cu, mu) =>
                        {
                            sl.Site = s;
                            sl.Creator = cu;
                            sl.Modifier = mu;
                            return sl;
                        },
                        "SiteId,CreatorId,ModifierId",
                        criteria.GetSpParams()).OrderBy(s => s.Site.Name).ToList<SiteLevel>();
            }
        }

        /// <summary>
        /// Fetch site URL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of site URL.</returns>
        public IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<SiteUrl, Site, SiteUrl>(
                        (su, s) =>
                        {
                            su.Site = s;
                            return su;
                        },
                        "Id",
                        criteria.GetSpParams()).OrderBy(s => s.Site.Name).ToList<SiteUrl>();
            }
        }

        /// <summary>
        /// Insert site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Insert(InsertSiteCriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                criteria.SiteId = Convert.ToInt32(db.GetParameterValue("SiteId"));

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Insert site URL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                criteria.SiteUrlId = Convert.ToInt32(db.GetParameterValue("SiteUrlId"));

                return this.GetCommandResult(hash);
            }
    }

        /// <summary>
        /// Update site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Delete site.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
