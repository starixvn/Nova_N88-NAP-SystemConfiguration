// --------------------------------------------------------------------------
// <copyright file="InsertFunctionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.16">Created</change>
// </history>

namespace Fanex.NAP.Core.Criteria.Function
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web;

    /// <summary>
    /// Insert function criteria.
    /// </summary>
    public class InsertFunctionCriteria : CriteriaBase
    {
        public InsertFunctionCriteria()
        {
            this.Environment = Constant.Uat;
        }
        
        public int FunctionId { get; set; }

        public int SiteId { get; set; }

        [Required]
        [Display(Name = "Function_Name", ResourceType = typeof(ViewResources))]
        [StringLength(50, MinimumLength = 2,
            ErrorMessageResourceName = "InputLengthValidationMsg",
            ErrorMessageResourceType = typeof(ViewResources))]
        public string FunctionName { get; set; }

        [Display(Name = "Function_Key", ResourceType = typeof(ViewResources))]
        [StringLength(50, MinimumLength = 2,
            ErrorMessageResourceName = "InputLengthValidationMsg",
            ErrorMessageResourceType = typeof(ViewResources))]
        public string FunctionKey { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ViewResources))]
        [StringLength(200, MinimumLength = 0,
            ErrorMessageResourceName = "InputLengthValidationMsg",
            ErrorMessageResourceType = typeof(ViewResources))]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        [Display(Name = "CRUD", ResourceType = typeof(ViewResources))]
        public byte Crud
        {
            get
            {
                return this.Readonly ? (byte)Constant.Crud.Read : (byte)Constant.Crud.Host;
            }
        }

        [Display(Name = "Read_only", ResourceType = typeof(ViewResources))]
        public bool Readonly { get; set; }

        [Required]
        [Display(Name = "Display_Order", ResourceType = typeof(ViewResources))]
        public int DisplayOrder { get; set; }

        [Required]
        [Display(Name = "Status", ResourceType = typeof(ViewResources))]
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
        public string Environment { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "NAP_Function_Insert";
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
