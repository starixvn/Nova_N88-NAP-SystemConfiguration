// --------------------------------------------------------------------------
// <copyright file="TraceRepository.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Trace;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Interface;

    /// <summary>
    /// Trace repository.
    /// </summary>
    public class TraceRepository : DynamicRepository, ITraceRepository
    {
        /// <summary>
        /// Search execute SP log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of execute SP log.</returns>
        public IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();

                var logList = db.Query<LogExecSp>(hash);

                criteria.TotalRows = Convert.ToInt32(hash["totalrows"]);

                return logList.AsQueryable();
            }
        }

        /// <summary>
        /// Get execute SP log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Execute SP log object.</returns>
        public LogExecSp GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();

                return db.QueryEntity<LogExecSp>(hash);
            }
        }

        /// <summary>
        /// Insert log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult InsertLog(ICriteria criteria)
        {
            Hashtable hash = criteria.GetSpParams();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                return new CommandResult();
            }
        }

        /// <summary>
        /// Search user login log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user login log.</returns>
        public IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();

                var logList = db.Query<LogUserLogin, User, Site, LogUserLogin>(
                    (l, u, s) => { l.User = u; l.Site = s; return l; },
                    "UserId,SiteId",
                    hash);

                criteria.TotalRows = Convert.ToInt32(hash["totalrows"]);

                return logList.AsQueryable();
            }
        }

        /// <summary>
        /// Search function log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user login log.</returns>
        public IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();

                var logList = db.Query<LogFunction, Function, User, Site, LogFunction>(
                    (l, f, u, s) => { l.Function = f; l.User = u; l.Site = s; return l; },
                    "FunctionId,UserId,SiteId",
                    hash);

                criteria.TotalRows = Convert.ToInt32(hash["totalrows"]);

                return logList.AsQueryable();
            }
        }
    }
}
