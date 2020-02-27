// --------------------------------------------------------------------------
// <copyright file="ICurrentContext.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Context
{
    using Fanex.NAP.Core.Configuration;

    public interface ICurrentContext
    {
        string IP { get; }

        string DomainName { get; }

        string BrowserName { get; }

        string Platform { get; }

        string CountryCode { get; }

        DomainElement CurrentDomain { get; }
    }
}