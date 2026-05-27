using BuildHub.Application.Services.Authentication.Jwt.Models;
using BuildHub.Domain.Autehntication.Users.Entities;

namespace BuildHub.Application.Services.Authentication.Jwt
{
	/// <summary>
	/// Defines the contract for services that provide JSON Web Token (JWT) generation, validation, or related operations.
	/// </summary>
	public interface IJwtService
	{
		/// <summary>
		/// Generates a security token for the specified user entity.
		/// </summary>
		/// <param name="user">The user entity for which to generate the security token. Cannot be null.</param>
		/// <returns>A string containing the generated security token for the specified user.</returns>
		public JwtModel GenerateSecurityToken(UserEntity user);
	}
}
