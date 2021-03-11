using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceLocatorBackend.Models;

namespace ServiceLocatorBackend.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class HelsinkiServiceController : ControllerBase
    {
        private readonly ILogger<HelsinkiServiceController> _logger;

        public HelsinkiServiceController(ILogger<HelsinkiServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<HelsinkiService> Get()
        {
            return new List<HelsinkiService> { new HelsinkiService { Name = "test" } };
        }
    }
}
