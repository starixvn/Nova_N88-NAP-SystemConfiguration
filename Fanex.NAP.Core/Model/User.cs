// --------------------------------------------------------------------------
// <copyright file="User.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.8">Created</change>
//   <change who="Lewis.Le" date="2012.9.24">Added data annotations</change>
// </history>

namespace Fanex.NAP.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Web;

    /// <summary>
    /// User model.
    /// </summary>
    [Serializable]
    public class User : IUser
    {
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(ViewResources))]
        public string Name { get; set; }

        [Display(Name = "Group", ResourceType = typeof(ViewResources))]
        public IGroup Group { get; set; }

        [Display(Name = "Username", ResourceType = typeof(ViewResources))]
        public string Username { get; set; }

        [Display(Name = "Password", ResourceType = typeof(ViewResources))]
        public string Password { get; set; }

        [Display(Name = "Last_Login_Date", ResourceType = typeof(ViewResources))]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(ViewResources))]
        public IUser Creator { get; set; }

        [Display(Name = "Created_Date", ResourceType = typeof(ViewResources))]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modifier", ResourceType = typeof(ViewResources))]
        public IUser Modifier { get; set; }

        [Display(Name = "Modified_Date", ResourceType = typeof(ViewResources))]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "First_Name", ResourceType = typeof(ViewResources))]
        public string FirstName { get; set; }

        [Display(Name = "Last_Name", ResourceType = typeof(ViewResources))]
        public string LastName { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ViewResources))]
        public string Email { get; set; }
    }
}