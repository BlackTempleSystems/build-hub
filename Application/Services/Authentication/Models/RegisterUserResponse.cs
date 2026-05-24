using BuildHub.Application.Services.Authentication.Jwt;
using BuildHub.Application.Services.Authentication.Jwt.Models;
using BuildHub.Domain.Autehntication.Users.Models;
using System.Text.Json.Serialization;

namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents the response returned after a user registration operation.
	/// </summary>
	public sealed class RegisterUserResponse
	{
		/// <summary>
		/// 
		/// </summary>
		public UserModel? User { get; set; }

		/// <summary>
		/// 
		/// </summary>

		[JsonIgnore]
		public JwtModel? Jwt { get; set; }

		/// <summary>
		/// Gets or sets the pair of access and refresh tokens associated with the current user session.
		/// </summary>
		[JsonIgnore]
		public TokenPairModel? TokenPair { get; set; }
	}
}
