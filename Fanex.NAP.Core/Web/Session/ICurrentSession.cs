// --------------------------------------------------------------------------
// <copyright file="ICurrentSession.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Session
{
    using System.Collections.Generic;
    using Fanex.NAP.Core.Model;

    public interface ICurrentSession
    {
        string Username { get; set; }

        int UserId { get; set; }

        int Level { get; set; }

        int OnUserId { get; set; }

        string SessionId { get; set; }

        IList<MenuItem> MenuItemList { get; set; }

        void ClearAll();
    }
}