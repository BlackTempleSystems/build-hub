using System.Text.Json.Serialization;

namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents the response returned after a successful refresh token operation.
	/// </summary>
	public sealed record class RefreshTokenResponse
	{
		/// <summary>
		/// Gets or sets the current access and refresh token pair used for authentication.
		/// </summary>
		[JsonIgnore]
		public TokenPairModel? TokenPair { get; set; }
	}
}
