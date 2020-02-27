// --------------------------------------------------------------------------
// <copyright file="ITraceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    using System.Linq;
    using Fanex.NAP.Core.Criteria.Trace;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface ITraceService : IBaseService
    {
        LogExecSp GetById(int logId);

        IQueryable<LogExecSp> SearchExecSp(GetAllLogCriteria criteria);

        IQueryable<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria);

        IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria);
    }
}