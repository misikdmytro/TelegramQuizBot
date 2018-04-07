namespace Materialise.FrontendDays.Bot.Api.Models
{
    public class StubAnswer
    {
        public int Id { get; set; }
        public virtual Question Question { get; set; }
        public int QuestionId { get; set; }
    }
}
