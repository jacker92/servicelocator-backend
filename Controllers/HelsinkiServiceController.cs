using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public IEnumerable<object> Get([FromQuery]string query)
        {
            _logger.LogInformation($"Invoked get with query: {query}");
            var response = _helsinkiServiceService.GetServices(query).Result;
            return response?.Results;
        }
    }
}
