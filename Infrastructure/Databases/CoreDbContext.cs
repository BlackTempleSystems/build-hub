using Microsoft.EntityFrameworkCore;

namespace BuildHub.Infrastructure.Databases;

public sealed class CoreDbContext(DbContextOptions<CoreDbContext> options)
	: DbContext(options);
