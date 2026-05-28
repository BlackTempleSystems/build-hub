#region
using BuildHub.Application.Messages;
using BuildHub.Application.Services.Authentication.Jwt.Configuration;
using BuildHub.Application.Services.Authentication.Jwt.Models;
using BuildHub.Common.Configuration;
using BuildHub.Common.Logger;
using BuildHub.Domain.Autehntication.RefreshTokens;
using BuildHub.Domain.Autehntication.RefreshTokens.Entities;
using BuildHub.Domain.Autehntication.Users.Entities;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace BuildHub.Application.Services.Authentication.RefreshToken
{
	/// <summary>
	/// Provides functionality for generating, validating, and rotating refresh tokens used in authentication workflows.
	/// </summary>
	/// <remarks>This service manages the lifecycle of refresh tokens, including secure generation, validation, and
	/// rotation for user authentication scenarios. It is intended to be used as part of a token-based authentication
	/// system to enable secure session renewal without requiring users to re-authenticate with their credentials. The
	/// service is not thread-safe and should be used accordingly in multi-threaded environments.</remarks>
	public sealed class RefreshTokenService : IRefreshTokenService
	{

		private sealed class RefreshTokenGenerationResult
		{
			public string RawToken { get; private set; }
			public string HashedToken { get; private set; }
			public DateTime ExpirationDate { get; private set; }

			public RefreshTokenGenerationResult(string rawToken, string hashedToken, DateTime expirationDate)
			{
				this.RawToken = rawToken;
				this.HashedToken = hashedToken;
				this.ExpirationDate = expirationDate;
			}
		}


		/// <summary>
		/// Refresh token byte size.
		/// </summary>
		private const int _RefreshTokenBytesSize = 64;

		/// <summary>
		/// Key to access the jwt section.
		/// </summary>
		private const string _JwtSectionKey = "Jwt";

		/// <summary>
		/// Configuration manager instance.
		/// </summary>
		private readonly ConfigurationManager _configurationManager;

		public RefreshTokenService()
		{
			this._configurationManager = ConfigurationManager.GetConfigurationManager();
		}

		/// <summary>
		/// Generates a new refresh token for use in authentication workflows.
		/// </summary>
		/// <returns>A <see cref="RefreshTokenModel"/> instance containing the newly generated refresh token and its associated
		/// metadata.</returns>
		public RefreshTokenModel GenerateAndSaveRefreshToken(UserEntity userEntity)
		{
			ArgumentNullException.ThrowIfNull(userEntity);

			var refreshToken = GenerateNewRefreshToken();

			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();
			RefreshTokenEntity? refreshTokenEntity = refreshTokensTable.GetByCondition(token => token.UserId, userEntity.Id).FirstOrDefault();

			if (refreshTokenEntity is null)
			{
				refreshTokenEntity = new RefreshTokenEntity();
				refreshTokenEntity.RefreshToken = refreshToken.HashedToken;
				refreshTokenEntity.ExpirationDate = refreshToken.ExpirationDate;
				refreshTokenEntity.UserId = userEntity.Id;

				if (refreshTokensTable.Insert(refreshTokenEntity) is null)
				{
					Logger.LogError(ApplicationMessages.RefreshTokenPersistenceFailed);
					throw new InvalidOperationException(ApplicationMessages.RefreshTokenPersistenceFailed);
				}
			}
			else
			{
				refreshTokenEntity.IsRevoked = false;
				refreshTokenEntity.RefreshToken = refreshToken.HashedToken;
				refreshTokenEntity.ExpirationDate = refreshToken.ExpirationDate;

				if (!refreshTokensTable.Update(refreshTokenEntity))
				{
					Logger.LogError(ApplicationMessages.RefreshTokenPersistenceFailed);
					throw new InvalidOperationException(ApplicationMessages.RefreshTokenPersistenceFailed);
				}
			}

			return new RefreshTokenModel
			{
				RefreshToken = refreshToken.RawToken,
				ExpirationDate = refreshToken.ExpirationDate
			};
		}

		/// <summary>
		/// Verifies whether the specified raw refresh token matches the provided hashed token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to validate. Cannot be null or empty.</param>
		/// <param name="hashedToken">The hashed representation of the refresh token to compare against. Cannot be null or empty.</param>
		/// <returns>true if the raw token matches the hashed token; otherwise, false.</returns>
		public bool VerifyRefreshToken(string rawRefreshToken, string hashedRefreshToken)
		{
			if (string.IsNullOrWhiteSpace(rawRefreshToken) || string.IsNullOrWhiteSpace(hashedRefreshToken))
				return false;

			string submittedTokenHash = HashRefreshToken(rawRefreshToken);
			return CryptographicOperations.FixedTimeEquals(
				Encoding.UTF8.GetBytes(submittedTokenHash),
				Encoding.UTF8.GetBytes(hashedRefreshToken));
		}

		/// <summary>
		/// Replaces the specified refresh token with a new one, invalidating the original token.
		/// </summary>
		/// <param name="refreshTokenEntity">The refresh token entity to be rotated. Cannot be null.</param>
		/// <returns>true if the refresh token was successfully rotated; otherwise, false.</returns>
		public RefreshTokenModel? RotateRefreshToken(string rawRefreshToken, UserEntity userEntity)
		{
			ArgumentNullException.ThrowIfNull(userEntity);

			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();

			var refreshTokenEntity = GetRefreshTokenForUser(userEntity.Id);
			if (refreshTokenEntity is null)
			{
				Logger.LogWarning(ApplicationMessages.RefreshTokenLookupFailed);
				return null;
			}

			if (refreshTokenEntity.IsRevoked)
			{
				Logger.LogWarning(ApplicationMessages.RefreshTokenRevoked);
				return null;
			}

			if (refreshTokenEntity.ExpirationDate < DateTime.UtcNow)
			{
				Logger.LogWarning(ApplicationMessages.RefreshTokenExpired);
				return null;
			}

			if (!VerifyRefreshToken(rawRefreshToken, refreshTokenEntity.RefreshToken))
			{
				Logger.LogWarning(ApplicationMessages.RefreshTokenVerificationFailed);
				return null;
			}

			var refreshToken = GenerateNewRefreshToken();
			refreshTokenEntity.RefreshToken = refreshToken.HashedToken;
			refreshTokenEntity.ExpirationDate = refreshToken.ExpirationDate;
			refreshTokenEntity.IsRevoked = false;

			if (!refreshTokensTable.Update(refreshTokenEntity))
			{
				Logger.LogError(ApplicationMessages.RefreshTokenPersistenceFailed);
				return null;
			}

			return new RefreshTokenModel()
			{
				RefreshToken = refreshToken.RawToken,
				ExpirationDate = refreshToken.ExpirationDate
			};
		}

		/// <summary>
		/// Retrieves the refresh token associated with the specified user.
		/// </summary>
		/// <param name="userId">The unique identifier of the user whose refresh token is to be retrieved. Must be a positive integer.</param>
		/// <returns>A <see cref="RefreshTokenModel"/> containing the refresh token information for the user, or <c>null</c> if no
		/// refresh token exists for the specified user.</returns>
		private RefreshTokenEntity? GetRefreshTokenForUser(int userId)
		{
			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();
			return refreshTokensTable.GetByCondition(rToken => rToken.UserId, userId).FirstOrDefault();
		}

		/// <summary>
		/// Generates a hashed representation of the specified refresh token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to be hashed. Cannot be null or empty.</param>
		/// <returns>A string containing the hashed value of the refresh token.</returns>
		private string HashRefreshToken(string rawRefreshToken)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken));
			return Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Generates a new refresh token and its expiration time.
		/// </summary>
		/// <returns>A tuple containing the newly generated refresh token as a string and its expiration date and time as a <see
		/// cref="DateTime"/> value.</returns>
		private RefreshTokenGenerationResult GenerateNewRefreshToken()
		{
			Span<byte> randomBytes = stackalloc byte[_RefreshTokenBytesSize];
			using var randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(randomBytes);

			var jwtOptions = _configurationManager.GetConfigurationModel<JwtOptions>(_JwtSectionKey);

			if (jwtOptions is null)
			{
				Logger.LogError(ApplicationMessages.RefreshTokenConfigurationMissing);
				throw new InvalidOperationException(ApplicationMessages.RefreshTokenConfigurationMissing);
			}

			var refreshToken = Convert.ToBase64String(randomBytes);
			var expirationDate = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenDays);

			var hashedRefreshToken = HashRefreshToken(refreshToken);

			return new RefreshTokenGenerationResult(refreshToken, hashedRefreshToken, expirationDate);
		}
	}
}
