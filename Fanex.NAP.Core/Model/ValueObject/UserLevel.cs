// --------------------------------------------------------------------------
// <copyright file="UserLevel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.6.9">Created</change>
// </history>

namespace Fanex.NAP.Core.Model.ValueObject
{
    using System;

    /// <summary>
    /// User level model.
    /// </summary>
    public class UserLevel
    {
        public int Level { get; set; }

        public User User { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastLoginDate { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public Site Site { get; set; }
    }
}
