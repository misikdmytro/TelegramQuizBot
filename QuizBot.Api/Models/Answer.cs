namespace QuizBot.Api.Models
{
    public class Answer : IIDentifiable
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual Question Question { get; set; }
        public int QuestionId { get; set; }
        public virtual Category Category { get; set; }
        public int? CategoryId { get; set; }
        public bool IsStub { get; set; }
    }
}
