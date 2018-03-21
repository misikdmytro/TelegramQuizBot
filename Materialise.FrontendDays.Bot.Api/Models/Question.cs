using System.Collections.Generic;

namespace Materialise.FrontendDays.Bot.Api.Models
{
    public class Question : IIDentifiable
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual ICollection<Answer> PossibleAnswers { get; set; }
    }
}
