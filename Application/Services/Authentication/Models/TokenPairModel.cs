using BuildHub.Application.Services.Authentication.Jwt;
using BuildHub.Application.Services.Authentication.Jwt.Models;

namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents a pair of tokens used for authentication or authorization operations.
	/// </summary>
	public sealed class TokenPairModel
	{
		public JwtModel? Jwt { get; set; }

		public RefreshTokenModel? RefreshToken { get; set; }
	}
}
