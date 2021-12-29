using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Basket.Api.Configurations.HealthCheck
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisHealthCheck(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!_redis.IsConnected)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}