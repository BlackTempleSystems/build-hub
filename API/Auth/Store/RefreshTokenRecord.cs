namespace BuildHub.API.Auth.Store;

public sealed record RefreshTokenRecord(
string TokenHash,
string UserId,
DateTimeOffset ExpiresAt,
DateTimeOffset CreatedAt,
DateTimeOffset? RevokedAt,
string? ReplacedByTokenHash
);
