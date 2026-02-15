namespace BuildHub.API.Auth.Models;

public record UserRecord(
  string UserId,
  string Email,
  string DisplayName,
  string Role
);