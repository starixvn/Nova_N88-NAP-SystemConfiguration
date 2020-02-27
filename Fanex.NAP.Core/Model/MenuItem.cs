// --------------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.8">Created</change>
// </history>

namespace Fanex.NAP.Core.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NAP.Core.Domain;

    /// <summary>
    /// Menu item model.
    /// </summary>
    [Serializable]
    public class MenuItem : IMenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IMenuItem Parent { get; set; }

        public IFunction Function { get; set; }

        public ISite Site { get; set; }

        public IUser Creator { get; set; }

        public IUser Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public short DisplayOrder { get; set; }

        public string Environment { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
        public string Url { get; set; }

        public int Policy { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
        public string ValidUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Url))
                {
                    return this.Url.StartsWith("/", StringComparison.CurrentCulture) ? "~" + this.Url : this.Url;
                }

                return "/";
            }
        }
    }
}