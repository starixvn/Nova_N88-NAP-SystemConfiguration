namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.User
{
    using N88.Core.Criteria.User;

    public class AddNewUserVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public InsertCustCriteria Criteria { get; set; }
    }
}