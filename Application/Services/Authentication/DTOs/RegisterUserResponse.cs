namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents the response returned after a user registration operation.
	/// </summary>
	public sealed class RegisterUserResponse
	{
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? AccessToken { get; set; }

		public RegisterUserResponse()
		{
		}
	}
}
