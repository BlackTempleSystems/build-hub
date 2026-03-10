using BuildHub.Domain.Services.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildHub.API.Services.HealthCheck
{
    /// <summary>
    /// Provides a health check for verifying the availability and responsiveness of a database connection.
    /// </summary>
    /// <remarks>This class is typically used with health monitoring frameworks to assess the operational
    /// status of a database. It implements the IHealthCheck interface, allowing integration with ASP.NET Core health
    /// checks or similar systems.</remarks>
    public sealed class DatabaseHealthCheck  : IHealthCheck
    {
        private readonly IDatabaseMaintananceService _databaseMaintananceService;

        public DatabaseHealthCheck(IDatabaseMaintananceService databaseMaintananceService)
        {
            this._databaseMaintananceService = databaseMaintananceService;
        }

        /// <summary>
        /// Performs a health check operation asynchronously and returns the result.
        /// </summary>
        /// <param name="context">The context information associated with the health check operation. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the health check operation.</param>
        /// <returns>A task that represents the asynchronous health check operation. The task result contains the health check
        /// result.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = HealthCheckResult.Healthy();

            if(!this._databaseMaintananceService.IsDatabaseRachable())
                healthCheckResult = HealthCheckResult.Unhealthy();

            return Task.FromResult<HealthCheckResult>(healthCheckResult);
        }
    }
}
