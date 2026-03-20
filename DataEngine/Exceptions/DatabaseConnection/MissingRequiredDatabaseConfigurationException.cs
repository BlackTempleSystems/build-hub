using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	/// <summary>
	/// Exception describing the scenario where a database configuration is missing.
	/// </summary>
	public sealed class MissingRequiredDatabaseConfigurationException : Exception
	{
		public MissingRequiredDatabaseConfigurationException()
			: base("Missing [REQUIRED] database configuration.")
		{
		}

		public MissingRequiredDatabaseConfigurationException(DatabaseSource databaseSource)
			: base($"Missing [REQUIRED] database configuration - [{databaseSource}].")
		{
		}
	}
}
