using BuildHub.DataEngine.Configuration;

namespace BuildHub.DataEngine.DatabaseConnection
{
    /// <summary>
    /// Represents a pool for managing and reusing database connections from one or more data sources.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for providing efficient allocation and release of
    /// database connections. Connection pooling can improve performance by reducing the overhead of repeatedly opening and
    /// closing connections. Thread safety and connection lifetime management are typically handled by the pool
    /// implementation.</remarks>
    public interface IDatabaseConnectionPool
    {
        public DatabaseConnectionPoolMetrics Metrics { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        public void Initialize(DatabaseConfiguration databaseConfiguration);

        /// <summary>
        /// Returns a reference to a database connection
        /// </summary>
        /// <returns>DatabaseConnection</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DatabaseConnection GetDatabaseConnection();

        /// <summary>
        /// Returns a database connection to the pool
        /// </summary>
        /// <param name="databaseConnection"></param>
        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection);

        /// <summary>
        /// Shuts down the pool closing all connections.
        /// </summary>
        /// <remarks>Call this method to initiate a controlled shutdown sequence. After calling this
        /// method, the application may no longer be available for further operations. The exact shutdown behavior
        /// depends on the implementation.</remarks>
        void ShutDown();
    }
}
