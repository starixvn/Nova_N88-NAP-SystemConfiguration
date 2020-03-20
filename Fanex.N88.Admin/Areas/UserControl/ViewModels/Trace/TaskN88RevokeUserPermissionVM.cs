namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.Trace
{
    using System.Collections.Generic;

    public class TaskN88RevokeUserPermissionVM
    {
        public IList<RevokedSite> RevokedSites { get; set; } = new List<RevokedSite>();
    }
}