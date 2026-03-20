namespace BuildHub.DataEngine.Statistics
{
    using BuildHub.Common.Utilities;
    using DatabaseConnection;

    /// <summary>
    /// A database service providing functionality for maintaining the database.
    /// For example growing, or reducing the pool, closing idle connections and more...
    /// </summary>
    public sealed class DatabaseMaintenanceService
    {
        /// <summary>
        /// Reference to the database connection pool
        /// </summary>
        private readonly DatabaseConnectionPool _databaseConnectionPool;

        public DatabaseMaintenanceService()
        {
            this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
        }

        /// <summary>
        /// Returns a collection of database source of databases that are unreachable.
        /// </summary>
        /// <returns>collection of database sources</returns>
        public IEnumerable<DatabaseSource> GetUnreachableDatabases()
        {
            var databaseSources = EnumUtilities.GetEnumValues<DatabaseSource>();
            foreach (var databaseSource in databaseSources)
            {
                using var databaseConnection = _databaseConnectionPool.GetDatabaseConnection(databaseSource);

                DatabaseConnectionValidator databaseConnectionValidator = new(databaseConnection);
                if (!databaseConnectionValidator.TestDatabaseConnection())
                    yield return databaseSource;
            }
        }
    }
}
