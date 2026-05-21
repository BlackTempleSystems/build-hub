using BuildHub.API.Controllers.Base;
using BuildHub.Application.Services.Authentication;
using BuildHub.Application.Services.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
	/// <summary>
	/// Authentication service.
	/// </summary>
	private readonly IAuthenticationService _authenticationService;

	public AuthenticationController(IAuthenticationService authenticationService)
	{
		this._authenticationService = authenticationService;
	}

	/// <summary>
	/// Authenticates a user based on the provided login credentials and returns a response indicating the result of the
	/// authentication attempt.
	/// </summary>
	/// <param name="loginRequest">The login credentials and related information required to authenticate the user. Cannot be null.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the login operation.</param>
	/// <returns>An IActionResult containing the authentication result. Returns a success response with authentication details if
	/// the credentials are valid.</returns>
	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
	{
		var loginResponse = await _authenticationService.LoginAsync(loginRequest);
		if (loginResponse.IsSuccess && loginResponse.Data is not null)
		{
			var jwt = loginResponse.Data.Jwt;

			Response.Cookies.Append("access_token", jwt.AccessToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = jwt?.ExpirationDate
			});
		}

		return FromResult(loginResponse);
	}

	/// <summary>
	/// Registers a new user with the provided registration details.
	/// </summary>
	/// <param name="registerUserRequest">The registration information for the new user. Cannot be null.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>An IActionResult indicating the result of the registration operation.</returns>
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register([FromBody] RegisterRequest registerUserRequest, CancellationToken cancellationToken)
	{
		var registerResponse = await _authenticationService.RegisterAsync(registerUserRequest);
		if(registerResponse.IsSuccess && registerResponse.Data is not null)
		{
			var jwt = registerResponse.Data.Jwt;
			var refreshToken  = registerResponse.Data.RefreshToken;

			Response.Cookies.Append("access_token", jwt.AccessToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = jwt?.ExpirationDate
			});

			Response.Cookies.Append("refresh_token", refreshToken.RefreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = refreshToken?.ExpirationDate
			});
		}

		return FromResult(registerResponse);
	}

	[Authorize]
	[HttpGet("authenticateUser")]
	public async Task<IActionResult> AuthenticateUser()
	{
		Guid userGuId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

		return FromResult(await _authenticationService.GetUserByGuidAsync(userGuId));
	}
}
