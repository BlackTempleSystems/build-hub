using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	/// <summary>
	/// 
	/// </summary>
	public class InvalidDatabaseConfigurationException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public DatabaseSource DatabaseSource { get; private set; }

		public InvalidDatabaseConfigurationException(string message, DatabaseSource databaseSource)
			: base(message)
		{
			this.DatabaseSource = databaseSource;
		}
	}
}
