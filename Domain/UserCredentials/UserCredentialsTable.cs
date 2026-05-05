using BuildHub.DataEngine.Tables.Base;
using BuildHub.Domain.UserCredentials.Entities;

namespace BuildHub.Domain.UserCredentials
{
	/// <summary>
	/// Table class for user credentials.
	/// </summary>
	public class UserCredentialsTable : BaseTable<UserCredentialsEntity>
	{
		public UserCredentialsTable()
			: base(DataEngine.DatabaseConnection.DatabaseSource.Users)
		{
		}
	}
}
