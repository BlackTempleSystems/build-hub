namespace BuildHub.Domain.Store;

public interface IRefreshTokenStore
{
	/// <summary>
	/// Stores the refresh token record asynchronously.
	/// </summary>
	Task StoreAsync(RefreshTokenRecord record, CancellationToken ct);

	/// <summary>
	/// Rotate: consume old token (one-time use) and replace with new record.
	/// Returns the old record if successful; null otherwise.
	/// </summary>
	Task<RefreshTokenRecord?> TryConsumeForRotationAsync(string presentedRefreshToken, CancellationToken ct);

	/// <summary>
	/// Revokes the refresh token asynchronously.
	/// </summary>
	Task RevokeAsync(string presentedRefreshToken, CancellationToken ct);

	/// <summary>
	/// Cleans up expired refresh tokens asynchronously.
	/// </summary>
	Task CleanupExpiredAsync(CancellationToken ct);
}
