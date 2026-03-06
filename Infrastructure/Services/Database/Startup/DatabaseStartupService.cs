using BuildHub.Common.Logger;
using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.Infrastructure.Services.Database.Startup
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SetupDatabaseConnections()
        {
            try
            {
                this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
            }
            catch (Exception exception)
            {
                Logger.LogFatal(exception, "Unable to connect to the database. Ensure the server is running and the connection string is valid..");
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CloseDatabaseConnections()
        {
            try
            {
                this._databaseConnectionPool?.Dispose();
            }
            catch (Exception exception)
            {
                Logger.LogFatal(exception, "Failed to release the database resources.");
            }

            return false;
        }
    }
}
