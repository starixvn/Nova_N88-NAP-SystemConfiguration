// --------------------------------------------------------------------------
// <copyright file="GetByParentKeyAndSiteCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.27">Created</change>
// </history>

namespace Fanex.NAP.Core.Criteria.Function
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetByParentKeyAndSiteCriteria class.
    /// </summary>
    public class GetByParentKeyAndSiteCriteria : CriteriaBase
    {
        public int SiteId { get; set; }

        public string ParentKey { get; set; }

        public string Environment { get; set; }

        public bool? IsActive { get; set; }

        public int? RoleId { get; set; }

        public int? UserId { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Function_GetByParentKeyAndSite";
        }

        public override bool IsValid()
        {
            return this.SiteId > 0;
        }
    }
}
