namespace UnitTests.Application.Services.CryptographicService
{
	using BuildHub.Application.Services.CryptographicService;

	[TestClass]
	[DoNotParallelize]
	public class CryptographicServiceTests
	{
		private const int _SaltSize = 16;
		private const int _HashSize = 32;

		[TestMethod]
		public void GenerateSalt_Should_Return_Configured_Salt_Size()
		{
			var service = new CryptographicService();

			byte[] salt = service.GenerateSalt();

			Assert.AreEqual(_SaltSize, salt.Length);
		}

		[TestMethod]
		public void GenerateSalt_Should_Return_Different_Values_On_Subsequent_Calls()
		{
			var service = new CryptographicService();

			byte[] firstSalt = service.GenerateSalt();
			byte[] secondSalt = service.GenerateSalt();

			CollectionAssert.AreNotEqual(firstSalt, secondSalt);
		}

		[TestMethod]
		public void HashPassword_Should_Return_Base64_Encoded_Salt_And_Hash()
		{
			var service = new CryptographicService();
			byte[] salt = service.GenerateSalt();

			string storedHash = service.HashPassword("ValidPassword1!", salt);
			byte[] decodedHash = Convert.FromBase64String(storedHash);

			Assert.AreEqual(_SaltSize + _HashSize, decodedHash.Length);
		}

		[TestMethod]
		public void VerifyPassword_Should_Return_True_For_Matching_Password()
		{
			var service = new CryptographicService();
			byte[] salt = service.GenerateSalt();
			string storedHash = service.HashPassword("ValidPassword1!", salt);

			Assert.IsTrue(service.VerifyPassword("ValidPassword1!", storedHash));
		}

		[TestMethod]
		public void VerifyPassword_Should_Return_False_For_Wrong_Password()
		{
			var service = new CryptographicService();
			byte[] salt = service.GenerateSalt();
			string storedHash = service.HashPassword("ValidPassword1!", salt);

			Assert.IsFalse(service.VerifyPassword("WrongPassword1!", storedHash));
		}

		[TestMethod]
		[DataRow("")]
		[DataRow(" ")]
		public void HashPassword_Should_Reject_Empty_Or_Whitespace_Password(string password)
		{
			var service = new CryptographicService();

			Assert.Throws<ArgumentException>(() => service.HashPassword(password, service.GenerateSalt()));
		}

		[TestMethod]
		public void HashPassword_Should_Reject_Invalid_Salt()
		{
			var service = new CryptographicService();

			Assert.Throws<ArgumentNullException>(() => service.HashPassword("ValidPassword1!", null!));
			Assert.Throws<ArgumentException>(() => service.HashPassword("ValidPassword1!", new byte[1]));
		}

		[TestMethod]
		public void VerifyPassword_Should_Return_False_For_Invalid_Stored_Hash()
		{
			var service = new CryptographicService();

			Assert.IsFalse(service.VerifyPassword("ValidPassword1!", "not-base64"));
			Assert.IsFalse(service.VerifyPassword("ValidPassword1!", Convert.ToBase64String(new byte[4])));
		}
	}
}
