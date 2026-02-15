using System.Collections.Concurrent;

namespace BuildHub.API.Auth.Store;

public sealed class InMemoryRefreshTokenStore : IRefreshTokenStore
{
	private readonly ConcurrentDictionary<string, RefreshTokenRecord> _byHash = new();

	public Task StoreAsync(RefreshTokenRecord record, CancellationToken ct)
	{
		_byHash[record.TokenHash] = record;
		return Task.CompletedTask;
	}

	public Task<RefreshTokenRecord?> TryConsumeForRotationAsync(string presentedRefreshToken, CancellationToken ct)
	{
		var hash = Crypto.Sha256Base64Url(presentedRefreshToken);

		if (!_byHash.TryGetValue(hash, out var existing))
			return Task.FromResult<RefreshTokenRecord?>(null);

		// invalid if expired or already revoked
		if (existing.ExpiresAt <= DateTimeOffset.UtcNow)
			return Task.FromResult<RefreshTokenRecord?>(null);

		if (existing.RevokedAt is not null)
			return Task.FromResult<RefreshTokenRecord?>(null);

		// Mark as revoked now (consumed)
		var consumed = existing with { RevokedAt = DateTimeOffset.UtcNow };
		_byHash[hash] = consumed;

		return Task.FromResult<RefreshTokenRecord?>(consumed);
	}

	public Task RevokeAsync(string presentedRefreshToken, CancellationToken ct)
	{
		var hash = Crypto.Sha256Base64Url(presentedRefreshToken);

		if (_byHash.TryGetValue(hash, out var existing) && existing.RevokedAt is null)
		{
			_byHash[hash] = existing with { RevokedAt = DateTimeOffset.UtcNow };
		}

		return Task.CompletedTask;
	}

	public Task CleanupExpiredAsync(CancellationToken ct)
	{
		var now = DateTimeOffset.UtcNow;
		foreach (var kvp in _byHash)
		{
			if (kvp.Value.ExpiresAt <= now)
				_byHash.TryRemove(kvp.Key, out _);
		}
		return Task.CompletedTask;
	}
}
