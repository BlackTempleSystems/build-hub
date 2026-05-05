using BuildHub.DataEngine.DatabaseConnection;
using BuildHub.DataEngine.Tables.Base;
using BuildHub.Domain.Users.Entities;

namespace BuildHub.Domain.Users
{
	/// <summary>
	/// Represents a strongly-typed table for managing user entities in the data store.
	/// </summary>
	/// <remarks>Inherits common table operations from the BaseTable class, providing functionality specific to
	/// UserEntity objects. This class is sealed and cannot be inherited.</remarks>
	public sealed class UsersTable : BaseTable<UserEntity>
	{
		public UsersTable()
			: base(DatabaseSource.Users)
		{
		}
	}
}
