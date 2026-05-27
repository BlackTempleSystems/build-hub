using BuildHub.Application.Services.Authentication.Jwt.Models;
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
		/// Verifies whether the specified raw refresh token matches the provided hashed token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to validate. Cannot be null or empty.</param>
		/// <param name="hashedToken">The hashed representation of the refresh token to compare against. Cannot be null or empty.</param>
		/// <returns>true if the raw token matches the hashed token; otherwise, false.</returns>
		public bool VerifyfRefreshToken(string rawRefreshToken, string hashedRefreshToken);

		/// <summary>
		/// Replaces the specified refresh token with a new one, invalidating the original token.
		/// </summary>
		/// <param name="refreshTokenEntity">The refresh token entity to be rotated. Cannot be null.</param>
		/// <returns>true if the refresh token was successfully rotated; otherwise, false.</returns>
		public RefreshTokenModel? RotateRefreshToken(string rawRefreshToken, UserEntity userEntity);
	}
}
