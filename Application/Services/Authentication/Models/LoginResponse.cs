namespace BuildHub.Application.Services.Authentication.Models
{
	using BuildHub.Application.Services.Authentication.Jwt.Models;
	using BuildHub.Domain.Autehntication.Users.Models;
	using Jwt;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Represents the result of a login operation.
	/// </summary>
	public sealed record class LoginResponse
	{
		/// <summary>
		/// Gets or sets the user associated with the current context.
		/// </summary>
		public UserModel? User { get; set; }
		
		/// <summary>
		/// Gets or sets the pair of access and refresh tokens associated with the current user session.
		/// </summary>
		[JsonIgnore]
		public TokenPairModel? TokenPair { get; set; }
}
}
