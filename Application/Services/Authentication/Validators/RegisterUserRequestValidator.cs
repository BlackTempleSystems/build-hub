using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Common.Validators;
using System.Text.RegularExpressions;
namespace BuildHub.Application.Services.Authentication.Validators
{
	/// <summary>
	/// Register user request validator.
	/// </summary>
	public class RegisterUserRequestValidator : IValidator<RegisterUserRequest>
	{
		/// <summary>
		/// Email regex for validation.
		/// </summary>
		private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public ValidationResult Validate(RegisterUserRequest value)
		{
			ValidationResult validationResult = new ValidationResult();

			// First name
			if (string.IsNullOrWhiteSpace(value.FirstName))
				validationResult.AddError("FirstName", "First name is required.");
			else if (value.FirstName.Length > 128)
				validationResult.AddError("FirstName", "First name cannot exceed 128 characters.");

			// Last name
			if (string.IsNullOrWhiteSpace(value.LastName))
				validationResult.AddError("LastName", "Last name is required.");
			else if (value.LastName.Length > 128)
				validationResult.AddError("LastName", "Last name cannot exceed 128 characters.");

			// Email
			if (string.IsNullOrWhiteSpace(value.Email))
				validationResult.AddError("Email", "Email is required.");
			else if (!EmailRegex.IsMatch(value.Email))
				validationResult.AddError("Email", "Email is not valid.");
			else if (value.Email.Length > 254)
				validationResult.AddError("Email", "Email cannot exceed 254 characters.");

			// Username
			if (string.IsNullOrWhiteSpace(value.UserName))
				validationResult.AddError("UserName", "Username is required.");
			else if (value.UserName.Length < 3)
				validationResult.AddError("UserName", "Username must be at least 3 characters.");
			else if (value.UserName.Length > 32)
				validationResult.AddError("UserName", "Username cannot exceed 32 characters.");

			// Password
			if (string.IsNullOrWhiteSpace(value.Password))
				validationResult.AddError("Password", "Password is required.");
			else if (value.Password.Length < 8)
				validationResult.AddError("Password", "Password must be at least 8 characters.");
			else if (value.Password.Length > 128)
				validationResult.AddError("Password", "Password cannot exceed 128 characters.");

			// Confirmed password
			if (string.IsNullOrWhiteSpace(value.ConfirmedPassword))
				validationResult.AddError("ConfirmedPassword", "Password confirmation is required.");
			else if (value.Password != value.ConfirmedPassword)
				validationResult.AddError("ConfirmedPassword", "Passwords do not match.");

			return validationResult;
		}
	}
}
