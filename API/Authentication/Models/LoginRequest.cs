namespace BuildHub.API.Authentication.Models
{
	/// <summary>
	/// Represents a request to initiate a user login operation.
	/// </summary>
	public sealed class LoginRequest
	{
		/// <summary>
		/// Gets or sets the username associated with the current user or account.
		/// </summary>
		public string? Username { get; set; }
		/// <summary>
		/// Gets or sets the password associated with the user.
		/// </summary>
		public string? UserPassword { get; set; }

		public LoginRequest()
		{
		}
	}
}
