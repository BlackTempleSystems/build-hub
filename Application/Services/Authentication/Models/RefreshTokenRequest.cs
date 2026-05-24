
namespace BuildHub.Application.Services.Authentication.Models
{
	public sealed record class RefreshTokenRequest
	{
		public Guid UserGuid { get; set; }
		public string? RefreshToken { get; set; }
	}
}
