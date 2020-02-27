// --------------------------------------------------------------------------
// <copyright file="INapPrincipal.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Auth
{
    public interface INapPrincipal
    {
        int UserId { get; set; }

        int OnUserId { get; set; }
    }
}
