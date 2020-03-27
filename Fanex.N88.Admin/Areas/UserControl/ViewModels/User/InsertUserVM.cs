namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.N88.Admin.Core.Resources;

    public class InsertUserVM
    {
        [Display(Name = "FirstName", ResourceType = typeof(ViewLang))]
        [StringLength(50, MinimumLength = 0, ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(ViewLang))]
        [StringLength(50, MinimumLength = 0, ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        public string LastName { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ViewLang))]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessageResourceName = "EmailFormatValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        public string Email { get; set; }

        [Display(Name = "Username", ResourceType = typeof(ViewLang))]
        [RegularExpression("^[\\w\\s]{1,}$", ErrorMessageResourceName = "InvalidNameValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ViewLang))]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewLang))]
        public string UserPwd { get; set; }

        [Display(Name = "Group", ResourceType = typeof(ViewLang))]
        public int GroupId { get; set; }

        [Required]
        [Display(Name = "UserType", ResourceType = typeof(ViewLang))]
        public bool? IsLicenseeUser { get; set; }

        [Display(Name = "Site", ResourceType = typeof(ViewLang))]
        public int? SiteId { get; set; }

        [Display(Name = "Level", ResourceType = typeof(ViewLang))]
        public int Level { get; set; }
    }
}