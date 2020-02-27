// --------------------------------------------------------------------------
// <copyright file="ServiceContext.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Service context.
    /// </summary>
    public class ServiceContext : IServiceContext
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public string Ip { get; set; }
    }
}