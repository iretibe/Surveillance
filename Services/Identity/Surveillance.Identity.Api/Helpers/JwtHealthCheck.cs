using Microsoft.Extensions.Diagnostics.HealthChecks;
using Surveillance.SharedKernel.Security;

namespace Surveillance.Identity.Api.Helpers
{
    public class JwtHealthCheck : IHealthCheck
    {
        private readonly JwtTokenGenerator _jwtGenerator;

        public JwtHealthCheck(JwtTokenGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var token = _jwtGenerator.GenerateToken(
                    Guid.NewGuid(),
                    "health",
                    "somady12@gmail.com",
                    new[] { "Health" });

                return Task.FromResult(string.IsNullOrEmpty(token)
                    ? HealthCheckResult.Unhealthy("JWT generation failed")
                    : HealthCheckResult.Healthy("JWT is working"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("JWT generation failed", ex));
            }
        }
    }
}
