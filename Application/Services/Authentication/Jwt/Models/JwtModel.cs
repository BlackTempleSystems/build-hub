namespace BuildHub.Application.Services.Authentication.Jwt
{
	/// <summary>
	/// Represents a JSON Web Token (JWT) and its associated metadata.
	/// </summary>
	/// <remarks>This model is typically used to encapsulate the access token and its expiration information when
	/// handling authentication or authorization workflows. Instances of this record are immutable except for property
	/// setters, and are intended for use in scenarios where JWTs are issued, validated, or stored.</remarks>
	public sealed record class JwtModel
	{
		/// <summary>
		/// Gets or sets the access token used for authenticating requests.
		/// </summary>
		public string AccessToken { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the date and time when the item expires.
		/// </summary>
		public DateTime ExpirationDate { get; set; }
	}
}
