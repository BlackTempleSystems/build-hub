
namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents a request to obtain a new access token using a refresh token.
	/// </summary>
	public sealed record class RefreshTokenRequest
	{
		/// <summary>
		/// Gets or sets the unique identifier for the user.
		/// </summary>
		public Guid UserGuid { get; set; }

		/// <summary>
		/// Gets or sets the refresh token used to obtain a new access token when the current token expires.
		/// </summary>
		public string RefreshToken { get; set; } = string.Empty;
	}
}
