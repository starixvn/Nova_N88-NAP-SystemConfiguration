// --------------------------------------------------------------------------
// <copyright file="InsertSiteCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.16">Created</change>
// </history>

namespace Fanex.NAP.Core.Criteria.Site
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Web;

    /// <summary>
    /// Insert new site criteria.
    /// </summary>
    public class InsertSiteCriteria : CriteriaBase
    {
        private const string UrlTpl = "<SiteURL URL='{0}' Environment='{1}' IsDefault='{2}' IsActive='{3}'></SiteURL>";

        public InsertSiteCriteria()
        {
            this.Environment = Constant.Uat;
        }

        public int SiteId { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(ViewResources))]
        [StringLength(50, MinimumLength = 2,
            ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewResources))]
        [RegularExpression(@"^[\w\s]{1,}$",
            ErrorMessageResourceName = "InvalidNameValidationMsg", ErrorMessageResourceType = typeof(ViewResources))]
        public string SiteName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ViewResources))]
        [StringLength(200, MinimumLength = 0,
            ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewResources))]
        public string Description { get; set; }

        [Display(Name = "URL", ResourceType = typeof(ViewResources))]
        [SpParam(Ignored = true)]
        public List<SiteUrl> Urls { get; set; }

        /// <summary>
        /// Gets site URL string.
        /// </summary>
        /// <value>
        /// <ROOT>
        ///    <SiteURL
        ///         URL="http://www.example.com/"
        ///         Environment="LIVE"
        ///         IsDefault="1"
        ///         IsActive="1">
        ///    </SiteURL>
        /// </ROOT>.
        /// </value>
        public string SiteUrls
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (this.Urls != null && this.Urls.Count > 0)
                {
                    foreach (var siteUrl in this.Urls)
                    {
                        value.AppendFormat(UrlTpl, siteUrl.Url, this.Environment, siteUrl.IsDefault, siteUrl.IsActive);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        [Display(Name = "Status", ResourceType = typeof(ViewResources))]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
        public string Environment { get; set; }

        public string SecretAccessKey { get; set; }

        public string SiteKey { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "NAP_Site_InsertWithSiteURL";
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