using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NewsProject.Controllers;
using NewsProject.Repository.Interface;
using NewsProject.Repository.Service;

namespace NewsArticle.NUnitTest
{
    public class TestNewsArticleController
    {
        private IMemoryCache _cache;
        private readonly HttpClient _client;

        public TestNewsArticleController(IMemoryCache cache, HttpClient client)
        {
            _cache = cache;
            _client = client;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetNewsAsyncTest()
        {
            NewsArticleService obj = new NewsArticleService(_cache, _client);
            var result = obj.GetNewsAsync().Result;

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetNewsArticleByIdAsyncTest()
        {
            Int32 articleId = 40890035;

            NewsArticleService obj = new NewsArticleService(_cache, _client);
            var result = obj.GetNewsArticleByIdAsync(articleId).Result;

            Assert.IsNotNull(result);
        }
    }
}