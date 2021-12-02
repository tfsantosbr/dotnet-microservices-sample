using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Eventflix.Api.Extensions.Configurations
{
    public static class AddLogsExtension
    {
        public static ConfigureHostBuilder AddLogs(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:Uri"]));

            host.UseSerilog((context, provider) => provider
                .WriteTo.Console()
                .WriteTo.Elasticsearch(elasticsearchSinkOptions));

            return host;
        }
    }
}