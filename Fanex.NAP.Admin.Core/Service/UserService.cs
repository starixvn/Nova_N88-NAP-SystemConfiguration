namespace Fanex.NAP.Admin.Core.Service
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.Admin.Core.DbParams.Commands.User;

    public interface IUserService
    {
        void SyncUserToOWP(int userId);
    }

    public class UserService : IUserService
    {
        public void SyncUserToOWP(int userId)
        {
            var command = new InsertUserToOWPCommand(userId);

            using (var db = new ObjectDb(command.GetSettingKey()))
            {
                var hash = command.GetSpParams();

                db.ExecuteNonQuery(hash);
            }

            if (command.ErrCode > 0)
            {
                Logger.Log.Error($"SyncUserToOWP error: UserId: {userId}; Message: {command.ErrMsg}");
            }
        }
    }
}