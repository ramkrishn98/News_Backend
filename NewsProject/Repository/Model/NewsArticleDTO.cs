
namespace NewsProject.Repository.Model
{
    public class NewsArticleDTO
    {
        public Int32 Id { get; set; }            
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? By { get; set; }
        public string? Url { get; set; }
    }
}
