using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceLocatorBackend.Models;
using ServiceLocatorBackend.Services;

namespace ServiceLocatorBackend.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class HelsinkiServiceController : ControllerBase
    {
        private readonly ILogger<HelsinkiServiceController> _logger;
        private readonly IHelsinkiServiceService _helsinkiServiceService;

        public HelsinkiServiceController(ILogger<HelsinkiServiceController> logger, IHelsinkiServiceService helsinkiServiceService)
        {
            _logger = logger;
            _helsinkiServiceService = helsinkiServiceService;
        }

        [HttpGet]
        public HelsinkiServiceResponse Get([FromQuery]string query, [FromQuery]string page = "1")
        {
            _logger.LogInformation($"Invoked get with query: {query}");
            var response = _helsinkiServiceService.GetServices(query, page).Result;
            return response;
        }
    }
}
