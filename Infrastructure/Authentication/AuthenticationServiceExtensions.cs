using BuildHub.Domain.Store;
using BuildHub.Infrastructure.Auth.Services;
using BuildHub.Infrastructure.Auth.Store;
using BuildHub.Infrastructure.Auth.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BuildHub.Infrastructure.Authentication;

/// <summary>
/// Provides extension methods for registering authentication and authorization services for BuildHub applications.
/// </summary>
/// <remarks>This class contains extension methods for configuring JWT-based authentication and related services
/// in an ASP.NET Core application's dependency injection container. These methods are intended to be called during
/// application startup to ensure authentication and authorization are properly set up.</remarks>
public static class AuthenticationServiceExtensions
{
	public static IServiceCollection AddBuildHubAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		// Auth services
		//services.AddSingleton<IUserValidator, DemoUserValidator>();              // replace later
		services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();  // swap to DB later
		services.AddSingleton<TokenService>();

		// JWT validation
		var jwtSection = configuration.GetSection("Jwt");
		var issuer = jwtSection["Issuer"]!;
		var audience = jwtSection["Audience"]!;
		var key = jwtSection["Key"]!;

		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

		services
		  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		  .AddJwtBearer(options =>
		  {
			  options.TokenValidationParameters = new TokenValidationParameters
			  {
				  ValidateIssuer = true,
				  ValidIssuer = issuer,

				  ValidateAudience = true,
				  ValidAudience = audience,

				  ValidateIssuerSigningKey = true,
				  IssuerSigningKey = signingKey,

				  ValidateLifetime = true,
				  ClockSkew = TimeSpan.FromSeconds(30)
			  };
		  });

		services.AddAuthorization();

		return services;
	}
}
