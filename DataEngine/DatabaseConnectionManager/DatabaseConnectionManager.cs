using System.Collections.Concurrent;

namespace BuildHub.DataEngine.DatabaseConnectionManager
{
    #region Dependencies
    using Common.Utilities;
    using Configuration;
    using DatabaseConnection;
    using Exceptions.DatabaseConnection;
    using Extensions;
    #endregion

    #region Aliases
    using DatabaseConnectionPoolsMap = ConcurrentDictionary<DatabaseConnection.DatabaseSource, DatabaseConnection.IDatabaseConnectionPool>;
    using DatabaseConfigurationsMap = Dictionary<DatabaseConnection.DatabaseSource, Configuration.DatabaseConfiguration>;
    using System.Diagnostics;
    using BuildHub.Common.Logger;
    using System.Linq;
    #endregion

    /// <summary>
    /// Provides functionality for managing database connections within the application.
    /// </summary>
    public sealed class DatabaseConnectionManager
    {
        /// <summary>
        /// Provides a lazily initialized, thread-safe instance of the DatabaseConnectionManager.
        /// </summary>
        /// <remarks>The instance is created only when first accessed, ensuring that resources are
        /// allocated only if needed. This approach is suitable for scenarios where the DatabaseConnectionManager may
        /// not always be required during application execution.</remarks>
        private static readonly Lazy<DatabaseConnectionManager> _databaseConnectionManagerInstance
            = new(() => new DatabaseConnectionManager());

        /// <summary>
        /// Stores the mapping between database sources and their associated connection pools.
        /// </summary>
        /// <remarks>Each entry associates a specific database source with its corresponding connection
        /// pool, enabling efficient management and reuse of database connections.</remarks>
        private DatabaseConnectionPoolsMap _connectionPoolsMap;

        /// <summary>
		/// Database configuration manager instance
		/// </summary>
		private DatabaseConfigurationManager _databaseConfigurationManager;

        /// <summary>
		/// Represents a collection of database connection configurations.
		/// </summary>
		/// <remarks>This field holds the configurations for connecting to one or more databases.  It is intended for
		/// internal use and should not be accessed directly outside of the containing class.</remarks>
		private DatabaseConfigurationsMap _databaseConfigurationsMap;

        private DatabaseConnectionManager()
        {
            this._connectionPoolsMap = new DatabaseConnectionPoolsMap();
            this._databaseConfigurationManager = DatabaseConfigurationManager.GetInstance();
            this._databaseConfigurationsMap = new();
        }

        /// <summary>
        /// Validates whether the required database configurations are present
        /// </summary>
        /// <returns></returns>
        private void AreRequiredDatabaseConfigurationsPresent()
        {
            var databaseSources = EnumUtilities.GetEnumValues<DatabaseSource>().Where(databaseSource => databaseSource.IsRequired());

            foreach (var databaseSource in databaseSources)
            {
                if (!_databaseConfigurationsMap.ContainsKey(databaseSource))
                    throw new MissingRequiredDatabaseConfigurationException(databaseSource);
            }
        }

        /// <summary>
        /// Validates the database configuration
        /// </summary>
        /// <param name="databaseConfiguration">The database configuration object</param>
        /// <exception cref="InvalidDatabaseConfigurationException"></exception>
        private void ValidateDatabaseConfiguration(DatabaseConfiguration databaseConfiguration)
        {
            var databaseSource = databaseConfiguration.DatabaseSource;

            if (databaseConfiguration.MaxPoolConnections < 1)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                   $"for  database: {databaseConfiguration.DatabaseSource}. Property \"MaxPoolConnections\" is less than 1 .", databaseSource);

            if (databaseConfiguration.MinPoolConnections > databaseConfiguration.MaxPoolConnections)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"MaxPoolConnections\" is greater than \"MinPoolConnections\" .", databaseSource);

