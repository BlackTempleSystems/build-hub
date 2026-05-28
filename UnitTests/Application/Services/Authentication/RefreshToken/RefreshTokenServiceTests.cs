using BuildHub.Application.Services.Authentication.RefreshToken;
using System.Security.Cryptography;
using System.Text;

namespace UnitTests.Application.Services.Authentication.RefreshToken
{
	[TestClass]
	public sealed class RefreshTokenServiceTests
	{
		[TestMethod]
		public void VerifyRefreshToken_Should_Return_True_For_Matching_Raw_And_Hashed_Token()
		{
			var service = new RefreshTokenService();
			const string rawToken = "refresh-token-value";
			string hashedToken = HashToken(rawToken);

			Assert.IsTrue(service.VerifyRefreshToken(rawToken, hashedToken));
		}

		[TestMethod]
		public void VerifyRefreshToken_Should_Return_False_For_Non_Matching_Token()
		{
			var service = new RefreshTokenService();
			string hashedToken = HashToken("refresh-token-value");

			Assert.IsFalse(service.VerifyRefreshToken("different-token-value", hashedToken));
		}

		[TestMethod]
		[DataRow("", "hash")]
		[DataRow(" ", "hash")]
		[DataRow("token", "")]
		[DataRow("token", " ")]
		public void VerifyRefreshToken_Should_Return_False_For_Missing_Token_Data(string rawToken, string hashedToken)
		{
			var service = new RefreshTokenService();

			Assert.IsFalse(service.VerifyRefreshToken(rawToken, hashedToken));
		}

		private static string HashToken(string rawToken)
		{
			byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
			return Convert.ToBase64String(bytes);
		}
	}
}
