namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents a request to register a new user with the required personal and account information.
	/// </summary>
	/// <remarks>This record is typically used as a data transfer object when submitting user registration data to
	/// an API or service. All properties should be populated with valid values before sending the request.</remarks>
	public sealed record class RegisterRequest
	{
		/// <summary>
		/// Gets or sets the first name of the person.
		/// </summary>
		public string FirstName { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the last name of the person.
		/// </summary>
		public string LastName { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		public string Email { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the user name associated with the current instance.
		/// </summary>
		public string UserName { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the password used for authentication.
		/// </summary>
		public string Password { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the confirmed password entered by the user.
		/// </summary>
		public string ConfirmedPassword { get; init; } = string.Empty;

		public RegisterRequest()
		{
		}
	}
}
