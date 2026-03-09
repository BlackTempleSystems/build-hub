namespace BuildHub.Domain.Services.Database
{
    /// <summary>
    /// Defines methods for initializing and shutting down database connections during application startup and shutdown.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for establishing and releasing database
    /// connections in a controlled manner. Proper use of these methods helps prevent resource leaks and ensures that
    /// database resources are available when needed. It is recommended to call these methods at appropriate points in
    /// the application's lifecycle, such as during startup and shutdown routines.</remarks>
    public interface IDatabaseStartupService
    {
        /// <summary>
        /// Initializes and establishes all required database connections for the application.
        /// </summary>
        /// <remarks>Call this method before performing any database operations to ensure that all
        /// connections are properly initialized.</remarks>
        /// <returns>true if all database connections are successfully established; otherwise, false.</returns>
        public bool Initialize();

        /// <summary>
        /// Closes all open database connections and releases associated resources.
        /// </summary>
        /// <remarks>This method should be called when database operations are complete to ensure that
        /// resources are properly released. Failing to close connections may lead to resource leaks and performance
        /// issues.</remarks>
        /// <returns>true if all database connections were successfully closed; otherwise, false.</returns>
        public bool Shutdown();
    }
}
