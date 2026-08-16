using Microsoft.EntityFrameworkCore;

namespace BuildHub.Infrastructure.Databases;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options)
	: DbContext(options);