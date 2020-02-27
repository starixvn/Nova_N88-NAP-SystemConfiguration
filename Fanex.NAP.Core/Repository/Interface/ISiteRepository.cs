// --------------------------------------------------------------------------
// <copyright file="ISiteRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.1">Created</change>
//   <change who="Lewis.Le" date="2012.10.5">Added FetchSiteUrl</change>
// </history>

namespace Fanex.NAP.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Site;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface ISiteRepository : IReadonlyRepository<ISite>
    {
        SiteUrl GetSiteUrl(ICriteria criteria);

        IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria);

        IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria);

        ICommandResult Insert(InsertSiteCriteria criteria, ICriteria logCriteria);

        ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria, ICriteria logCriteria);

        ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

        ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);
    }
}