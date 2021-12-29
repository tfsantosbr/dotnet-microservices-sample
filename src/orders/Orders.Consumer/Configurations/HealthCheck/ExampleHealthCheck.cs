using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orders.Consumer.Configurations.HealthCheck
{
    public class ExampleHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckResultHealthy = true;

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Example health check is: HEALTHY"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Example health check is: UNHEALTHY"));
        }
    }
}