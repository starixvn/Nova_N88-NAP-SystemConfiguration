// --------------------------------------------------------------------------
// <copyright file="AssignFunctionGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.20">Created</change>
//   <change who="Lewis.Le" date="2012.9.7">Added GroupName, SiteId</change>
// </history>

namespace Fanex.NAP.Core.Criteria.Group
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Web;

    /// <summary>
    /// Assign permission to group criteria.
    /// </summary>
    public class AssignFunctionGroupCriteria : CriteriaBase
    {
        private const string AssignPermissionTpl = "<FunctionPolicy SiteId='{0}' FunctionId='{1}' Policy='{2}'></FunctionPolicy>";

        public int GroupId { get; set; }

        [SpParam(Ignored = true)]
        public string GroupName { get; set; }

        [Display(Name = "Site", ResourceType = typeof(ViewResources))]
        public int SiteId { get; set; }

        [SpParam(Ignored = true)]
        public int[] FuncRead { get; set; }

        [SpParam(Ignored = true)]
        public int[] FuncHost { get; set; }

        /// <summary>
        /// Gets permission string.
        /// </summary>
        /// <value>
        /// XML string <ROOT><FunctionPolicy SiteId="4" FunctionId="4" Policy="2"></FunctionPolicy></ROOT>.
        /// </value>
        public string Value
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (this.FuncRead != null)
                {
                    foreach (var funcId in this.FuncRead)
                    {
                        value.AppendFormat(AssignPermissionTpl, this.SiteId, funcId, 1); // 1 -> read
                    }
                }

                if (this.FuncHost != null)
                {
                    foreach (var funcId in this.FuncHost)
                    {
                        value.AppendFormat(AssignPermissionTpl, this.SiteId, funcId, 2); // 2 -> host
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "NAP_Group_AssignFunction";
        }

        /// <summary>
        /// Check data constraints.
        /// </summary>
        /// <returns>True: valid; False: invalid.</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}