using BuildHub.Common.Configuration.Base;

namespace BuildHub.Application.Services.Authentication.Jwt.Configuration
{
	/// <summary>
	/// Represents configuration options for issuing and validating JSON Web Tokens (JWT) within an application.
	/// </summary>
	/// <remarks>Use this class to specify the issuer, audience, signing key, and token lifetimes when configuring JWT
	/// authentication. All properties are required for correct token generation and validation.</remarks>
	public sealed class JwtOptions : IConfigurationModel
	{
		/// <summary>
		/// Gets the identifier of the entity that issued the token or credential.
		/// </summary>
		public string Issuer { get; init; } = default!;

		/// <summary>
		/// Gets the intended audience for the token or resource.
		/// </summary>
		public string Audience { get; init; } = default!;

		/// <summary>
		/// Gets the security key used for authentication or encryption purposes.
		/// </summary>
		public string SecurityKey { get; init; } = default!;

		/// <summary>
		/// Gets the expiration period, in minutes, for the associated operation or item.
		/// </summary>
		public int ExpirationInMinutes { get; init; } = 20;

		/// <summary>
		/// Gets the number of days for which a refresh token remains valid.
		/// </summary>
		public int RefreshTokenDays { get; init; } = 14;
	}

}
