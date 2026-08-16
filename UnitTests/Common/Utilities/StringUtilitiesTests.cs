using BuildHub.Common.Utilities;

namespace UnitTests.Common.Utilities
{
	[TestClass]
	public class StringUtilitiesTests
	{
		[TestMethod]
		[DataRow("Build")]
		[DataRow("User Password")]
		[DataRow("Build -  Hub")]
		public void Stringify_And_Match(string text)
		{
			Assert.AreEqual<string>($"'{text}'", StringUtilities.Stringify(text));
		}

		[TestMethod]
		[DataRow("Build")]
		[DataRow("User Password")]
		[DataRow("Build -  Hub")]
		public void Add_Quotes_Should_Be_Equal(string text)
		{
			Assert.AreEqual<string>($@"""{text}""", StringUtilities.AddQuotes(text));
		}
	}

}
