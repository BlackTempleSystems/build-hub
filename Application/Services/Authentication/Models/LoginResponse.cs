namespace BuildHub.Application.Services.Authentication.Models
{
	using BuildHub.Domain.Autehntication.Users.Models;
	using Jwt;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Represents the result of a login operation.
	/// </summary>
	public sealed record class LoginResponse
	{
		/// <summary>
		/// 
		/// </summary>
		/// 
		public UserModel? User { get; set; }
		/// <summary>
		/// Gets or sets the JSON Web Token (JWT) used for authenticating API requests.
		/// </summary>

		[JsonIgnore]
		public JwtModel? Jwt { get; set; }
	}
}
