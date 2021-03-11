using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using ServiceLocatorBackend.Extensions;
using ServiceLocatorBackend.Models;

namespace ServiceLocatorBackend.Services
{
    public class HelsinkiServiceService : IHelsinkiServiceService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _distributedCache;

        public HelsinkiServiceService(HttpClient client, IDistributedCache distributedCache)
        {
            _httpClient = client;
            _distributedCache = distributedCache;
        }

        public async Task<HelsinkiServiceResponse> GetServices(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            return await _distributedCache.GetRecordAsync<HelsinkiServiceResponse>(query) ?? 
                   await GetServicesFromExternalService(query);
        }

        private async Task<HelsinkiServiceResponse> GetServicesFromExternalService(string query)
        {
            var response = await _httpClient.GetAsync($"search/?format=json&type=unit&q={query}");

            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync
                <HelsinkiServiceResponse>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            await _distributedCache.SetRecordAsync(query, result);
            return result;
        }
    }
}
