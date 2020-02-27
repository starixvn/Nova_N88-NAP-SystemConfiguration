// --------------------------------------------------------------------------
// <copyright file="ViewFlags.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web
{
    using Fanex.NAP.Core.Business;

    /// <summary>
    /// View flags.
    /// </summary>
    public class ViewFlags
    {
        public ViewFlags()
        {
            this.IsLive = false;
            this.HasEditRight = false;
            this.IsNapAdminSite = false;
            this.HasAssignFunctionRight = false;
            this.HasAssignFunctionToUserRight = false;
            this.HasAssignFunctionToGroupRight = false;
            this.HasAssignSiteRight = false;
            this.IsShowLevelColumn = false;
            this.IsShowActionColumn = false;
            this.IsReCheckin = false;
        }
        
        public bool IsLive { get; set; }

        public bool HasEditRight { get; set; }

        public bool IsNapAdminSite { get; set; }

        public bool HasAssignSiteReadRight { get; set; }

        public bool HasAssignSiteRight { get; set; }

        public bool HasAssignFunctionRight { get; set; }

        public bool HasAssignFunctionToUserRight { get; set; }

        public bool HasAssignFunctionToGroupRight { get; set; }

        public bool HasManageUserRight { get; set; }

        public bool IsShowLevelColumn { get; set; }

        public bool IsShowActionColumn { get; set; }

        public bool IsReCheckin { get; set; }

        /// <summary>
        /// Sets HasEditRight to true if policy is >= Host.
        /// </summary>
        /// <param name="policy">Function policy.</param>
        public void SetEditRight(int policy)
        {
            if (policy >= (int)Constant.Policy.Host)
            {
                this.HasEditRight = true;
            }
        }
    }
}
