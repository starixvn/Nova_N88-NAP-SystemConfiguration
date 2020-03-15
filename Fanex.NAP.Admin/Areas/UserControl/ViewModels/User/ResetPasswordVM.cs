namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using N88.Core.Criteria.User;

    public class ResetPasswordVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public ResetPasswordCustCriteria Criteria { get; set; }
    }
}