using Elastic.Apm.SerilogEnricher;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Eventflix.Api.Extensions.Configurations
{
    public static class AddLogsExtension
    {
        public static ConfigureHostBuilder AddLogs(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:Uri"]))
            {
                IndexFormat = configuration["Elasticsearch:LogsSettings:IndexFormat"]
            };

            host.UseSerilog((context, provider) => provider
                .Enrich.WithElasticApmCorrelationInfo()
                .Enrich.WithCorrelationId()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithProperty("AppName", configuration["Elasticsearch:LogsSettings:AppName"])
                .WriteTo.Console()
                .WriteTo.Elasticsearch(elasticsearchSinkOptions));

            return host;
        }
    }
}