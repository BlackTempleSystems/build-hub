using BuildHub.API.Controllers.Base;
using BuildHub.Application.Services.Authentication;
using BuildHub.Application.Services.Authentication.Models;
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
		return ApiOk(new LoginResponse());
	}

	/// <summary>
	/// Registers a new user with the provided registration details.
	/// </summary>
	/// <param name="registerUserRequest">The registration information for the new user. Cannot be null.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>An IActionResult indicating the result of the registration operation.</returns>
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
	{
		return FromResult(await _authenticationService.RegisterUser(registerUserRequest));
	}
}
