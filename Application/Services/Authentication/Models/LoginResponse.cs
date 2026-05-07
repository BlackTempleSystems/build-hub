namespace BuildHub.Application.Services.Authentication.Models
{
	using Jwt;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Represents the result of a login operation.
	/// </summary>
	public sealed record class LoginResponse
	{
		/// <summary>
		/// Gets or sets the user name associated with the account.
		/// </summary>
		public string UserName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the JSON Web Token (JWT) used for authenticating API requests.
		/// </summary>

		[JsonIgnore]
		public JwtModel? Jwt { get; set; }
	}
}
