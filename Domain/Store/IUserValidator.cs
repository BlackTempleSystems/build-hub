using BuildHub.Domain.Users;
namespace BuildHub.Domain.Store;

public interface IUserValidator
{
	Task<UserRecord?> ValidateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken);
}
