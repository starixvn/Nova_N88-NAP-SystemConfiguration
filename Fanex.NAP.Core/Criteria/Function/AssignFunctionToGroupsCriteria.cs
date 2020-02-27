// --------------------------------------------------------------------------
// <copyright file="AssignFunctionToGroupsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Lewis.Le" date="2012.11.1">Created</change>
// </history>

namespace Fanex.NAP.Core.Criteria.Function
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// Assign function to group criteria.
    /// </summary>
    public class AssignFunctionToGroupsCriteria : CriteriaBase
    {
        private const string AssignPermissionTpl = "<FunctionPolicy GroupId='{0}' Policy='{1}'></FunctionPolicy>";

        public int FunctionId { get; set; }

        public int SiteId { get; set; }

        /// <summary>
        /// Gets permission string.
        /// </summary>
        /// <value>
        /// <ROOT><User GroupId="1" Policy="2"></User></ROOT>.
        /// </value>
        public string Value
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (this.GroupRead != null)
                {
                    foreach (var groupId in this.GroupRead)
                    {
                        value.AppendFormat(AssignPermissionTpl, groupId, 1); // 1 -> read
                    }
                }

                if (this.GroupHost != null)
                {
                    foreach (var userId in this.GroupHost)
                    {
                        value.AppendFormat(AssignPermissionTpl, userId, 2); // 2 -> host
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        [SpParam(Ignored = true)]
        public List<string> GroupRead { get; set; }

        [SpParam(Ignored = true)]
        public List<string> GroupHost { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "NAP_Function_AssignFunctionToGroups";
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