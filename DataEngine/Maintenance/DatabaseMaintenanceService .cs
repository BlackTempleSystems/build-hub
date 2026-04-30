namespace BuildHub.DataEngine.Statistics
{
    #region
    using BuildHub.Common.Utilities;
    using DatabaseConnection;
    using DatabaseConnectionManager;
    #endregion

    /// <summary>
    /// A database service providing functionality for maintaining the database.
    /// For example growing, or reducing the pool, closing idle connections and more...
    /// </summary>
    public sealed class DatabaseMaintenanceService
    {
        public DatabaseMaintenanceService()
        {
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
                using var databaseConnection = DatabaseConnectionManager.GetInstance().GetDatabaseConnection(databaseSource);

                DatabaseConnectionValidator databaseConnectionValidator = new(databaseConnection);
                if (!databaseConnectionValidator.TestDatabaseConnection())
                    yield return databaseSource;
            }
        }
    }
}
