using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.DatabaseConnection
{
    #region Dependencies
    using Common.Logger;
    using Common.Utilities;
    using DataEngine.Configuration;
    using Exceptions.DatabaseConnection;
    using Extensions;
    using Messages;
    #endregion

    #region Aliases
    using Timer = System.Timers.Timer;
    #endregion

    /// <summary>
    /// Database connection pool singleton, initializing and managing a number of database connections.
    /// </summary>
    internal sealed class DatabaseConnectionPoolImplementation : IDatabaseConnectionPool, IDisposable
    {
        /// <summary>
        /// Interval in milliseconds on which a snapshot of the metrics is logged (5 minutes).
        /// </summary>
        private const int _METRICS_SNAPSHOT_INTERVAL = 300000;

        /// <summary>
        /// Database configuration for a specific database source.
        /// </summary>
        private DatabaseConfiguration? _databaseConfiguration;

        /// <summary>Available connection ready for use</summary>
        private readonly Queue<DatabaseConnection> _databaseConnectionsQueue;

        /// <summary>
        /// Provides access to metrics related to the database connection pool.
        /// </summary>
        private readonly DatabaseConnectionPoolMetrics _metrics;

        /// <summary>
        /// An object used to synchronize access to shared resources in a multi-threaded environment.
        /// </summary>
        /// <remarks>This field is intended to be used as a locking mechanism to ensure thread safety when accessing
        /// or modifying shared data. Always use this object with a <c>lock</c> statement to avoid race conditions.</remarks>
        private readonly object _mutex = new object();

        /// <summary>
        /// Timer which logs the database connection pool metrics on certain period.
        /// </summary>
        private Timer _metricsLogTimer;

        /// <summary>
        /// Timer on which we close idle connections if needed.
        /// </summary>
        private Timer _closeIdleConnectionsTimer;

        /// <summary>
        /// Boolean indicating whether the object is disposed.
        /// </summary>
        private volatile bool _isDisposed;

        /// <summary>
        /// Information about the current status of the pool.
        /// </summary>
        public DatabaseConnectionPoolMetrics Metrics { get { return _metrics; } }

        public DatabaseConnectionPoolImplementation()
        {
            this._databaseConnectionsQueue = new();
            this._metrics = new();
            this._metricsLogTimer = new Timer();
            this._closeIdleConnectionsTimer = new Timer();
            this._isDisposed = false;

        }

        ~DatabaseConnectionPoolImplementation() => Dispose(false);

        /// <summary>
        /// Logs the current metrics snapshot on a certain interval
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnMetricsSnapshot(Object? source, System.Timers.ElapsedEventArgs e)
        {
            var utilization = _metrics.CalculateUtilizationPercentage();

            if (utilization <= 0.0)
                return;

            if(utilization >= _databaseConfiguration!.PoolGrowthThresholdPercentage)
            {
                Logger.LogWarning(DataEngineMessages.DATABASE_STATISTICS_REPORT, _metrics.TotalConnectionsCount, _metrics.IdleConnectionsCount, _metrics.ActiveConnectionsCount,
                _metrics.CalculateUtilizationPercentage(), _databaseConfiguration!.DatabaseSource);
            }
            else
            {
                Logger.LogInformation(DataEngineMessages.DATABASE_STATISTICS_REPORT, _metrics.TotalConnectionsCount, _metrics.IdleConnectionsCount, _metrics.ActiveConnectionsCount,
                _metrics.CalculateUtilizationPercentage(), _databaseConfiguration!.DatabaseSource);
            }  
        }

        /// <summary>
        /// Closes idle connections on a certain timer interval depending on the utilization and minimum connections.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnCloseIdleConnections(Object? source, System.Timers.ElapsedEventArgs e)
        {
            if (_databaseConnectionsQueue.Count <= _databaseConfiguration!.MinPoolConnections)
                return;

            var utilization = _metrics.CalculateUtilizationPercentage();
            if (utilization < _databaseConfiguration!.PoolGrowthThresholdPercentage)
                return;

            int idleConnectionsToCloseCount = _databaseConnectionsQueue.Count - _databaseConfiguration!.MinPoolConnections;

            for (int index = 0; index < idleConnectionsToCloseCount; index++)
            {
                var databaseConnection = _databaseConnectionsQueue.Dequeue();
                databaseConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Grows the connection pool according to the current utilization threshold and opened connections.
        /// </summary>
        /// <exception cref="ConnectionPoolExhaustedException"></exception>
        private void GrowConnectionPool()
        {
            int maxConnectionsCount = _databaseConfiguration!.MaxPoolConnections;
            int totalOpenedConnectionsCount = Metrics.TotalConnectionsCount;
            int activeConnectionsCount = Metrics.ActiveConnectionsCount;

            //No more capacity to grow
            if (totalOpenedConnectionsCount >= maxConnectionsCount)
                return;

            double utilizationPercentage = ((double)activeConnectionsCount / totalOpenedConnectionsCount) * 100;
            if (utilizationPercentage < _databaseConfiguration.PoolGrowthThresholdPercentage)
                return;

            int connectionsToOpen = Math.Min(_databaseConfiguration!.PoolGrowthStep, maxConnectionsCount - totalOpenedConnectionsCount);
            int newTotalConnectionsOpened = totalOpenedConnectionsCount + connectionsToOpen;

            if (newTotalConnectionsOpened > maxConnectionsCount)
                throw new ConnectionPoolExhaustedException(_databaseConfiguration.DatabaseSource);

            for (int index = 0; index < connectionsToOpen; index++)
            {
                var databaseConnection = InitializeConnection(_databaseConfiguration.DatabaseSource);
                _databaseConnectionsQueue.Enqueue(databaseConnection);
                Monitor.Pulse(_mutex);
            }

            Logger.LogInformation(DataEngineMessages.CONNECTION_POOL_REGROWN, _databaseConfiguration.DatabaseSource, 
                connectionsToOpen, newTotalConnectionsOpened, maxConnectionsCount);
        }

        /// <summary>
        /// Returns a reference to a database connection
        /// </summary>
        /// <returns>DatabaseConnection</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DatabaseConnection GetDatabaseConnection()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

            lock (_mutex)
            {
                int retryCount = 0;
                DatabaseConnection? databaseConnection = null;

                while (retryCount < _databaseConfiguration!.AcquireConnectionRetryCount)
                {
                    if (_databaseConnectionsQueue.Count > 0)
                    {
                        databaseConnection = _databaseConnectionsQueue.Dequeue();
                        break;
                    }

                    retryCount++;

                    _metrics.OnWaitStart();

                    if (!Monitor.Wait(_mutex, _databaseConfiguration.AcquireConnectionTimeout))
                        GrowConnectionPool();

                    _metrics.OnWaitEnd();
                }

                if (databaseConnection is null)
                {
                    Logger.LogWarning(DataEngineMessages.CONNECTION_POOL_EXHAUSTED, _databaseConfiguration.DatabaseSource);
                    throw new ConnectionPoolExhaustedException(_databaseConfiguration.DatabaseSource);
                }

                databaseConnection.IsConnectionPooled = false;
                _metrics.OnAcquire();

                return databaseConnection;
            }
        }

        /// <summary>
        /// Returns a database connection to the pool
        /// </summary>
        /// <param name="databaseConnection"></param>
        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

            if (databaseConnection.IsConnectionPooled)
                throw new DatabaseConnectionLeakException();

            lock (_mutex)
            {
                var databaseSource = databaseConnection.DatabaseSource;
                if (databaseConnection.IsConnectionOpen())
                {
                    var databaseConfiguration = this._databaseConfiguration;
                    if (databaseConfiguration is null)
                        throw new MissingRequiredDatabaseConfigurationException(databaseSource);

                    if (_databaseConnectionsQueue.Count < databaseConfiguration.MaxPoolConnections)
                    {
                        if (!_databaseConnectionsQueue.Contains(databaseConnection))
                        {
                            databaseConnection.IsConnectionPooled = true;
                            _databaseConnectionsQueue.Enqueue(databaseConnection);
                            _metrics.OnRelease();

                            Monitor.Pulse(_mutex);
                        }
                    }
                    else
                    {
                        databaseConnection.CloseConnection();
                        Logger.LogInformation($"Closing excess connection for database: {databaseSource}");
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the connection string associated with the specified database source.
        /// </summary>
        /// <remarks>This method uses the configuration manager to obtain the connection string based on the
        /// description of the provided database source. Ensure that the connection string is defined in the configuration for
        /// the application.</remarks>
        /// <param name="databaseSource">The database source for which to retrieve the connection string. This parameter must be a valid value of the
        /// DatabaseSource enumeration.</param>
        /// <returns>A string representing the connection string for the specified database source.</returns>
        /// <exception cref="EmptyConnectionStringException">Thrown if the connection string for the specified database source is null or empty.</exception>
        private string GetConnectionString(DatabaseSource databaseSource)
        {
            DatabaseConfigurationManager databaseConfigurationsMap = DatabaseConfigurationManager.GetInstance();

            string connectionStringKey = EnumUtilities.GetEnumDescription<DatabaseSource>(databaseSource);
            string connectionString = databaseConfigurationsMap.GetConnectionString(connectionStringKey);

            if (string.IsNullOrEmpty(connectionString))
                throw new EmptyConnectionStringException(databaseSource);

            return connectionString;
        }

        /// <summary>
        /// Initializes and opens a connection to the specified database source.
        /// </summary>
        /// <remarks>This method retrieves the connection string for the specified database source, attempts to open
        /// the connection,  and validates the connection. If the connection cannot be opened or fails validation, the
        /// connection is closed  and an exception is thrown.</remarks>
        /// <param name="databaseSource">The database source to connect to.</param>
        /// <returns>A <see cref="DatabaseConnection"/> instance representing the established connection.</returns>
        /// <exception cref="DatabaseConnectionValidationException">Thrown if the connection to the specified database source fails validation.</exception>
        private DatabaseConnection InitializeConnection(DatabaseSource databaseSource)
        {
            var connectionString = GetConnectionString(databaseSource);
            var databaseConnection = new DatabaseConnection(databaseSource, connectionString);

            try
            {
                databaseConnection.OpenConnection();
            }
            catch (SqlException exception)
            {
                databaseConnection.CloseConnection();

                Logger.LogError(exception, DataEngineMessages.DATABASE_CONNECTION_FAILED, databaseSource,
                    databaseConnection.InternalConnection.DataSource, databaseConnection.InternalConnection.Database);

                throw new DatabaseConnectionException();
            }

            var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
            if (!databaseConnectionValidator.TestDatabaseConnection())
            {
                databaseConnection.CloseConnection();
                throw new DatabaseConnectionValidationException(databaseSource);
            }

            databaseConnection.IsConnectionPooled = true;
            _metrics.OnCreate();

            return databaseConnection;
        }

        /// <summary>
        /// Initializes the connections for a specific database configuration
        /// </summary>
        /// <param name="databaseConfiguration">Database configuration model</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void InitializeConnections()
        {
            DatabaseSource databaseSource = _databaseConfiguration!.DatabaseSource;

            try
            {
                for (var index = 0; index < _databaseConfiguration.MinPoolConnections; index++)
                {
                    lock(_mutex)
                    {
                        var databaseConnection = InitializeConnection(databaseSource);
                        _databaseConnectionsQueue.Enqueue(databaseConnection);

                        Monitor.Pulse(_mutex);
                    }
                }
            }
            catch (DatabaseConnectionException exception)
            {
                if (databaseSource.IsRequired())
                {
                    throw;
                }
                else
                {
                    Logger.LogWarning(exception, DataEngineMessages.FAILED_TO_ESTABLISH_CONNECTION_TO_NO_REQUIRED_DATABASE, databaseSource);
                    return;
                }
            }
        }

        /// <summary>
        /// Initializes a number of database connections
        /// </summary>
        public void Initialize(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;

            if (_databaseConfiguration is null)
                throw new NullReferenceException(nameof(databaseConfiguration));

            InitializeConnections();

            this._metricsLogTimer.Interval = _METRICS_SNAPSHOT_INTERVAL;
            this._metricsLogTimer.AutoReset = true;
            this._metricsLogTimer.Elapsed += this.OnMetricsSnapshot;
            this._metricsLogTimer.Enabled = true;

            this._closeIdleConnectionsTimer.Interval = _databaseConfiguration!.CloseIdleConnectionsTimeout;
            this._closeIdleConnectionsTimer.AutoReset = true;
            this._closeIdleConnectionsTimer.Elapsed += this.OnCloseIdleConnections;
            this._closeIdleConnectionsTimer.Enabled = true;
        }

        /// <summary>
        /// Shuts down the pool closing all connections.
        /// </summary>
        public void ShutDown()
        {
            lock (_mutex)
            {
                foreach (DatabaseConnection databaseConnection in this._databaseConnectionsQueue)
                {
                    databaseConnection.CloseConnection();
                    _metrics.OnClose();
                }

                _databaseConnectionsQueue.Clear();
            }
        }

        /// <summary>
        /// Releases the resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method should be called when the instance is no longer needed to free up resources.  It
        /// suppresses finalization to prevent the garbage collector from calling the finalizer.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method should be called when the instance is no longer needed to ensure that all resources 
        /// are properly released. Once disposed, the instance should not be used further.</remarks>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    ShutDown();

                _isDisposed = true;
            }
        }
    }
}
