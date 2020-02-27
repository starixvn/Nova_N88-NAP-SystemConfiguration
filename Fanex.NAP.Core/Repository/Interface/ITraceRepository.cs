// --------------------------------------------------------------------------
// <copyright file="ITraceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.1">Created</change>
//   <change who="Lewis.Le" date="2012.10.5">Added FetchSiteUrl</change>
// </history>

namespace Fanex.NAP.Core.Repository.Interface
{
    using System.Linq;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Trace;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface ITraceRepository : IDynamicRepository
    {
        IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria);

        IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria);

        IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria);

        LogExecSp GetById(ICriteria criteria);

        ICommandResult InsertLog(ICriteria criteria);
    }
}