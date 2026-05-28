using BuildHub.Application.Services.Authentication.Jwt;
using BuildHub.Application.Services.Authentication.Jwt.Models;

namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents a pair of tokens used for authentication or authorization operations.
	/// </summary>
	public sealed class TokenPairModel
	{
		/// <summary>
		/// Gets or sets the JSON Web Token (JWT) associated with the current context.
		/// </summary>
		public JwtModel? Jwt { get; set; }

		/// <summary>
		/// Gets or sets the refresh token associated with the current authentication session.
		/// </summary>
		public RefreshTokenModel? RefreshToken { get; set; }
	}
}
