using BuildHub.DataEngine.Entities;

namespace BuildHub.Domain.Autehntication.RefreshTokens.Entities
{
	/// <summary>
	/// A user's refresh token entity
	/// </summary>
	[TableName("REFRESH_TOKENS")]
	public sealed class RefreshTokenEntity : VersionedEntity
	{
		[ColumnInfo("USER_ID")]
		public int UserId { get; set; }

		[ColumnInfo("REFRESH_TOKEN")]
		public string RefreshToken { get; set; } = string.Empty;

		[ColumnInfo("EXPIRATION_DATE")]
		public DateTime ExpirationDate { get; set; }

		[ColumnInfo("IS_REVOKED")]
		public bool IsRevoked { get; set; } = false;
	}
}
