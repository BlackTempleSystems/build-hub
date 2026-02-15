namespace BuildHub.API.Auth.Store;

public interface IRefreshTokenStore
{
	Task StoreAsync(RefreshTokenRecord record, CancellationToken ct);

	/// <summary>
	/// Rotate: consume old token (one-time use) and replace with new record.
	/// Returns the old record if successful; null otherwise.
	/// </summary>
	Task<RefreshTokenRecord?> TryConsumeForRotationAsync(string presentedRefreshToken, CancellationToken ct);

	Task RevokeAsync(string presentedRefreshToken, CancellationToken ct);

	Task CleanupExpiredAsync(CancellationToken ct);
}
