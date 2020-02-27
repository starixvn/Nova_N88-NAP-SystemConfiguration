// --------------------------------------------------------------------------
// <copyright file="UserPolicy.cs" company="Nexcel Solutions Vietnam">
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
    /// User policy model.
    /// </summary>
    public class UserPolicy
    {
        public int Policy { get; set; }

        public User User { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
