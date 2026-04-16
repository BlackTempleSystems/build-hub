namespace BuildHub.API.Auth.Models;

public record LoginRequest(string UsernameOrEmail, string Password);
