using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Application.Services.Authentication.Validators.Base;
using BuildHub.Common.Validators;
namespace BuildHub.Application.Services.Authentication.Validators
{
	/// <summary>
	/// Register user request validator.
	/// </summary>
	public class RegisterRequestValidator : BaseAuthenticationValidator,  IValidator<RegisterRequest>
	{
		
		public ValidationResult Validate(RegisterRequest registerRequest)
		{
			ValidationResult validationResult = new ValidationResult();

			// First name
			if (string.IsNullOrWhiteSpace(registerRequest.FirstName))
				validationResult.AddError("FirstName", "First name is required.");
			else if (registerRequest.FirstName.Length > 128)
				validationResult.AddError("FirstName", "First name cannot exceed 128 characters.");

			// Last name
			if (string.IsNullOrWhiteSpace(registerRequest.LastName))
				validationResult.AddError("LastName", "Last name is required.");
			else if (registerRequest.LastName.Length > 128)
				validationResult.AddError("LastName", "Last name cannot exceed 128 characters.");

			// Email
			validationResult += ValidateEmail(registerRequest.Email);

			// Username
			if (string.IsNullOrWhiteSpace(registerRequest.UserName))
				validationResult.AddError("UserName", "Username is required.");
			else if (registerRequest.UserName.Length < 3)
				validationResult.AddError("UserName", "Username must be at least 3 characters.");
			else if (registerRequest.UserName.Length > 32)
				validationResult.AddError("UserName", "Username cannot exceed 32 characters.");

			// Password
			validationResult += ValidatePassword(registerRequest.Password);

			// Confirmed password
			if (string.IsNullOrWhiteSpace(registerRequest.ConfirmedPassword))
				validationResult.AddError("ConfirmedPassword", "Password confirmation is required.");
			else if (registerRequest.Password != registerRequest.ConfirmedPassword)
				validationResult.AddError("ConfirmedPassword", "Passwords do not match.");

			return validationResult;
		}
	}
}
