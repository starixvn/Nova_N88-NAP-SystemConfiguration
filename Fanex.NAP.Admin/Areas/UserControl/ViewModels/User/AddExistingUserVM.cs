namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using Fanex.NAP.Core.Helpers;
    using N88.Core.Domain;

    public class AddExistingUserVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public PagerList<IUser> UserList { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}