namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Extensions;

	[TestClass]
	public sealed class DatabaseSourceExtensionsTests
	{
		[TestMethod]
		[DataRow(DatabaseSource.Core)]
		[DataRow(DatabaseSource.Users)]
		public void IsRequired_Should_Return_True_For_Required_Database_Sources(DatabaseSource databaseSource)
		{
			Assert.IsTrue(databaseSource.IsRequired());
		}

		[TestMethod]
		public void IsRequired_Should_Return_False_For_Optional_Database_Source()
		{
			Assert.IsFalse(DatabaseSource.IntegrationTests.IsRequired());
		}

		[TestMethod]
		public void IsRequired_Should_Return_False_For_Invalid_Database_Source()
		{
			Assert.IsFalse(((DatabaseSource)999).IsRequired());
		}
	}
}
