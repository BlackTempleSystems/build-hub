namespace BuildHub.DataEngine.Messages
{
	/// <summary>
	/// Class holding data-engine related messages and message templates
	/// </summary>
	internal static class DataEngineMessages
	{
		public readonly static string CONNECTION_POOL_INITIALIZED_SUCCESSFULLY = "Database connection pool was initialized successfully.";
		public readonly static string CONNECTION_POOL_EXHAUSTED = "Connection pool exhausted for {DatabaseSource}";
		public readonly static string DATABASE_CONNECTION_FAILED = "Database connection failed | Source: [{DatabaseSource}] | Server: [{Server}] | Database: [{DatabaseName}]";
		public readonly static string FAILED_TO_ESTABLISH_CONNECTION_TO_NO_REQUIRED_DATABASE = "Failed to establish a connection to [NOT REQUIRED] | Source: [{DatabaseSource}]";
		public readonly static string DATABASE_CONNECTION_ESTABLISHED_SUCCESSFULLY = "Database connection established successfully | Source: [{DatabaseSource}]";
		public readonly static string DATABASE_STATISTICS_REPORT = "Database Statistics - Total: {TotalActiveConnections} | Idle: {TotalIdleConnections} | Active: {TotalConnectionsCurrentlyInUse} | Utilization: {Utilization}% | Source: [{DatabaseSource}]";
		public readonly static string CONNECTION_POOL_REGROWN = "Connection pool regrown | Added: [{ConnectionsAdded}] | Total: [{NewTotal}/{MaxConnections}] | Source: [{DatabaseSource}]";
	}
}
