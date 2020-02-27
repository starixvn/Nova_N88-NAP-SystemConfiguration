// --------------------------------------------------------------------------
// <copyright file="GetAllByUsernameGroupSiteUserCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.21">Created</change>
// </history>

namespace Fanex.NAP.Core.Criteria.User
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.Data.Repository;

    /// <summary>
    /// Check user by username group site criteria.
    /// </summary>
    public class GetAllByUsernameGroupSiteUserCriteria : CriteriaBase
    {
        private readonly string _siteTml = "<Site SiteId='{0}'></Site>";

        public string Username { get; set; }

        public int GroupId { get; set; }

        /// <summary>
        /// Gets site list string.
        /// </summary>
        /// <value>
        /// <ROOT>
        ///     <Site SiteId="1"></Site>
        ///     <Site SiteId="2"></Site>
        /// </ROOT>.
        /// </value>
        public string Site
        {
            get
            {
                if (ListSite != null)
                {
                    var value = new StringBuilder("<ROOT>");

                    foreach (SelectListItem siteId in ListSite)
                    {
                        value.AppendFormat(this._siteTml, siteId.Value); // 1 -> read
                    }

                    return value.Append("</ROOT>").ToString();
                }

                return string.Empty;
            }
        }

        [SpParam(Ignored = true)]
        public List<SelectListItem> ListSite { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "NAP_User_GetAllByUsernameGroupSite";
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