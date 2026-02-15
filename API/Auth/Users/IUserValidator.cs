using BuildHub.API.Auth.Models;

namespace BuildHub.API.Auth.Users;

public interface IUserValidator
{
	Task<UserRecord?> ValidateAsync(string usernameOrEmail, string password, CancellationToken ct);
}
