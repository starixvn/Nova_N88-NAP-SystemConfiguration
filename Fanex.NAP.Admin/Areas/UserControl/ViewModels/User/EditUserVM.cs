namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using NAP.Core.Criteria.User;

    public class EditUserVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public UpdateCustCriteria Criteria { get; set; }
    }
}