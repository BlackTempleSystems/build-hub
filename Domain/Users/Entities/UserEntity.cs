using BuildHub.DataEngine.Entities;
using System.ComponentModel.DataAnnotations;

namespace BuildHub.Domain.Users.Entities
{
	/// <summary>
	/// Represents a user entity within the system.
	/// </summary>
	/// <remarks>This class is a sealed type that inherits from BaseEntity and is intended to encapsulate
	/// user-related data. It cannot be inherited.</remarks>
	[TableName("USERS")]
	public sealed class UserEntity : VersionedEntity
	{
		[MaxLength(128)]
		[ColumnInfo("FIRST_NAME")]
		public string FirstName { get; set; } = string.Empty;

		[MaxLength(128)]
		[ColumnInfo("LAST_NAME")]
		public string LastName { get; set; } = string.Empty;

		[MaxLength(254)]
		[ColumnInfo("EMAIL")]
		public string Email { get; set; } = string.Empty;

		[MaxLength(32)]
		[ColumnInfo("USER_NAME")]
		public string UserName { get; set; } = string.Empty;

		[ColumnInfo("FAILED_LOGIN_ATTEMPTS")]
		public int FailedLoginAttempts { get; set; }

		public UserEntity()
		{
		}
	}
}
