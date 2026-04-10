namespace BuildHub.Domain.Users;

public record UserRecord(
	string UserId,
	string Email,
	string DisplayName,
	string Role
);