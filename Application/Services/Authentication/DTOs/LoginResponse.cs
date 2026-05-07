namespace BuildHub.Application.Services.Authentication.Models
{
	/// <summary>
	/// Represents the result of a login operation.
	/// </summary>
	public sealed record class LoginResponse
	{
		public string AccessToken { get; set; } = string.Empty;

		public LoginResponse()
		{
		}
	}
}
