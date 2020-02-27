// --------------------------------------------------------------------------
// <copyright file="IBaseService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
//   <change who="Edward.Nguyen" date="2012.8.14">Add GetRepositoryExecLog</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Criteria.Misc;

    public interface IBaseService
    {
        IServiceContext ServiceContext { get; set; }

        IEnumerable<T> GetData<T>(ICriteria criteria);

        T GetItem<T>(ICriteria criteria) where T : new();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed")]
        LogExecSpCriteria GetLogExecSpCriteria();
    }
}
