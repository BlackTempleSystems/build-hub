using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.DatabaseConnection
{
	#region
	using Exceptions.DatabaseConnection;
	using DataEngine.Configuration;
	using Common.Utilities;
	using Common.Logger;
	#endregion

	using DatabaseConfigurationsMap = Dictionary<DatabaseSource, Configuration.DatabaseConfiguration>;
    using BuildHub.DataEngine.Extensions;

    /// <summary>
    /// Database connection pool singleton, initializing and managing a number of database connections.
    /// </summary>
    public sealed class DatabaseConnectionPool : IDisposable
	{
		private static readonly Lazy<DatabaseConnectionPool> _databaseConnectionPoolInstance = new Lazy<DatabaseConnectionPool>(() => new DatabaseConnectionPool());

		/// <summary>
		/// Database configuration manager instance
		/// </summary>
		private DatabaseConfigurationManager _configurationManager;

		/// <summary>
		/// Represents a collection of database connection configurations.
		/// </summary>
		/// <remarks>This field holds the configurations for connecting to one or more databases.  It is intended for
		/// internal use and should not be accessed directly outside of the containing class.</remarks>
		private DatabaseConfigurationsMap _databaseConfigurationsMap;
		/// <summary>Available connection ready for use</summary>
		private readonly Dictionary<DatabaseSource, Queue<DatabaseConnection>> _availableDatabaseConnectionsMap;

		/// <summary>
		/// An object used to synchronize access to shared resources in a multi-threaded environment.
		/// </summary>
		/// <remarks>This field is intended to be used as a locking mechanism to ensure thread safety when accessing
		/// or modifying shared data. Always use this object with a <c>lock</c> statement to avoid race conditions.</remarks>
		private readonly object _mutex = new object();

		private bool _isDisposed;

		private DatabaseConnectionPool()
		{
			this._configurationManager = DatabaseConfigurationManager.GetInstance();
			this._availableDatabaseConnectionsMap = new Dictionary<DatabaseSource, Queue<DatabaseConnection>>();
			this._databaseConfigurationsMap = new Dictionary<DatabaseSource, DatabaseConfiguration>();
			this._isDisposed = false;

            Initialize();
		}

		~DatabaseConnectionPool() => Dispose(false);

		/// <summary>
		/// Returns an instance to the connection pool
		/// </summary>
		/// <returns>DatabaseConnectionPool</returns>
		public static DatabaseConnectionPool GetInstance() => _databaseConnectionPoolInstance.Value;

		/// <summary>
		/// Returns the number of currently available database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetAvailableDatabaseConnectionsCount(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				if (!_availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var databaseConnections))
					throw new MissingRequiredDatabaseConfigurationException(databaseSource);

				return databaseConnections.Count;
			}
		}
		/// <summary>
		/// Returns the number of currently used database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetCurrentlyUsedConnectionsCount(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				var databaseConfiguration = _databaseConfigurationsMap[databaseSource];
				if (databaseConfiguration is null)
					throw new MissingRequiredDatabaseConfigurationException(databaseSource);

				return databaseConfiguration.MaxPoolConnections - this._availableDatabaseConnectionsMap[databaseSource].Count;
			}
		}

		/// <summary>
		/// Returns a reference to a database connection
		/// </summary>
		/// <returns>DatabaseConnection</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseConnection GetDatabaseConnection(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				int retryCount = 0;
				DatabaseConnection? databaseConnection = null;
				DatabaseConfiguration databaseConfiguration = _databaseConfigurationsMap[databaseSource];

				while (retryCount < databaseConfiguration.RetrieveConnectionRetryCount)
				{
					if (!_availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var availableDatabaseConnections))
						throw new MissingRequiredDatabaseConfigurationException(databaseSource);

					if (availableDatabaseConnections.Count > 0)
					{
						databaseConnection = availableDatabaseConnections.Dequeue();
						break;
					}

                    Monitor.Wait(_mutex, databaseConfiguration.RetrieveConnectionTimeout);
					retryCount++;
				}

				if (databaseConnection is null)
				{
					Logger.LogWarning($"Connection pool exhausted for {databaseSource}.");
					throw new ConnectionPoolExhaustedException(databaseSource);
				}

                databaseConnection.IsConnectionPooled = false;

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
				if (!this._availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var availableDatabaseConnections))
					throw new MissingRequiredDatabaseConfigurationException(databaseSource);

				if (databaseConnection.IsConnectionOpen())
				{
					var databaseConfiguration = this._databaseConfigurationsMap[databaseSource];
					if (databaseConfiguration is null)
						throw new MissingRequiredDatabaseConfigurationException(databaseSource);

					if (availableDatabaseConnections.Count < databaseConfiguration.MaxPoolConnections)
					{
						if (!availableDatabaseConnections.Contains(databaseConnection))
						{
							databaseConnection.IsConnectionPooled = true;
                            availableDatabaseConnections.Enqueue(databaseConnection);

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
			string connectionStringKey = EnumUtilities.GetEnumDescription<DatabaseSource>(databaseSource);
			string connectionString = this._configurationManager.GetConnectionString(connectionStringKey);

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
				Logger.LogError(exception, $"An error occurred while trying to open connection for {databaseSource}");
				throw;
			}

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
			if (!databaseConnectionValidator.TestDatabaseConnection())
			{
				databaseConnection.CloseConnection();
				throw new DatabaseConnectionValidationException(databaseSource);
			}

			return databaseConnection;
		}

		/// <summary>
		/// Initializes the connections for a specific database configuration
		/// </summary>
		/// <param name="databaseConfiguration">Database configuration model</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		private void InitializeConnections(DatabaseConfiguration databaseConfiguration)
		{
			ValidateDatabaseConfiguration(databaseConfiguration);

			var availableDatabaseConnections = new Queue<DatabaseConnection>();
            DatabaseSource databaseSource = databaseConfiguration.DatabaseSource;

            for (var index = 0; index < databaseConfiguration.MinPoolConnections; index++)
			{
				var databaseConnection = InitializeConnection(databaseConfiguration.DatabaseSource);
                databaseConnection.IsConnectionPooled = true;

                availableDatabaseConnections.Enqueue(databaseConnection);
			}

			Logger.LogInformation($"Database connection established successfully - [{databaseSource}]");
			this._availableDatabaseConnectionsMap.Add(databaseConfiguration.DatabaseSource, availableDatabaseConnections);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private void ValidateRequiredDatabaseConfigurations()
		{
            var databaseSources = EnumUtilities.GetEnumValues<DatabaseSource>().Where(databaseSource => databaseSource.IsRequired());

            foreach (var databaseSource in databaseSources)
            {
                if (!_databaseConfigurationsMap.ContainsKey(databaseSource))
				{
                    throw new MissingRequiredDatabaseConfigurationException(databaseSource);
                }
            }
        }

		/// <summary>
		/// Validates the database configuration
		/// </summary>
		/// <param name="databaseConfiguration">The database configuration object</param>
		/// <exception cref="InvalidDatabaseConfigurationException"></exception>
		private void ValidateDatabaseConfiguration(DatabaseConfiguration databaseConfiguration)
		{
            if (databaseConfiguration.MaxPoolConnections < 1)
                throw new InvalidDatabaseConfigurationException("MaxPoolConnections must be at least 1");

            if (databaseConfiguration.MinPoolConnections > databaseConfiguration.MaxPoolConnections)
				throw new InvalidDatabaseConfigurationException($"Invalid database configuration provided " +
					$"for  database: {databaseConfiguration.DatabaseSource} the minimum pool connections are greater than the maximum pool connections.");
		}

		/// <summary>
		/// Initializes a number of database connections
		/// </summary>
		private void Initialize()
		{
            _databaseConfigurationsMap = _configurationManager.GetDatabaseConfigurations();
			ValidateRequiredDatabaseConfigurations();

            foreach (DatabaseConfiguration databaseConfiguration in _databaseConfigurationsMap.Values)
				InitializeConnections(databaseConfiguration);

			Logger.LogInformation("Database connection pool was initialized successfully.");
		}

		/// <summary>
		/// Closes all connections.
		/// </summary>
		private void Cleanup()
		{
			lock (_mutex)
			{
				foreach (var databaseConnections in this._availableDatabaseConnectionsMap.Values)
				{
					foreach (DatabaseConnection databaseConnection in databaseConnections)
						databaseConnection.CloseConnection();

					databaseConnections.Clear();
				}
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
					Cleanup();

				_isDisposed = true;
			}
		}
	}
}
