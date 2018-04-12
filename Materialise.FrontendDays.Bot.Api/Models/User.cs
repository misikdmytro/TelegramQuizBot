namespace Materialise.FrontendDays.Bot.Api.Models
{
    public class User : IIDentifiable
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public long ChatId { get; set; }
        public bool IsWinner { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}
