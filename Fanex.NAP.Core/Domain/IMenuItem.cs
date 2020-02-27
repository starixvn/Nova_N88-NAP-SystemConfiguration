// --------------------------------------------------------------------------
// <copyright file="IMenuItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.7.18">Created</change>
// </history>

namespace Fanex.NAP.Core.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IMenuItem : IEntity<int, string>
    {
        string Description { get; set; }

        IMenuItem Parent { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Reviewed")]
        IFunction Function { get; set; }

        ISite Site { get; set; }

        IUser Creator { get; set; }

        IUser Modifier { get; set; }

        DateTime ModifiedDate { get; set; }

        DateTime CreatedDate { get; set; }

        bool IsActive { get; set; }

        short DisplayOrder { get; set; }

        string Environment { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
        string Url { get; set; }

        int Policy { get; set; }
    }
}