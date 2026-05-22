#region
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
	public sealed class RefreshTokenService : IRefreshTokenService
	{
		/// <summary>
		/// Refresh token byte size.
		/// </summary>
		private const int _RefershTokenBytesSize = 64;

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
			Span<byte> randomBytes = stackalloc byte[_RefershTokenBytesSize];
			using var randomNumebrGenerator = RandomNumberGenerator.Create();
			randomNumebrGenerator.GetBytes(randomBytes);

			var jwtOptions = _configurationManager.GetConfigurationModel<JwtOptions>(_JwtSectionKey);

			if (jwtOptions is null)
				throw new InvalidOperationException();

			var refreshToken = Convert.ToBase64String(randomBytes);
			var expirationDate = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenDays);

			var hashedRefreshToken = HashfRefreshToken(refreshToken);

			var refreshTokenEntity = new RefreshTokenEntity();
			refreshTokenEntity.RefreshToken = hashedRefreshToken;
			refreshTokenEntity.ExpirationDate = expirationDate;
			refreshTokenEntity.UserId = userEntity.Id;

			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();
			if (refreshTokensTable.Insert(refreshTokenEntity) is null)
			{
				Logger.LogError($"Failed to insert refresh token for userEntity userEntity ID '{userEntity.Id}' during registration.");
				throw new InvalidOperationException();
			}

			return new RefreshTokenModel
			{
				RefreshToken = refreshToken,
				ExpirationDate = expirationDate
			};
		}

		/// <summary>
		/// Generates a hashed representation of the specified refresh token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to be hashed. Cannot be null or empty.</param>
		/// <returns>A string containing the hashed value of the refresh token.</returns>
		public string HashfRefreshToken(string rawRefreshToken)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken));
			return Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Verifies whether the specified raw refresh token matches the provided hashed token.
		/// </summary>
		/// <param name="rawToken">The raw refresh token to validate. Cannot be null or empty.</param>
		/// <param name="hashedToken">The hashed representation of the refresh token to compare against. Cannot be null or empty.</param>
		/// <returns>true if the raw token matches the hashed token; otherwise, false.</returns>
		public bool VerifyfRefreshToken(string rawRefreshToken, string hashedRefreshToken)
		{
			return HashfRefreshToken(rawRefreshToken).Equals(hashedRefreshToken);
		}

		/// <summary>
		/// Retrieves the refresh token associated with the specified user.
		/// </summary>
		/// <param name="userId">The unique identifier of the user whose refresh token is to be retrieved. Must be a positive integer.</param>
		/// <returns>A <see cref="RefreshTokenModel"/> containing the refresh token information for the user, or <c>null</c> if no
		/// refresh token exists for the specified user.</returns>
		public RefreshTokenEntity? GetRefreshTokenForUser(int userId)
		{
			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();
			return  refreshTokensTable.GetByCondition(rToken => rToken.UserId, userId).FirstOrDefault();
		}
	}
}
