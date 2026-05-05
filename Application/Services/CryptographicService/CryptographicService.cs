using BuildHub.Application.Services.CryptographicService.Configuration;
using BuildHub.Common.Configuration;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace BuildHub.Application.Services.CryptographicService
{
	/// <summary>
	/// Provides cryptographic operations for secure password hashing and verification using the Argon2id algorithm.
	/// </summary>
	/// <remarks>
	/// This service implements the Argon2id password hashing algorithm, which is the current OWASP recommended
	/// approach for password storage. All settings are configurable via <see cref="CryptographicSettings"/> and
	/// validated at startup to prevent insecure configurations reaching production.
	/// </remarks>
	public sealed class CryptographicService : ICryptographicService
	{
		private readonly CryptographicSettings _cryptographicSettings;

		/// <summary>
		/// Initializes a new instance of <see cref="CryptographicService"/> with the provided settings.
		/// </summary>
		/// <param name="settings">The cryptographic configuration settings.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when any setting value does not meet the minimum security requirements.</exception>
		public CryptographicService()
		{
			_cryptographicSettings = ConfigurationManager.GetConfigurationManager().
				GetConfigurationModel<CryptographicSettings>("CryptographicSettings")!;

			ValidateSettings();
		}

		/// <inheritdoc/>
		public byte[] GenerateSalt()
			=> RandomNumberGenerator.GetBytes(_cryptographicSettings!.SaltSize);

		/// <inheritdoc/>
		/// <exception cref="ArgumentException">Thrown when <paramref name="password"/> is null or empty.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="salt"/> is null.</exception>
		public string HashPassword(string password, byte[] salt)
		{
			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Password cannot be null, empty, or whitespace.", nameof(password));

			ArgumentNullException.ThrowIfNull(salt);
			if (salt.Length != _cryptographicSettings.SaltSize)
				throw new ArgumentException("Invalid salt size");

			byte[] hash = ComputeHash(password, salt);

			byte[] combined = new byte[_cryptographicSettings!.SaltSize + _cryptographicSettings.HashSize];
			Buffer.BlockCopy(salt, 0, combined, 0, _cryptographicSettings.SaltSize);
			Buffer.BlockCopy(hash, 0, combined, _cryptographicSettings.SaltSize, _cryptographicSettings.HashSize);

			return Convert.ToBase64String(combined);
		}

		/// <inheritdoc/>
		/// <exception cref="ArgumentException">Thrown when <paramref name="password"/> is null or empty.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="storedHash"/> is null or empty.</exception>
		public bool VerifyPassword(string password, string storedHash)
		{
			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Password cannot be null, empty, or whitespace.", nameof(password));

			if (string.IsNullOrWhiteSpace(storedHash))
				throw new ArgumentException("Stored hash cannot be null, empty, or whitespace.", nameof(storedHash));

			if (!TryDecodeHash(storedHash, out byte[] combined))
				return false;

			if (!IsValidLength(combined))
				return false;

			byte[] salt = ExtractSalt(combined);
			byte[] expectedHash = ExtractHash(combined);
			byte[] actualHash = ComputeHash(password, salt);

			return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
		}

		/// <summary>
		/// Computes an Argon2id hash for the given password and salt.
		/// </summary>
		private byte[] ComputeHash(string password, byte[] salt)
		{
			using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
			{
				Salt = salt,
				DegreeOfParallelism = _cryptographicSettings!.DegreeOfParallelism,
				MemorySize = _cryptographicSettings.MemorySize,
				Iterations = _cryptographicSettings.Iterations
			};

			return argon2.GetBytes(_cryptographicSettings.HashSize);
		}

		/// <summary>
		/// Attempts to Base64-decode a stored hash string.
		/// </summary>
		private static bool TryDecodeHash(string storedHash, out byte[] combined)
		{
			try
			{
				combined = Convert.FromBase64String(storedHash);
				return true;
			}
			catch (FormatException)
			{
				combined = [];
				return false;
			}
		}

		/// <summary>
		/// Validates that the combined byte array length matches the expected salt + hash size.
		/// </summary>
		private bool IsValidLength(byte[] combined)
			=> combined.Length == _cryptographicSettings!.SaltSize + _cryptographicSettings.HashSize;

		/// <summary>
		/// Extracts the salt from the combined salt+hash byte array.
		/// </summary>
		private byte[] ExtractSalt(byte[] combined)
		{
			byte[] salt = new byte[_cryptographicSettings!.SaltSize];
			Buffer.BlockCopy(combined, 0, salt, 0, _cryptographicSettings.SaltSize);
			return salt;
		}

		/// <summary>
		/// Extracts the hash from the combined salt+hash byte array.
		/// </summary>
		private byte[] ExtractHash(byte[] combined)
		{
			byte[] hash = new byte[_cryptographicSettings!.HashSize];
			Buffer.BlockCopy(combined, _cryptographicSettings.SaltSize, hash, 0, _cryptographicSettings.HashSize);
			return hash;
		}

		/// <summary>
		/// Validates all cryptographic settings meet minimum security requirements.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when any setting is below the minimum acceptable value.</exception>
		private void ValidateSettings()
		{
			if (_cryptographicSettings!.SaltSize < 16)
				throw new ArgumentException("SaltSize must be at least 16 bytes.", nameof(_cryptographicSettings.SaltSize));

			if (_cryptographicSettings.HashSize < 32)
				throw new ArgumentException("HashSize must be at least 32 bytes.", nameof(_cryptographicSettings.HashSize));

			if (_cryptographicSettings.Iterations < 3)
				throw new ArgumentException("Iterations must be at least 3 for Argon2id.", nameof(_cryptographicSettings.Iterations));

			if (_cryptographicSettings.MemorySize < 65_536)
				throw new ArgumentException("MemorySize must be at least 65536 KB (64 MB).", nameof(_cryptographicSettings.MemorySize));

			if (_cryptographicSettings.DegreeOfParallelism < 1)
				throw new ArgumentException("DegreeOfParallelism must be at least 1.", nameof(_cryptographicSettings.DegreeOfParallelism));
		}
	}
}