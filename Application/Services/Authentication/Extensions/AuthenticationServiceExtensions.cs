#region
using BuildHub.Application.Services.Authentication.Jwt;
using BuildHub.Application.Services.Authentication.Jwt.Configuration;
using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Application.Services.Authentication.Validators;
using BuildHub.Common.Configuration;
using BuildHub.Common.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
#endregion

namespace BuildHub.Application.Services.Authentication.Extensions
{
	using CryptographicService;

	/// <summary>
	/// Provides extension methods for registering authentication and authorization services for BuildHub applications.
	/// </summary>
	/// <remarks>This class contains extension methods for configuring JWT-based authentication and related services
	/// in an ASP.NET Core application's dependency injection container. These methods are intended to be called during
	/// application startup to ensure authentication and authorization are properly set up.</remarks>
	public static class AuthenticationServiceExtensions
	{
		/// <summary>
		/// Adds JWT-based authentication and authorization services to the specified service collection using settings from
		/// the provided configuration.
		/// </summary>
		/// <remarks>This method configures JWT bearer authentication using values from the 'Jwt' section of the
		/// configuration. It also registers token-related services and enables authorization. Ensure that the configuration
		/// contains valid 'Issuer', 'Audience', and 'Key' values under the 'Jwt' section.</remarks>
		/// <param name="services">The service collection to which authentication and authorization services will be added.</param>
		/// <param name="configuration">The application configuration containing JWT settings such as issuer, audience, and signing key.</param>
		/// <returns>The same service collection instance, enabling method chaining.</returns>
		public static IServiceCollection AddBuildHubAuthentication(this IServiceCollection services)
		{
			// Auth services
			services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
			services.AddSingleton<IJwtService, JwtService>();
			services.AddSingleton<ICryptographicService, CryptographicService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();

			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			var jwtOptions = configurationManager.GetConfigurationModel<JwtOptions>("Jwt");

			if (jwtOptions is null)
				throw new InvalidOperationException();

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

			services
			  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			  .AddJwtBearer(options =>
			  {
				  options.TokenValidationParameters = new TokenValidationParameters
				  {
					  ValidateIssuer = true,
					  ValidIssuer = jwtOptions.Issuer,

					  ValidateAudience = true,
					  ValidAudience = jwtOptions.Audience,

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
}
