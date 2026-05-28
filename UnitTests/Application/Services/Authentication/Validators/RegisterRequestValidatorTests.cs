using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Application.Services.Authentication.Validators;

namespace UnitTests.Application.Services.Authentication.Validators
{
	[TestClass]
	public sealed class RegisterRequestValidatorTests
	{
		[TestMethod]
		public void Validate_Should_Pass_For_Valid_Request()
		{
			var validator = new RegisterRequestValidator();

			var result = validator.Validate(CreateValidRequest());

			Assert.IsTrue(result.IsValid);
		}

		[TestMethod]
		[DataRow("")]
		[DataRow("not-an-email")]
		[DataRow("missing-domain@")]
		public void Validate_Should_Fail_For_Invalid_Email(string email)
		{
			var validator = new RegisterRequestValidator();
			var request = CreateValidRequest() with { Email = email };

			var result = validator.Validate(request);

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(RegisterRequest.Email)));
		}

		[TestMethod]
		public void Validate_Should_Fail_When_Email_Exceeds_Max_Length()
		{
			var validator = new RegisterRequestValidator();
			var request = CreateValidRequest() with
			{
				Email = $"{new string('a', RegisterRequestValidator._EmailMaximumLenght)}@example.com"
			};

			var result = validator.Validate(request);

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(RegisterRequest.Email)));
		}

		[TestMethod]
		[DataRow("", nameof(RegisterRequest.UserName))]
		[DataRow("", nameof(RegisterRequest.FirstName))]
		[DataRow("", nameof(RegisterRequest.LastName))]
		public void Validate_Should_Fail_When_Required_Text_Field_Is_Empty(string value, string propertyName)
		{
			var validator = new RegisterRequestValidator();
			var request = propertyName switch
			{
				nameof(RegisterRequest.UserName) => CreateValidRequest() with { UserName = value },
				nameof(RegisterRequest.FirstName) => CreateValidRequest() with { FirstName = value },
				nameof(RegisterRequest.LastName) => CreateValidRequest() with { LastName = value },
				_ => throw new ArgumentOutOfRangeException(nameof(propertyName))
			};

			var result = validator.Validate(request);

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Any(error => error.PropertyName == propertyName));
		}

		[TestMethod]
		[DataRow("Short1!")]
		[DataRow("lowercase1!")]
		[DataRow("UPPERCASE1!")]
		[DataRow("NoNumber!!")]
		[DataRow("NoSpecial11")]
		[DataRow("TooLongPassword1!")]
		public void Validate_Should_Fail_For_Passwords_That_Do_Not_Match_Policy(string password)
		{
			var validator = new RegisterRequestValidator();
			var request = CreateValidRequest() with
			{
				Password = password,
				ConfirmedPassword = password
			};

			var result = validator.Validate(request);

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(RegisterRequest.Password)));
		}

		[TestMethod]
		public void Validate_Should_Fail_When_Confirmed_Password_Does_Not_Match()
		{
			var validator = new RegisterRequestValidator();
			var request = CreateValidRequest() with
			{
				ConfirmedPassword = "Different1!"
			};

			var result = validator.Validate(request);

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(RegisterRequest.ConfirmedPassword)));
		}

		private static RegisterRequest CreateValidRequest()
		{
			return new RegisterRequest
			{
				FirstName = "Adam",
				LastName = "Ayash",
				Email = "adam@example.com",
				UserName = "adam",
				Password = "Password1!",
				ConfirmedPassword = "Password1!"
			};
		}
	}
}
