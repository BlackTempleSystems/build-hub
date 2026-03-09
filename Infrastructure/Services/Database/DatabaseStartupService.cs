using BuildHub.Common.Logger;
using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.Domain.Services.Database
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DatabaseStartupService : IDatabaseStartupService
    {
        /// <summary>
        /// 
        /// </summary>
        private DatabaseConnectionPool? _databaseConnectionPool = null;

        public DatabaseStartupService()
        {
        }

        /// <summary>
        /// Initializes and establishes connections to the database by setting up the connection pool.
        /// </summary>
        /// <remarks>If the connection setup fails, a fatal error is logged indicating the reason for the
        /// failure, such as issues with the database server or an invalid connection string.</remarks>
        /// <returns>true if the database connections are successfully established; otherwise, false.</returns>
        public bool Initialize()
        {
            try
            {
                this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
            }
            catch (Exception exception)
            {
                Logger.LogFatal(exception, "Unable to connect to the database. Ensure the server is running and the connection string is valid..");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to dispose of the database connection pool and release associated resources.
        /// </summary>
        /// <remarks>If an exception occurs during disposal, it is logged as a fatal error. The return
        /// value does not indicate whether the resources were successfully released.</remarks>
        /// <returns>Always returns <see langword="false"/>.</returns>
        public bool Shutdown()
        {
            try
            {
                this._databaseConnectionPool?.Dispose();
            }
            catch (Exception exception)
            {
                Logger.LogFatal(exception, "Failed to release the database resources.");
                return false;
            }

            return true;
        }
    }
}
