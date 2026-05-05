namespace BuildHub.Application.Services.CryptographicService
{
	public interface ICryptographicService
	{
		/// <summary>
		/// Generates a salt.
		/// </summary>
		/// <returns></returns>
		public byte[] GenerateSalt();

		/// <summary>
		/// Hashes the password by given salt.
		/// </summary>
		/// <param name="password"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		public string HashPassword(string password, byte[] salt);

		/// <summary>
		/// Verifies if a given hash matches a password.
		/// </summary>
		/// <param name="password"></param>
		/// <param name="storedHash"></param>
		/// <returns></returns>
		public bool VerifyPassword(string password, string storedHash);
	}
}
