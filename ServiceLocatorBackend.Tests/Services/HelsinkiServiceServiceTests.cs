using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ServiceLocatorBackend.Services;
using ServiceLocatorBackend.Tests.Utils;

namespace ServiceLocatorBackend.Tests.Services
{
    [TestFixture()]
    public class HelsinkiServiceServiceTests
    {
        private HelsinkiServiceService _helsinkiServiceService;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<IDistributedCache> _distributedCache;

        [SetUp]
        public void InitTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _distributedCache = new Mock<IDistributedCache>();
            _helsinkiServiceService = new HelsinkiServiceService(_httpClientFactory.Object,
                                                                 _distributedCache.Object);
        }

        [Test()]
        [TestCase("test")]
        public void ShouldThrowJsonException_IfServerDoesNotReturnJsonResponse(string query)
        {
            Assert.ThrowsAsync<JsonException>(() => _helsinkiServiceService.GetServices(query, "1"));
        }

        [Test()]
        [TestCase(" ")]
        [TestCase(null)]
        public void ShouldReturnNull_ForNullOrWhiteSpaceQuery(string query)
        {
            var result = _helsinkiServiceService.GetServices(query, "1");

            Assert.IsNull(result.Result);
        }

        [Test()]
        [TestCase("test")]
        public void ShouldReturnCacheResult_IfValueIsFound(string query)
        {
            var response = TestDataRepository.CreateHelsinkiServiceResponse();

            var key = $"{query}|1";

            _distributedCache.Setup(x => x.GetAsync(key, default))
                .Returns(
                Task.FromResult(
                    Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(response))));

            var result = _helsinkiServiceService.GetServices(query, "1");

            Assert.AreEqual(response.Count, result.Result.Count);
            Assert.AreEqual(response.Next, result.Result.Next);
            Assert.AreEqual(response.Previous, result.Result.Previous);
            Assert.AreEqual(response.Results, result.Result.Results);

            _distributedCache.Verify(x => x.GetAsync(key, default));
            _distributedCache.VerifyNoOtherCalls();
            _httpClientFactory.VerifyNoOtherCalls();
        }

        [Test()]
        [TestCase("test")]
        public void ShouldCallApi_IfKeyNotFoundInCache(string query)
        {
            var key = $"{query}|1";
            var client = SetupMockHttpClient();
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            _distributedCache.Setup(x => x.GetAsync(key, default))
            .Returns(
                Task.FromResult((byte[])null));

            var result = _helsinkiServiceService.GetServices(query, "1");

            Assert.IsNotNull(result.Result);

            _distributedCache.Verify(x => x.GetAsync(key, default));
            _distributedCache.Verify(x => x.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            _distributedCache.VerifyNoOtherCalls();

            _httpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
            _httpClientFactory.VerifyNoOtherCalls();
        }

        private static HttpClient SetupMockHttpClient()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"name\":\"thecodebuzz\",\"city\":\"USA\"}"),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            return client;
        }
    }
}
