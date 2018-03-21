using Materialise.FrontendDays.Bot.Api.Models;

namespace Materialise.FrontendDays.Bot.Api.Extensions
{
    public static class UserExtensions
    {
        public static bool NeedEmailInfo(this User user)
        {
            return user.UserStatus == UserStatus.NewUser || user.UserStatus == UserStatus.WaitForEmail;
        }

        public static bool HasPlayed(this User user)
        {
            return user.UserStatus == UserStatus.Answered || user.UserStatus == UserStatus.Failed;
        }
    }
}
