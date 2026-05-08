using BuildHub.Application.Services.Authentication.Jwt.Configuration;
using BuildHub.Common.Configuration;
using BuildHub.Domain.Users.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildHub.Application.Services.Authentication.Jwt;

/// <summary>
/// Provides functionality for generating JSON Web Tokens (JWT) and refresh tokens for user authentication.
/// </summary>
/// <remarks>This service encapsulates the logic for creating access and refresh tokens based on user information
/// and application configuration. It is intended to be used as a singleton for issuing tokens in authentication
/// workflows. The service requires valid JWT configuration settings to operate correctly.</remarks>
public sealed class JwtService : IJwtService
{
	/// <summary>
	/// Key to access the jwt section.
	/// </summary>
	private const string JwtSectionKey = "Jwt";

	/// <summary>
	/// Configuration manager instance.
	/// </summary>
	private readonly ConfigurationManager _configurationManager;

	public JwtService()
	{
		this._configurationManager = ConfigurationManager.GetConfigurationManager();
	}

	/// <summary>
	/// Generates a collection of claims for the specified user to be included in a JWT token.
	/// </summary>
	/// <remarks>The returned claims are suitable for use in JWT token generation. Additional claims, such as user
	/// roles, may be added in the future.</remarks>
	/// <param name="user">The user entity for which to generate claims. Cannot be null.</param>
	/// <returns>An enumerable collection of claims representing the user's identity and email address. The collection includes a
	/// unique identifier and email claim for the user.</returns>
	private IEnumerable<Claim> GetClaims(UserEntity user)
	{
		return
			[
				new Claim(JwtRegisteredClaimNames.Sub, user.Guid.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email!),
				new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				//TODO Roles.
			];
	}

	/// <summary>
	/// Generates a JSON Web Token (JWT) for the specified user based on the current security configuration.
	/// </summary>
	/// <remarks>The generated token includes issuer, audience, expiration, and claims as defined in the security
	/// configuration. The caller is responsible for securely storing and transmitting the token as appropriate.</remarks>
	/// <param name="user">The user entity for which to generate the security token. Must contain the necessary claims and identity
	/// information.</param>
	/// <returns>A string containing the generated JWT. The token includes claims for the specified user and is signed using the
	/// configured security key.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the JWT configuration is missing or invalid.</exception>
	public JwtModel GenerateSecurityToken(UserEntity user)
	{
		var jwtOptions = _configurationManager.GetConfigurationModel<JwtOptions>(JwtSectionKey);

		if (jwtOptions is null)
			throw new InvalidOperationException();

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
		var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var currentDateTime = DateTime.UtcNow;
		var expirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(GetClaims(user)),
			Expires = expirationDate,
			SigningCredentials = signingCredentials,
			Issuer = jwtOptions.Issuer,
			Audience = jwtOptions.Audience,
			IssuedAt = currentDateTime,
			NotBefore = currentDateTime,
		};

		var jwtTokenHandler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
		var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);

		var jwtModel = new JwtModel()
		{
			AccessToken = securityToken,
			ExpirationDate = expirationDate
		};

		return jwtModel;
	}
}