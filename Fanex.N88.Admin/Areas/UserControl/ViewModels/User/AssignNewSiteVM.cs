namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.User
{
    using System.Collections.Generic;
    using N88.Core.Domain;
    using N88.Core.Model.ValueObject;

    public class AssignNewSiteVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public List<ISite> AvailableSitesForUser { get; set; }

        public List<SiteLevel> CurrentUserSiteLevelList { get; set; }

        public List<ISite> HostRightSites { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public int Level { get; set; }
    }
}