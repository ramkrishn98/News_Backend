using Microsoft.Extensions.Caching.Memory;
using NewsProject.Repository.Interface;
using NewsProject.Repository.Model;
using System.Text.Json;

namespace NewsProject.Repository.Service
{
    public class NewsArticleService : INewsArticle
    {
        private IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly HttpClient _client;
        private string baseURL = "https://hacker-news.firebaseio.com/v0";
        public NewsArticleService(IMemoryCache cache, HttpClient client)
        {
            _cache = cache;
            _client = client;
        }

        public async Task<List<Int32>> GetNewsAsync()
        {
            try
            {
                var url = string.Format(baseURL + "/topstories.json");
                var result = new List<Int32>();

                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var newsResponse = JsonSerializer.Deserialize<List<Int32>>(stringResponse, new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    if (newsResponse != null)
                    {
                        result = newsResponse.Take(200).ToList();
                    }
                }
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("HTTP request failed: " + ex.Message);
            }
            catch (JsonException ex)
            {
                throw new Exception("JSON deserialization failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured: " + ex.Message);
            }
        }

        public async Task<NewsArticleDTO> GetNewsArticleByIdAsync(int articleId)
        {
            try
            {
                string cacheKey = $"News_{articleId}";

                if (!_cache.TryGetValue(cacheKey, out NewsArticleDTO? _newsArticleDTO))
                {
                    //var response = await GetStoryByIdAsync(articleId);
                    var response = await _client.GetAsync(string.Format(baseURL + "/item/{0}.json", articleId));

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponse = await response.Content.ReadAsStringAsync();
                        var newsArtcleDetailsResponse = JsonSerializer.Deserialize<NewsArticleDTO>(stringResponse, new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        if (newsArtcleDetailsResponse != null)
                        {
                            _newsArticleDTO = newsArtcleDetailsResponse;
                        }
                    }
                    _cache.Set(cacheKey, _newsArticleDTO, _cacheExpiration);
                }
                return _newsArticleDTO ?? new NewsArticleDTO();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("HTTP request failed: " + ex.Message);
            }
            catch (JsonException ex)
            {
                throw new Exception("JSON deserialization failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured: " + ex.Message);
            }
        }
    }
}
