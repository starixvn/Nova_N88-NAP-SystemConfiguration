// --------------------------------------------------------------------------
// <copyright file="ISite.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.1">Created</change>
//   <change who="Lewis.Le" date="2012.8.29">Added NumOfUser</change>
// </history>

namespace Fanex.NAP.Core.Domain
{
    public interface ISite : IEntity<byte, string>
    {
        string SiteKey { get; set; }

        string SecretAccessKey { get; set; }

        string Description { get; set; }

        bool IsActive { get; set; }

        string Environment { get; set; }

        int NumOfUser { get; set; }
    }
}