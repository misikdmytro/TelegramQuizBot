namespace QuizBot.Api.Models
{
    public class BotInfo
    {
        public BotInfo(string token, string hostUrl)
        {
            Token = token;
            HostUrl = hostUrl;
        }

        public string Token { get; }
        public string HostUrl { get; }
    }
}
