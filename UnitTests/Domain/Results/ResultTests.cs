using BuildHub.Domain.Results;

namespace UnitTests.Domain.Results
{
	[TestClass]
	public sealed class ResultTests
	{
		[TestMethod]
		public void Success_Should_Create_Ok_Result_With_Data()
		{
			var data = new TestData("BuildHub");

			var result = Result<TestData>.Success(data);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreSame(data, result.Data);
			Assert.AreEqual(ResultStatus.Ok, result.Status);
			Assert.IsNull(result.ErrorMessage);
		}

		[TestMethod]
		public void Failure_Should_Create_Failed_Result_With_Error_And_Status()
		{
			var result = Result<string>.Failure("User does not exist", ResultStatus.NotFound);

			Assert.IsFalse(result.IsSuccess);
			Assert.IsNull(result.Data);
			Assert.AreEqual("User does not exist", result.ErrorMessage);
			Assert.AreEqual(ResultStatus.NotFound, result.Status);
		}

		[TestMethod]
		public void Failure_Should_Format_Error_Message_When_Arguments_Are_Provided()
		{
			var result = Result<string>.Failure("User '{0}' already exists", ResultStatus.Conflict, "adam");

			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual("User 'adam' already exists", result.ErrorMessage);
			Assert.AreEqual(ResultStatus.Conflict, result.Status);
		}

		private sealed record TestData(string Name);
	}
}
