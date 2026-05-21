using BuildHub.DataEngine.Entities;

namespace BuildHub.Domain.Autehntication.UserCredentials.Entities
{
	/// <summary>
	/// User credentials entity.
	/// </summary>
	[TableName("USER_CREDENTIALS")]
	public sealed class UserCredentialsEntity : VersionedEntity
	{
		[ColumnInfo("USER_ID")]
		public int UserId { get; set; }

		[ColumnInfo("HASHED_PASSWORD")]
		public string? HashedPassword { get; set; }
	}
	
}
