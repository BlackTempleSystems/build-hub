namespace BuildHub.DataEngine.Statistics
{
    using BuildHub.Common.Logger;
    using BuildHub.Common.Utilities;
    using BuildHub.DataEngine.Extensions;
    using DatabaseConnection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DatabaseMaintenanceManager
    {
        /// <summary>
        /// Reference to the database connection pool
        /// </summary>
        private readonly DatabaseConnectionPool _databaseConnectionPool;

        public DatabaseMaintenanceManager()
        {
            this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
        }

        /// <summary>
        /// Retrieves a connection for every database and performs a simple select
        /// </summary>
        /// <returns>Boolean representing whether the database is reachable or not</returns>
        public bool IsDatabaseIsReachable()
        {
            var databaseSources = EnumUtilities.GetEnumValues<DatabaseSource>().Where(source => source.IsRequired());
            foreach (var databaseSource in databaseSources)
            {
                try
                {

                    using var databaseConnection = _databaseConnectionPool.GetDatabaseConnection(databaseSource);
                    DatabaseConnectionValidator databaseConnectionValidator = new(databaseConnection);
                    if (!databaseConnectionValidator.TestDatabaseConnection())
                        return false;

                }
                catch (Exception exception)
                {
                    Logger.LogFatal(exception, "");
                    return false;
                }
            }

            return true;
        }
    }
}
