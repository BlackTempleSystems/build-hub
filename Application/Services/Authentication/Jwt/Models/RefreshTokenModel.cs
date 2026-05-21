namespace BuildHub.Application.Services.Authentication.Jwt.Models
{
	/// <summary>
	/// 
	/// </summary>
	public sealed record class RefreshTokenModel
	{
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpirationDate { get; set; }
	}
}
