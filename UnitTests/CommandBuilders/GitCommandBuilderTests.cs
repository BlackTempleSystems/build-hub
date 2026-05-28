using BuildHub.CommandBuilder.CommandBuilders;
using BuildHub.CommandBuilder.Models.Contexts;

namespace TestProject.CommandBuilders
{
	[TestClass]
	public sealed class GitCommandBuilderTests
	{
		[TestMethod]
		public void GenerateCommand_WithBranch_Should_Create_Clone_Checkout_And_Pull_Commands()
		{
			var result = new GitCommandBuilder()
				.SetRepository("https://example.com/build-hub.git")
				.SetTargetDirectory(@"C:\src\build-hub")
				.SetBranch("develop")
				.GenerateCommand();

			Assert.IsTrue(result.Success);
			Assert.AreEqual("Git", result.BuilderName);
			Assert.HasCount(1, result.Steps);

			var commands = result.Steps.Single().Commands.Select(command => command.Arguments).ToArray();

			Assert.HasCount(4, commands);
			Assert.AreEqual(@"if not exist ""C:\src\build-hub"" mkdir ""C:\src\build-hub""", commands[0]);
			Assert.AreEqual(@"if not exist ""C:\src\build-hub\.git"" git clone ""https://example.com/build-hub.git"" ""C:\src\build-hub"" else git -C ""C:\src\build-hub"" fetch", commands[1]);
			Assert.AreEqual(@"git -C ""C:\src\build-hub"" checkout develop", commands[2]);
			Assert.AreEqual(@"git -C ""C:\src\build-hub"" pull", commands[3]);
		}

		[TestMethod]
		public void GenerateCommand_WithVersion_Should_Checkout_Version_Instead_Of_Branch()
		{
			var result = new GitCommandBuilder(new GitContext
			{
				Repository = "https://example.com/build-hub.git",
				TargetDirectory = @"C:\src\build-hub",
				Branch = "develop",
				Version = "v1.2.3"
			}).GenerateCommand();

			var commands = result.Steps.Single().Commands.Select(command => command.Arguments).ToArray();

			Assert.AreEqual(@"git -C ""C:\src\build-hub"" checkout v1.2.3", commands[2]);
			CollectionAssert.DoesNotContain(commands, @"git -C ""C:\src\build-hub"" checkout develop");
		}

		[TestMethod]
		public void GenerateCommand_WithClean_Should_Add_Cleanup_Command_First()
		{
			var result = new GitCommandBuilder()
				.SetRepository("https://example.com/build-hub.git")
				.SetTargetDirectory(@"C:\src\build-hub")
				.SetClean(true)
				.GenerateCommand();

			var commands = result.Steps.Single().Commands.Select(command => command.Arguments).ToArray();

			Assert.HasCount(5, commands);
			Assert.AreEqual(@"rmdir /s /q ""C:\src\build-hub""", commands[0]);
		}

		[TestMethod]
		public void Setters_Should_Return_Same_Builder_Instance()
		{
			var builder = new GitCommandBuilder();

			var returned = builder
				.SetRepository("https://example.com/build-hub.git")
				.SetTargetDirectory(@"C:\src\build-hub")
				.SetBranch("main")
				.SetVersion("v1.0.0")
				.SetClean(true);

			Assert.AreSame(builder, returned);
		}

		[TestMethod]
		public void GenerateCommand_WhenRepositoryMissing_Should_Throw()
		{
			var builder = new GitCommandBuilder()
				.SetTargetDirectory(@"C:\src\build-hub");

			var exception = Assert.Throws<InvalidOperationException>(() => builder.GenerateCommand());
			StringAssert.Contains(exception.Message, "Repository URL cannot be empty.");
		}

		[TestMethod]
		public void GenerateCommand_WhenTargetDirectoryMissing_Should_Throw()
		{
			var builder = new GitCommandBuilder()
				.SetRepository("https://example.com/build-hub.git");

			var exception = Assert.Throws<InvalidOperationException>(() => builder.GenerateCommand());
			StringAssert.Contains(exception.Message, "Target directory cannot be empty.");
		}
	}
}
