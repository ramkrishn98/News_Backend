using NewsProject.Repository.Model;

namespace NewsProject.Repository.Interface
{
    public interface INewsArticle
    {
        Task<List<Int32>> GetNewsAsync();
        Task<NewsArticleDTO> GetNewsArticleByIdAsync(int articleId);
    }
}
