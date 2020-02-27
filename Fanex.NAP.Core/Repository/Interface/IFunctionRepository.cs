// --------------------------------------------------------------------------
// <copyright file="IFunctionRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.1">Created</change>
//   <change who="Lewis.Le" date="2012.10.26">Added ICommandResult Duplicate(ref DuplicateFunctionCriteria criteria, ICriteria logCriteria)</change>
// </history>

namespace Fanex.NAP.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Function;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface IFunctionRepository : IReadonlyRepository<IFunction>
    {
        IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria);

        IEnumerable<FunctionPolicy> FetchFullFunctionPolicy(ICriteria criteria);

        ICommandResult Duplicate(DuplicateFunctionCriteria criteria, ICriteria logCriteria);

        ICommandResult Insert(InsertFunctionCriteria criteria, ICriteria logCriteria);

        ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

        ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);
    }
}
