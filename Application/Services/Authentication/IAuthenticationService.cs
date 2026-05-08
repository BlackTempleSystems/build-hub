using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Domain.Results;
using BuildHub.Domain.Users.Models;

namespace BuildHub.Application.Services.Authentication
{
	/// <summary>
	/// Defines a contract for authentication services that manage user authentication within an application.
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Authenticates a user based on the provided login request data.
		/// </summary>
		/// <param name="loginRequestData">The login request information containing user credentials to be validated. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous authentication operation.</returns>
		public Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequestData);

		/// <summary>
		/// Registers a new user with the specified registration details.
		/// </summary>
		/// <param name="registerUserRequest">An object containing the information required to register the user. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous registration operation.</returns>
		public Task<Result<RegisterUserResponse>> RegisterAsync(RegisterRequest registerUserRequest);

		/// <summary>
		/// Retrieves the user from the database by guid.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public Task<Result<UserModel>> GetUserByGuidAsync(Guid userGuid);
	}
}
