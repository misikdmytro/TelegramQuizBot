namespace QuizBot.Api.Models
{
    public class UserAnswer : IIDentifiable
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Answer Answer { get; set; }
        public int? AnswerId { get; set; }
    }
}
