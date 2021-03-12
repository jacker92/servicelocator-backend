﻿using System;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _distributedCache;

        public HelsinkiServiceService(IHttpClientFactory httpClientFactory, IDistributedCache distributedCache)
        {
            _httpClientFactory = httpClientFactory;
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
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new Uri("https://api.hel.fi/servicemap/v2/");
                var response = await client.GetAsync($"search/?format=json&type=unit&q={query}");

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
}
