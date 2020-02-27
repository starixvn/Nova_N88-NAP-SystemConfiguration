namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using NAP.Core.Domain;
    using NAP.Core.Model.ValueObject;
    using System.Collections.Generic;

    public class AssignSiteVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public List<ISite> SiteList { get; set; }

        public List<SiteLevel> CurrentUserSiteLevelList { get; set; }

        public List<SiteLevel> SiteLevelList { get; set; }

        public List<ISite> EditableSiteList { get; set; }

        public int CurrentSessionLevel { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool IsSelfEdit { get; set; }

        public int CurrentSiteId { get; set; }

        public bool IsPortalAdmin { get; set; }
    }
}