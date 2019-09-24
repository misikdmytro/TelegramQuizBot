namespace QuizBot.Api.Models
{
    public class Category : IIDentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
