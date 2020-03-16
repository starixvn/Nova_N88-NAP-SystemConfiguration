namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.User
{
    using N88.Core.Criteria.User;

    public class EditUserVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public UpdateCustCriteria Criteria { get; set; }
    }
}