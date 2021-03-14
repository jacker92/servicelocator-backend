using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ServiceStack.Redis;

namespace ServiceLocatorBackend.Utils
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public RedisHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var splittedUri = _configuration["REDIS_ENDPOINT"].Split(':');

                using (var client = new RedisClient(splittedUri[0],
                                                    int.Parse(splittedUri[1]),
                                                    _configuration["REDIS_PASSWORD"]))
                {
                    var response = client.Info;

                    if (response != null && response.Any())
                    {
                        return Task.FromResult(HealthCheckResult.Healthy($"RedisCheck: Healthy"));
                    }
                    return Task.FromResult(HealthCheckResult.Unhealthy($"RedisCheck: Unhealthy"));

                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"RedisCheck: Exception during check: {ex.GetType().FullName}"));
            }
        }
    }
}
