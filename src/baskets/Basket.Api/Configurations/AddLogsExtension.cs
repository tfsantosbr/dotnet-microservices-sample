using Elastic.Apm.SerilogEnricher;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Eventflix.Api.Extensions.Configurations
{
    public static class AddLogsExtension
    {
        public static ConfigureHostBuilder AddLogs(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Uri"];
            var indexFormat = configuration["Elasticsearch:IndexFormat"];

            var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(url))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
            };

            host.UseSerilog((context, provider) => provider
                .MinimumLevel.Override("Elastic.Apm", LogEventLevel.Fatal)
                .Enrich.WithElasticApmCorrelationInfo()
                .Enrich.WithCorrelationId()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(elasticsearchSinkOptions)
                );

            return host;
        }
    }
}