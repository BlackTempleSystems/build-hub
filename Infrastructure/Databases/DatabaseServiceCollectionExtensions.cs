using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildHub.Infrastructure.Databases;

public static class DatabaseServiceCollectionExtensions
{
	public static IServiceCollection AddBuildHubDatabases(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<CoreDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("Core")));

		services.AddDbContext<UsersDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("Users")));

		services.AddHealthChecks()
			.AddDbContextCheck<CoreDbContext>(
				"database-core",
				failureStatus: HealthStatus.Unhealthy)
			.AddDbContextCheck<UsersDbContext>(
				"database-users",
				failureStatus: HealthStatus.Unhealthy);

		return services;
	}
}
