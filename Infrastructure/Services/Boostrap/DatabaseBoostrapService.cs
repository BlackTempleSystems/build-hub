#region Dependencies
using BuildHub.Common.Logger;
using BuildHub.DataEngine.DatabaseConnectionManager;
using BuildHub.DataEngine.Exceptions.DatabaseConnection;
using Microsoft.Extensions.Hosting;
#endregion

namespace BuildHub.API.Startup
{
    /// <summary>
    /// Provides a hosted service that manages the startup and shutdown of the application's database connection
    /// lifecycle within an ASP.NET Core environment.
    /// </summary>
    /// <remarks>This service implements the IHostedService interface, allowing it to participate in the
    /// application's startup and graceful shutdown process. It is typically registered with the dependency injection
    /// container to ensure that database connections are properly initialized when the application starts and disposed
    /// of when the application stops.</remarks>
    public sealed class DatabaseBootstrapService : IHostedService
    {
        /// <summary>
        /// Access to the application lifetime events
        /// </summary>
        private readonly IHostApplicationLifetime _applicationLifetime;

        public DatabaseBootstrapService(IHostApplicationLifetime applicationLifetime)
        {
            this._applicationLifetime = applicationLifetime;
        }

        /// <summary>
        /// Starts the asynchronous operation, allowing for cancellation through the provided token.
        /// </summary>
        /// <remarks>Override this method in a derived class to provide the logic for starting the service
        /// asynchronously.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous start operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    DatabaseConnectionManager.GetInstance().Initialize();
                }
                catch (InvalidDatabaseConfigurationException invalidDatabaseConfigurationException)
                {
                    Logger.LogFatal(invalidDatabaseConfigurationException, "Invalid database configuration | Source {DatabaseSource}. Ensure all database configurations are valid.",
                        invalidDatabaseConfigurationException.DatabaseSource);

                    this._applicationLifetime.StopApplication();
                }
                catch (Exception exception)
                {
                    Logger.LogFatal(exception, "Unable to connect to the database. Ensure the server is running and the connection string is valid..");
                    this._applicationLifetime.StopApplication();
                }

            });
        }

        /// <summary>
        /// Stops the asynchronous operation and releases any resources used by it.
        /// </summary>
        /// <remarks>This method is intended to be overridden in a derived class to provide the actual
        /// stopping logic.</remarks>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>A task that represents the asynchronous stop operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    DatabaseConnectionManager.GetInstance().ShutDown();
                }
                catch(Exception exception)
                {
                    Logger.LogError(exception, "An error occurred while trying to clean all database resources.");
                }
            });
        }
    }
}
