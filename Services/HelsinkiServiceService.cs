using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceLocatorBackend.Models;

namespace ServiceLocatorBackend.Services
{
    public class HelsinkiServiceService : IHelsinkiServiceService
    {
        private readonly HttpClient _httpClient;

        public HelsinkiServiceService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<HelsinkiServiceResponse> GetServices(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            var response = await _httpClient.GetAsync($"search/?format=json&type=unit&q={query}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync
                <HelsinkiServiceResponse>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
