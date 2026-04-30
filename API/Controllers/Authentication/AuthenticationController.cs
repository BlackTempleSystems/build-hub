using BuildHub.API.Authentication.Models;
using BuildHub.API.Controllers.Base;
using BuildHub.Domain.Store;
using BuildHub.Infrastructure.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildHub.API.Controllers.Authentication;

/// <summary>
/// Provides authentication endpoints for user login, token refresh, and logout operations using JWT access and refresh
/// tokens.
/// </summary>
/// <remarks>This controller exposes endpoints for authenticating users, issuing and rotating JWT tokens, and
/// revoking refresh tokens. All endpoints are accessible without authentication except for protected actions such as
/// 'Ping'. Token-based authentication is used for securing API access. The controller is intended for use in stateless
/// authentication scenarios where clients manage access and refresh tokens.</remarks>
public class AuthenticationController : BaseApiController
{
	private readonly TokenService _tokenService;
	private readonly IRefreshTokenStore _refresh;

	public AuthenticationController(TokenService tokenService, IRefreshTokenStore refresh)
	{
		_tokenService = tokenService;
		_refresh = refresh;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
	{
		return ApiOk(new LoginResponse());
	}
}
