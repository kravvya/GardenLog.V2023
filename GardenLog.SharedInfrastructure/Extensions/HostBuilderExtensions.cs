using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Elasticsearch;

namespace GardenLog.SharedInfrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseLogging(this IHostBuilder builder) =>
          builder.UseSerilog((context, logger) =>
          {
              logger.Enrich.FromLogContext();
              logger.Enrich.WithSpan();
              logger.Enrich.WithClientIp();
              logger.Enrich.WithClientAgent();

              logger.ReadFrom.Configuration(context.Configuration);

              if (context.HostingEnvironment.IsDevelopment())
              {
                  logger.WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level: u3} {Message}{NewLine}{Exception}");
              }
              else
              {
                  logger.WriteTo.Console(new ElasticsearchJsonFormatter());
              }
          });

        public static void RegisterPoly(this WebApplicationBuilder builder, string registryName, int numberOfRetries)
        {
            var registry = new Polly.Registry.PolicyRegistry()
            {
                {
                    registryName,
                     HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .WaitAndRetryAsync(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            context.GetLogger()?.LogWarning("Failed to post event to target consumer. Delaying for {delay}ms, then making retry {retry}. Error message {exception}", timespan.TotalMilliseconds, retryAttempt, outcome.Exception);
                        })
                },
            };

            builder.Services.AddPolicyRegistry(registry);
        }

    }
}
