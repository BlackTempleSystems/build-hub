using BuildHub.Domain.Users;
using BuildHub.Infrastructure.Auth.Configuration;
using BuildHub.Infrastructure.Auth.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildHub.Infrastructure.Auth.Services;

public sealed class TokenService
{
	private readonly JwtOptions _opts;
	private readonly SymmetricSecurityKey _signingKey;

	public TokenService(IConfiguration cfg)
	{
		_opts = cfg.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt config missing");
		_signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
	}

	public (string Token, int ExpiresInSeconds) CreateAccessToken(UserRecord user)
	{
		var now = DateTime.UtcNow;
		var expires = now.AddMinutes(_opts.AccessTokenMinutes);

		var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

		// Required/asked claims: sub, email, jti
		var claims = new List<Claim>
		{
		  new(JwtRegisteredClaimNames.Sub, user.UserId),
		  new(JwtRegisteredClaimNames.Email, user.Email),
		  new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

		  // Useful extras
		  new(JwtRegisteredClaimNames.UniqueName, user.DisplayName),
		  new(ClaimTypes.Role, user.Role),
		  new("name", user.DisplayName)
		};

		var jwt = new JwtSecurityToken(
		  issuer: _opts.Issuer,
		  audience: _opts.Audience,
		  claims: claims,
		  notBefore: now,
		  expires: expires,
		  signingCredentials: creds
		);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
		return (tokenString, (int)TimeSpan.FromMinutes(_opts.AccessTokenMinutes).TotalSeconds);
	}

	public (string RefreshToken, DateTimeOffset ExpiresAt) CreateRefreshToken()
	{
		var refreshToken = CryptoUtility.GenerateSecureToken(48);
		var expiresAt = DateTimeOffset.UtcNow.AddDays(_opts.RefreshTokenDays);
		return (refreshToken, expiresAt);
	}
}