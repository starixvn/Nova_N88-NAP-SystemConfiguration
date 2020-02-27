// --------------------------------------------------------------------------
// <copyright file="IServiceContext.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    public interface IServiceContext
    {
        int UserId { get; set; }

        int SiteId { get; set; }

        string Ip { get; set; }
    }
}
