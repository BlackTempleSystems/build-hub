using BuildHub.API.Auth.Models;
using BuildHub.API.Controllers.Base;
using BuildHub.Domain.Store;
using BuildHub.Domain.Users;
using BuildHub.Infrastructure.Auth.Security;
using BuildHub.Infrastructure.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildHub.API.Controllers;

public class AuthController : BaseApiController
{
	private readonly IUserValidator _users;
	private readonly TokenService _tokens;
	private readonly IRefreshTokenStore _refresh;

	public AuthController(IUserValidator users, TokenService tokens, IRefreshTokenStore refresh)
	{
		_users = users;
		_tokens = tokens;
		_refresh = refresh;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult<TokenPairResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
	{
		var user = await _users.ValidateAsync(req.UsernameOrEmail, req.Password, ct);
		if (user is null) return Unauthorized();

		var (accessToken, expiresInSeconds) = _tokens.CreateAccessToken(user);
		var (refreshToken, refreshExpiresAt) = _tokens.CreateRefreshToken();

		var refreshRecord = new RefreshTokenRecord(
		  TokenHash: CryptoUtility.Sha256Base64Url(refreshToken),
		  UserId: user.UserId,
		  ExpiresAt: refreshExpiresAt,
		  CreatedAt: DateTimeOffset.UtcNow,
		  RevokedAt: null,
		  ReplacedByTokenHash: null
		);

		await _refresh.StoreAsync(refreshRecord, ct);

		return Ok(new TokenPairResponse(
		  AccessToken: accessToken,
		  ExpiresInSeconds: expiresInSeconds,
		  RefreshToken: refreshToken
		));
	}

	[HttpPost("refresh")]
	[AllowAnonymous]
	public async Task<ActionResult<TokenPairResponse>> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
	{
		// Clean old tokens occasionally (cheap for in-memory; for DB do this via job)
		await _refresh.CleanupExpiredAsync(ct);

		// Consume the old refresh token (one-time use)
		var oldRecord = await _refresh.TryConsumeForRotationAsync(req.RefreshToken, ct);
		if (oldRecord is null) return Unauthorized();

		// Build a "user" shape for token claims.
		// In real implementation, you'd load user details by oldRecord.UserId.
		// For now, we’ll mint minimal claims and placeholder extras.
		var user = new UserRecord(
		  UserId: oldRecord.UserId,
		  Email: "unknown@local",
		  DisplayName: oldRecord.UserId,
		  Role: "User"
		);

		var (accessToken, expiresInSeconds) = _tokens.CreateAccessToken(user);

		// Issue a NEW refresh token (rotation)
		var (newRefreshToken, newRefreshExpiresAt) = _tokens.CreateRefreshToken();
		var newHash = CryptoUtility.Sha256Base64Url(newRefreshToken);

		// Store new refresh token
		var newRecord = new RefreshTokenRecord(
		  TokenHash: newHash,
		  UserId: oldRecord.UserId,
		  ExpiresAt: newRefreshExpiresAt,
		  CreatedAt: DateTimeOffset.UtcNow,
		  RevokedAt: null,
		  ReplacedByTokenHash: null
		);
		await _refresh.StoreAsync(newRecord, ct);

		// Update old record to point to the new one (useful for audit/reuse detection later)
		var updatedOld = oldRecord with { ReplacedByTokenHash = newHash };
		await _refresh.StoreAsync(updatedOld, ct);

		return Ok(new TokenPairResponse(
		  AccessToken: accessToken,
		  ExpiresInSeconds: expiresInSeconds,
		  RefreshToken: newRefreshToken
		));
	}

	[HttpPost("logout")]
	[AllowAnonymous]
	public async Task<IActionResult> Logout([FromBody] RefreshRequest req, CancellationToken ct)
	{
		await _refresh.RevokeAsync(req.RefreshToken, ct);
		return NoContent();
	}

	// Example protected endpoint (optional)
	[HttpGet("ping")]
	[Authorize]
	public IActionResult Ping() => Ok(new { ok = true });
}
