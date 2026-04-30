namespace BuildHub.API.Auth.Models;

public record TokenPairResponse(
  string AccessToken,
  int ExpiresInSeconds,
  string RefreshToken
);
