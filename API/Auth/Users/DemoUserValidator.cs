using BuildHub.API.Auth.Models;

namespace BuildHub.API.Auth.Users;

public sealed class DemoUserValidator : IUserValidator
{
	// DEMO ONLY: hardcoded credential
	public Task<UserRecord?> ValidateAsync(string usernameOrEmail, string password, CancellationToken ct)
	{
		// Example: username "admin" and password "admin123!"
		if ((usernameOrEmail.Equals("admin", StringComparison.OrdinalIgnoreCase)
			 || usernameOrEmail.Equals("admin@local", StringComparison.OrdinalIgnoreCase))
			&& password == "admin123!")
		{
			return Task.FromResult<UserRecord?>(new UserRecord(
			  UserId: "user-1",
			  Email: "admin@local",
			  DisplayName: "Admin",
			  Role: "Admin"
			));
		}

		return Task.FromResult<UserRecord?>(null);
	}
}
