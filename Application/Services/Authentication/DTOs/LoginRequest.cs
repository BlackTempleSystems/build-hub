namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents a request to initiate a user login operation.
	/// </summary>
	public sealed record class LoginRequest
	{
		/// <summary>
		/// Gets or sets the username associated with the current user or account.
		/// </summary>
		public string Username { get; init; } = string.Empty;
		/// <summary>
		/// Gets or sets the password associated with the user.
		/// </summary>
		public string UserPassword { get; init; } = string.Empty;

		public LoginRequest()
		{
		}
	}
}
