using BuildHub.Common.Configuration.Base;
using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Configuration
{
	public sealed record class DatabaseConfiguration : IConfigurationModel
	{
		/// <summary>
		/// Gets or sets the source configuration for the database connection.
		/// </summary>
		public DatabaseSource DatabaseSource { get; set; }
		/// <summary>
		/// Gets or sets the minimum number of connections to maintain in the connection pool.
		/// </summary>
		/// <remarks>Increasing this value can help reduce connection acquisition latency under load by ensuring that
		/// a baseline number of connections are always available. Setting this value too high may result in unnecessary
		/// resource usage if the application does not require many concurrent connections.</remarks>
		public int MinPoolConnections { get; set; }
		/// <summary>
		/// Gets or sets the maximum number of connections allowed in the connection pool.
		/// </summary>
		/// <remarks>Setting this property limits the total number of simultaneous connections that can be pooled.
		/// Exceeding this limit may cause connection requests to wait until a pooled connection becomes available.</remarks>
		public int MaxPoolConnections { get; set; }
		/// <summary>
		/// Gets or sets the number of times to retry retrieving a connection after a failure.
		/// </summary>
		public int RetrieveConnectionRetryCount { get; set; }
		/// <summary>
		/// Gets or sets the timeout, in seconds, to wait when establishing a connection before the attempt is aborted.
		/// </summary>
		public int RetrieveConnectionTimeout { get; set; }

		/// <summary>
		/// The threshold in percentage which indicates whether we need to scale the pool up.
		/// </summary>
        public double PoolGrowthThreshold { get; set; }

		/// <summary>
		/// How many connections will be opened at once if the pool is about to be scaled up.
		/// </summary>
        public int PoolGrowthStep { get; set; }
    }
}
