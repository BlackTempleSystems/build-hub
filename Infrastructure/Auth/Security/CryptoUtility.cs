using System.Security.Cryptography;
using System.Text;

namespace BuildHub.Infrastructure.Auth.Security;

public static class CryptoUtility
{
	public static string GenerateSecureToken(int numBytes = 32)
	{
		var bytes = RandomNumberGenerator.GetBytes(numBytes);
		return Base64UrlEncode(bytes);
	}

	public static string Sha256Base64Url(string input)
	{
		var bytes = Encoding.UTF8.GetBytes(input);
		var hash = SHA256.HashData(bytes);
		return Base64UrlEncode(hash);
	}

	private static string Base64UrlEncode(byte[] bytes)
	{
		// Base64Url without padding
		return Convert.ToBase64String(bytes)
		  .TrimEnd('=')
		  .Replace('+', '-')
		  .Replace('/', '_');
	}
}
