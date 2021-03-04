using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApplyTLS.Services
{
    public class GreeterHealthChecker : IHealthCheck
    {
        private readonly Greeter.GreeterClient _client;

        public GreeterHealthChecker(Greeter.GreeterClient client)
        {
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var reply = await _client.SayHelloAsync(
                new HelloRequest {Name = "GreeterClient"});
            if (reply.Message == "Hello GreeterClient")
            {
                return HealthCheckResult.Healthy("A healthy result.");
            }

            return HealthCheckResult.Unhealthy("An unhealthy result.");
        }
    }
}