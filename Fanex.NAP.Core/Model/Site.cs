// --------------------------------------------------------------------------
// <copyright file="Site.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.8">Created</change>
//   <change who="Lewis.Le" date="2012.8.29">Added NumOfUser</change>
// </history>

namespace Fanex.NAP.Core.Model
{
    using System;
    using Fanex.NAP.Core.Domain;

    /// <summary>
    /// Site model.
    /// </summary>
    [Serializable]
    public class Site : ISite
    {
        public byte Id { get; set; }

        public string Name { get; set; }

        public string SiteKey { get; set; }

        public string SecretAccessKey { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Environment { get; set; }

        public int NumOfUser { get; set; }
    }
}