using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public sealed class BuildCommandResultTests
	{
		[TestMethod]
		public void CreateSuccess_Should_Set_Success_State_And_Preserve_Steps()
		{
			var step = new ExecutionStep
			{
				Id = Guid.NewGuid(),
				Order = 1,
				Name = "Build",
				StepType = "MSBuild",
				Commands = new[]
				{
					new ExecutionCommand
					{
						Executable = "MSBuild",
						Arguments = "\"BuildHub.sln\""
					}
				}
			};

			var result = BuildCommandResult.CreateSuccess("MSBuild", TimeSpan.FromMilliseconds(25), new[] { step });

			Assert.IsTrue(result.Success);
			Assert.AreEqual("MSBuild", result.BuilderName);
			Assert.AreEqual(string.Empty, result.ErrorMessage);
			Assert.AreEqual(TimeSpan.FromMilliseconds(25), result.BuildTime);
			Assert.HasCount(1, result.Steps);
			Assert.AreSame(step, result.Steps[0]);
		}

		[TestMethod]
		public void CreateFailure_Should_Set_Error_State_And_Empty_Steps()
		{
			var result = BuildCommandResult.CreateFailure("Git", TimeSpan.FromSeconds(1), "Repository missing");

			Assert.IsFalse(result.Success);
			Assert.AreEqual("Git", result.BuilderName);
			Assert.AreEqual("Repository missing", result.ErrorMessage);
			Assert.AreEqual(TimeSpan.FromSeconds(1), result.BuildTime);
			Assert.IsEmpty(result.Steps);
		}

		[TestMethod]
		public void ToString_ForSuccess_Should_Include_Step_And_Command_Details()
		{
			var step = new ExecutionStep
			{
				Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
				Order = 2,
				Name = "Fetch",
				StepType = "Git",
				Commands = new[]
				{
					new ExecutionCommand
					{
						Executable = "git",
						Arguments = "pull",
						WorkingDirectory = @"C:\src"
					}
				}
			};

			var result = BuildCommandResult.CreateSuccess("Git", TimeSpan.FromMilliseconds(10), new[] { step });

			string text = result.ToString();

			StringAssert.Contains(text, "Success: True");
			StringAssert.Contains(text, "BuilderName: \"Git\"");
			StringAssert.Contains(text, "Order: 2 Name: Fetch");
			StringAssert.Contains(text, "Executable: git Arguments: pull Working dir: C:\\src");
			StringAssert.Contains(text, "BuildTime:");
		}

		[TestMethod]
		public void ToString_ForFailure_Should_Include_Error_And_No_Steps()
		{
			var result = BuildCommandResult.CreateFailure("Git", TimeSpan.Zero, "Repository missing");

			string text = result.ToString();

			StringAssert.Contains(text, "Success: False");
			StringAssert.Contains(text, "BuilderName: \"Git\"");
			StringAssert.Contains(text, "\t<none>");
			StringAssert.Contains(text, "ErrorMessage: \"Repository missing\"");
		}
	}
}
