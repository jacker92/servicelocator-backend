using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocatorBackend.Services;
using ServiceLocatorBackend.Utils;
using StackExchange.Redis;

namespace ServiceLocatorBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllers();

            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("Redis healthcheck");

            services.AddTransient<IHelsinkiServiceService, HelsinkiServiceService>();

            AddRedis(services);
        }

        private void AddRedis(IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    Password = Configuration["REDIS_PASSWORD"]
                };

                options.ConfigurationOptions.EndPoints.Add(Configuration["REDIS_ENDPOINT"]);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options
                                  .AllowAnyOrigin()
                                  .AllowAnyMethod());

            app.UsePathBase(new PathString("/api"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
