namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using NAP.Core.Criteria.User;

    public class AddNewUserVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public InsertCustCriteria Criteria { get; set; }
    }
}