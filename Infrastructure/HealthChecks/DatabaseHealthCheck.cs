using BuildHub.Common.Logger;
using BuildHub.DataEngine.Exceptions.DatabaseConnection;
using BuildHub.DataEngine.Extensions;
using BuildHub.DataEngine.Statistics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildHub.API.Services.HealthCheck
{
    /// <summary>
    /// Provides a health check for verifying the availability and responsiveness of a database connection.
    /// </summary>
    /// <remarks>This class is typically used with health monitoring frameworks to assess the operational
    /// status of a database. It implements the IHealthCheck interface, allowing integration with ASP.NET Core health
    /// checks or similar systems.</remarks>
    public sealed class DatabaseHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Database maintenance service instance
        /// </summary>
        private readonly DatabaseMaintenanceService _databaseMaintananceService;

        public DatabaseHealthCheck()
        {
            this._databaseMaintananceService = new DatabaseMaintenanceService();
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
            HealthCheckResult healthCheckResult = HealthCheckResult.Healthy("All of the required databases are reachable.");

            try
            {
                var databaseSources = _databaseMaintananceService.GetUnreachableDatabases();
               
                if(databaseSources.Any())
                    healthCheckResult = HealthCheckResult.Unhealthy($"Database is unreachable | Source [{databaseSources}]");
            }
            catch(DatabaseConnectionNotEstablishedException databaseConnectionNotEstablishedException)
            {
                if (!databaseConnectionNotEstablishedException.DatabaseSource.IsRequired())
                    Logger.LogWarning($"Failed to establish a connection to [NOT REQUIRED] | Source: [{databaseConnectionNotEstablishedException.DatabaseSource}]");

            }
            catch(Exception exception)
            {
                Logger.LogError(exception, "");
                healthCheckResult = HealthCheckResult.Unhealthy();
            }
            
            return Task.FromResult(healthCheckResult);
        }
    }
}
