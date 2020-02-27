namespace Fanex.NAP.Admin.Core.DbParams.Commands.User
{
    using Fanex.Data.Repository;

    public class InsertUserToOWPCommand : CriteriaBase
    {
        public InsertUserToOWPCommand(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public int ErrCode { get; }

        public string ErrMsg { get; }

        public override string GetSettingKey() => "OWP_UserInsert";

        public override bool IsValid() => UserId > 0;
    }
}