            if (databaseConfiguration.AcquireConnectionRetryCount <= 0)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"AcquireConnectionRetryCount\" is less than or equal to 0 .", databaseSource);

            if (databaseConfiguration.AcquireConnectionTimeout <= 0)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"AcquireConnectionTimeout\" is less than or equal to 0 .", databaseSource);

            if (databaseConfiguration.PoolGrowthThresholdPercentage <= 0)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"PoolGrowthThresholdPercentage\" is less than or equal to 0 .", databaseSource);

            if (databaseConfiguration.PoolGrowthStep <= 0)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"PoolGrowthStep\" is less than or equal to 0 .", databaseSource);

            if (databaseConfiguration.CloseIdleConnectionsTimeout <= 0)
                throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
                  $"for  database: {databaseConfiguration.DatabaseSource}. Property \"CloseIdleConnectionsTimeout\" is less than or equal to 0 .", databaseSource);
        }

        /// <summary>
        /// Retrieves the singleton instance of the DatabaseConnectionManager.
        /// </summary>
        /// <remarks>This method provides access to the application's global DatabaseConnectionManager.
        /// The same instance is returned on every call.</remarks>
        /// <returns>The single, shared instance of the DatabaseConnectionManager.</returns>
        public static DatabaseConnectionManager GetInstance() => _databaseConnectionManagerInstance.Value;

        /// <summary>
        /// Initializes the database connection pools using the configured database settings.
        /// </summary>
        /// <remarks>This method retrieves all configured database settings, validates them, and creates a
        /// connection pool for each database source. It must be called before attempting to access any database
        /// connections through this component. Calling this method multiple times may result in duplicate configuration
        /// handling, depending on the implementation of the underlying connection pool map.</remarks>
        public void Initialize()
        {
            Debug.Assert(this._connectionPoolsMap.Count == 0);

            _databaseConfigurationsMap = _databaseConfigurationManager.GetDatabaseConfigurations();
            AreRequiredDatabaseConfigurationsPresent();

            foreach (DatabaseConfiguration databaseConfiguration in _databaseConfigurationsMap.Values)
            {
                ValidateDatabaseConfiguration(databaseConfiguration);

                IDatabaseConnectionPool connectionPool = new DatabaseConnectionPoolImplementation();
                connectionPool.Initialize(databaseConfiguration);

                if (!this._connectionPoolsMap.TryAdd(databaseConfiguration.DatabaseSource, connectionPool))
                {
                    throw new DuplicateDatabaseConfigurationException();
                }
            }

            Logger.LogInformation("Database connection pools initialized: {DatabaseSources}", _connectionPoolsMap.Keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseSource"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private IDatabaseConnectionPool GetConnectionPool(DatabaseSource databaseSource)
        {
            if (!this._connectionPoolsMap.TryGetValue(databaseSource, out IDatabaseConnectionPool? connectionPool))
                throw new KeyNotFoundException();

            if (connectionPool is null)
                throw new NullReferenceException();

            return connectionPool;
        }

        /// <summary>
        /// Establishes a connection to the specified database source.
        /// </summary>
        /// <param name="databaseSource">The database source to which the connection will be established. Cannot be null.</param>
        public DatabaseConnection GetDatabaseConnection(DatabaseSource databaseSource)
        {
            var connectionPool = GetConnectionPool(databaseSource);
            return connectionPool.GetDatabaseConnection();
        }

        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
        {
            var connectionPool = GetConnectionPool(databaseConnection.DatabaseSource);
            connectionPool.ReleaseDatabaseConnection(databaseConnection);
        }

        public DatabaseConnectionPoolMetrics GetPoolMetrics(DatabaseSource databaseSource)
        {
            var connectionPool = GetConnectionPool(databaseSource);
            return connectionPool.Metrics;
        }

        public void ShutDown()
        {
            foreach (IDatabaseConnectionPool conenctionPool in _connectionPoolsMap.Values)
                conenctionPool.ShutDown();
        }
    }
}
