// --------------------------------------------------------------------------
// <copyright file="SiteLevel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.8">Created</change>
// </history>

namespace Fanex.NAP.Core.Model.ValueObject
{
    using System;

    /// <summary>
    /// Site level model.
    /// </summary>
    public class SiteLevel
    {
        public Site Site { get; set; }

        public int Level { get; set; }

        public bool IsActive { get; set; }
        
        public DateTime LastLoginDate { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}