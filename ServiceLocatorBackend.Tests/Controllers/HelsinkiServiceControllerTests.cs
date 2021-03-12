using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ServiceLocatorBackend.Controllers;
using ServiceLocatorBackend.Models;
using ServiceLocatorBackend.Services;
using System.Threading.Tasks;

namespace ServiceLocatorBackend.Tests.Controllers
{
    [TestFixture()]
    public class HelsinkiServiceControllerTests
    {
        private HelsinkiServiceController _helsinkiServiceController;
        private Mock<IHelsinkiServiceService> _helsinkiServiceService;
        private ILogger<HelsinkiServiceController> _logger;

        [SetUp]
        public void InitTests()
        {
            _logger = new Mock<ILogger<HelsinkiServiceController>>().Object;
            _helsinkiServiceService = new Mock<IHelsinkiServiceService>();
            _helsinkiServiceController = new HelsinkiServiceController(_logger, _helsinkiServiceService.Object);
        }

        [Test()]
        [TestCase("test")]
        [TestCase(" ")]
        public void ShouldCallHelsinkiServiceService_WithAnyQueryValue(string query)
        {
            _helsinkiServiceService.VerifyNoOtherCalls();

            _helsinkiServiceController.Get(query);

            _helsinkiServiceService.Verify(x => x.GetServices(query));
            _helsinkiServiceService.VerifyNoOtherCalls();
        }

        [Test()]
        [TestCase("test")]
        [TestCase(" ")]
        public void ShouldReturnNull_IfServiceReturnsNull(string query)
        {
            _helsinkiServiceService.Setup(x => x.GetServices(query)).Returns(
                Task.FromResult((HelsinkiServiceResponse)null));

            var result = _helsinkiServiceController.Get(query);

            Assert.IsNull(result);

            _helsinkiServiceService.Verify(x => x.GetServices(query));
            _helsinkiServiceService.VerifyNoOtherCalls();
        }

        [Test()]
        [TestCase("test")]
        [TestCase(" ")]
        public void ShouldReturnMockedResponse(string query)
        {
            var returned = new HelsinkiServiceResponse
            {
                Results = new object[]
                {
                   new object
                   {
                   }
                }
            };

            _helsinkiServiceService.Setup(x => x.GetServices(query)).Returns(
                Task.FromResult(returned));

            var result = _helsinkiServiceController.Get(query);

            Assert.AreEqual(returned.Results, result);

            _helsinkiServiceService.Verify(x => x.GetServices(query));
            _helsinkiServiceService.VerifyNoOtherCalls();
        }
    }
}