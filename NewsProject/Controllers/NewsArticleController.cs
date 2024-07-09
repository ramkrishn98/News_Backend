using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewsProject.Repository.Interface;
using NewsProject.Repository.Model;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace NewsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticleController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly INewsArticle _newsArticleRepository;
        private readonly ILogger<NewsArticleController> _logger;
      

        public NewsArticleController(IMemoryCache cache, ILogger<NewsArticleController> logger, INewsArticle newsArticle)
        {
            _cache = cache;
            _newsArticleRepository = newsArticle;
            _logger = logger;
        }


        [HttpGet(Name = "GetNewsArticles")]
        public async Task<List<NewsArticleDTO>> GetNewsArticles(string searchKeyWord = "")
        {
            var stories = new List<NewsArticleDTO>();
            var newsIds = await _newsArticleRepository.GetNewsAsync();

            var tasks = newsIds.Select(_newsArticleRepository.GetNewsArticleByIdAsync);
            stories = (await Task.WhenAll(tasks)).ToList();

            if (!String.IsNullOrEmpty(searchKeyWord))
            {
                var search = searchKeyWord.ToLower();
                stories = stories.Where(s =>
                                   s.Title?.ToLower().IndexOf(search) > -1 || s.By?.ToLower().IndexOf(search) > -1)
                                   .ToList();
            }

            return stories;
        }

       

        //private async Task<NewsArticleDTO> GetNewsArticleByIdAsync(int ArticleId)
        //{
        //    try
        //    {
        //        var result = new NewsArticleDTO();
        //        var response = await _newsArticleRepository.GetStoryByIdAsync(ArticleId);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var stringResponse = await response.Content.ReadAsStringAsync();
        //            var newsArtcleDetailsResponse = JsonSerializer.Deserialize<NewsArticleDTO>(stringResponse, new JsonSerializerOptions()
        //            {
        //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //            });
        //            if (newsArtcleDetailsResponse != null)
        //            {
        //                result = newsArtcleDetailsResponse;
        //            }
        //        }
        //        return result;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception("HTTP request failed: " + ex.Message);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new Exception("JSON deserialization failed: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An unexpected error occured: " + ex.Message);
        //    }
        //}
    }
}
