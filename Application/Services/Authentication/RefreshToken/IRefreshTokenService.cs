using BuildHub.Application.Services.Authentication.Jwt.Models;
using BuildHub.Domain.Autehntication.RefreshTokens.Entities;
using BuildHub.Domain.Autehntication.Users.Entities;

namespace BuildHub.Application.Services.Authentication.RefreshToken
{
	/// <summary>
	/// Defines a contract for services that manage refresh tokens used in authentication or authorization workflows.
	/// </summary>
	internal interface IRefreshTokenService
	{
		/// <summary>
		/// Generates a new refresh token for use in authentication workflows and saves it to the database.
		/// </summary>
		/// <returns>A <see cref="RefreshTokenModel"/> instance containing the newly generated refresh token and its associated
		/// metadata.</returns>
		public RefreshTokenModel GenerateAndSaveRefreshToken(UserEntity usersEntity);

		/// <summary>
		/// Generates a hashed representation of the specified refresh token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to be hashed. Cannot be null or empty.</param>
		/// <returns>A string containing the hashed value of the refresh token.</returns>
		public string HashfRefreshToken(string rawRefreshToken);

		/// <summary>
		/// Verifies whether the specified raw refresh token matches the provided hashed token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to validate. Cannot be null or empty.</param>
		/// <param name="hashedToken">The hashed representation of the refresh token to compare against. Cannot be null or empty.</param>
		/// <returns>true if the raw token matches the hashed token; otherwise, false.</returns>
		public bool VerifyfRefreshToken(string rawRefreshToken, string hashedRefreshToken);

		/// <summary>
		/// Retrieves the refresh token associated with the specified user.
		/// </summary>
		/// <param name="userId">The unique identifier of the user whose refresh token is to be retrieved. Must be a positive integer.</param>
		/// <returns>A <see cref="RefreshTokenModel"/> containing the refresh token information for the user, or <c>null</c> if no
		/// refresh token exists for the specified user.</returns>
		public RefreshTokenEntity? GetRefreshTokenForUser(int userId);
	}
}
