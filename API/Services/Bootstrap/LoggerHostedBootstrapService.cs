using BuildHub.Infrastructure.Services.Startup.Logger;

namespace BuildHub.API.Startup
{
    /// <summary>
    /// Provides startup and shutdown services for logging functionality within the application.
    /// </summary>
    /// <remarks>Inherits from BaseStartupService and is intended to manage the lifecycle of logging services.
    /// Override the StartAsync and StopAsync methods to implement custom logic for initializing and terminating logging
    /// resources as part of the application's startup and shutdown processes.</remarks>
    public class LoggerHostedBootstrapService : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILoggerStartupService _loggerStartupService;

        public LoggerHostedBootstrapService(IHostApplicationLifetime applicationLifeTime,
            ILoggerStartupService loggerStartupService)
        {
            this._applicationLifetime = applicationLifeTime;
            this._loggerStartupService = loggerStartupService;
        }

        /// <summary>
        /// Starts the asynchronous initialization of the logger service.
        /// </summary>
        /// <remarks>The logger initialization is performed on a background task, allowing the calling
        /// thread to continue without blocking. The returned task completes when the logger has finished
        /// initializing.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the initialization operation.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                this._loggerStartupService.InitializeLogger();
            });
        }

        /// <summary>
        /// Stops the execution of the service asynchronously.
        /// </summary>
        /// <remarks>This implementation completes immediately as there are no ongoing operations to
        /// stop.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to signal the request to cancel the stop operation.</param>
        /// <returns>A task that represents the asynchronous stop operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
           return Task.CompletedTask;
        }
    }
}
