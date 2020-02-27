using Fanex.NAP.Admin.Configurations;

namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    public class AssignSiteFunctionViewModel
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int LoginUserId { get; set; }

        public bool LoginUserIsPortalAdmin { get; set; }

        public bool LoginUserHasAssignSitePermission { get; set; }

        public bool LoginUserHasAssignSiteReadPermission { get; set; }

        public int LoginUserAssignFunctionPermission { get; set; }

        public bool IsSelfEdit { get; set; }

        public int CurrentSiteId { get; set; }

        public bool IsDevelopingVueJS { get; } = AppSettings.VueJSEnvironment == "DEV";
    }
}