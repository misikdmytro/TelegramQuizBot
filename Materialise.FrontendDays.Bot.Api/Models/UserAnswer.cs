namespace Materialise.FrontendDays.Bot.Api.Models
{
    public class UserAnswer : IIDentifiable
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Question Question { get; set; }
        public int QuestionId { get; set; }
        public string RealAnswer { get; set; }
        /// <summary>
        /// Null means that no answer given
        /// True - answer is correct
        /// False - answer is incorrect
        /// </summary>
        public bool? IsCorrect { get; set; }
    }
}
