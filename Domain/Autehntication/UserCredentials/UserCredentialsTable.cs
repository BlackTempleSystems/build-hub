using BuildHub.DataEngine.Tables.Base;
using BuildHub.Domain.Autehntication.UserCredentials.Entities;

namespace BuildHub.Domain.Autehntication.UserCredentials
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
