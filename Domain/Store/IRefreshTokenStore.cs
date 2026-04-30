namespace BuildHub.Domain.Store;

/// <summary>
/// Defines methods for storing, rotating, revoking, and cleaning up refresh tokens in a persistent store.
/// </summary>
/// <remarks>Implementations of this interface are responsible for managing the lifecycle of refresh tokens,
/// including secure storage, one-time use enforcement, and removal of expired tokens. Thread safety and persistence
/// guarantees depend on the specific implementation.</remarks>
public interface IRefreshTokenStore
{
	/// <summary>
	/// Stores the refresh token record asynchronously.
	/// </summary>
	Task StoreAsync(RefreshTokenRecord record, CancellationToken cancellationToken);

	/// <summary>
	/// Rotate: consume old token (one-time use) and replace with new record.
	/// Returns the old record if successful; null otherwise.
	/// </summary>
	Task<RefreshTokenRecord?> TryConsumeForRotationAsync(string presentedRefreshToken, CancellationToken cancellationToken);

	/// <summary>
	/// Revokes the refresh token asynchronously.
	/// </summary>
	Task RevokeAsync(string presentedRefreshToken, CancellationToken cancellationToken);

	/// <summary>
	/// Cleans up expired refresh tokens asynchronously.
	/// </summary>
	Task CleanupExpiredAsync(CancellationToken cancellationToken);
}
