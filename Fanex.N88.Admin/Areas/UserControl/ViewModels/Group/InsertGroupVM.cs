namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.Group
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.N88.Admin.Core.Resources;

    public class InsertGroupVM
    {
        [Required]
        [Display(Name = "Name", ResourceType = typeof(ViewLang))]
        [StringLength(50, MinimumLength = 2,
            ErrorMessageResourceName = "InputLengthValidationMsg",
            ErrorMessageResourceType = typeof(ViewLang))]
        [RegularExpression(@"^[\w\s]{1,}$",
            ErrorMessageResourceName = "InvalidNameValidationMsg",
            ErrorMessageResourceType = typeof(ViewLang))]
        public string GroupName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ViewLang))]
        [StringLength(200, MinimumLength = 0,
            ErrorMessageResourceName = "InputLengthValidationMsg",
            ErrorMessageResourceType = typeof(ViewLang))]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Status", ResourceType = typeof(ViewLang))]
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        [Required]
        [Display(Name = "GroupType", ResourceType = typeof(ViewLang))]
        public bool? IsLicenseeGroup { get; set; }

        [Display(Name = "Site", ResourceType = typeof(ViewLang))]
        public int? SiteId { get; set; }

        public IList<SelectListItem> SiteList { get; set; }
    }
}