using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orders.Consumer.Configurations.HealthCheck.Publishers
{
    public class HealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<HealthCheckPublisher> _logger;
        private readonly string _fileName = "healthy";
        private HealthStatus _prevStatus = HealthStatus.Unhealthy;

        public HealthCheckPublisher(ILogger<HealthCheckPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var fileExists = _prevStatus == HealthStatus.Healthy;

            if (report.Status == HealthStatus.Healthy)
            {
                if (!fileExists)
                {
                    using var _ = File.Create(_fileName);
                }
            }
            else if (fileExists)
            {
                File.Delete(_fileName);
            }

            _prevStatus = report.Status;

            _logger.LogInformation("{Timestamp} Health Check Status: {Result}",
                    DateTime.UtcNow, report.Status);

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;

        }
    }
}