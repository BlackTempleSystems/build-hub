using BuildHub.Common.Validators;
using System.Text.RegularExpressions;

namespace BuildHub.Application.Services.Authentication.Validators.Base
{
	/// <summary>
	/// Provides a base class for implementing authentication validators that perform validation on authentication-related
	/// data, such as email addresses.
	/// </summary>
	/// <remarks>This abstract class is intended to be inherited by custom authentication validator implementations.
	/// It supplies common validation logic that can be extended or overridden in derived classes.</remarks>
	public abstract class BaseAuthenticationValidator
	{
		protected BaseAuthenticationValidator()
		{
		}

		/// <summary>
		/// Email regex for validation.
		/// </summary>
		private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Validates the specified email address and returns the result of the validation.
		/// </summary>
		/// <param name="email">The email address to validate. Cannot be null, empty, or consist only of white-space characters.</param>
		/// <returns>A ValidationResult object that contains any validation errors found for the email address. If the email is valid,
		/// the result will contain no errors.</returns>
		public virtual ValidationResult ValidateEmail(string email)
		{
			ValidationResult validationResult = new ValidationResult();

			if (string.IsNullOrWhiteSpace(email))
				validationResult.AddError("Email", "Email is required.");
			else if (!EmailRegex.IsMatch(email))
				validationResult.AddError("Email", "Email is not valid.");
			else if (email.Length > 254)
				validationResult.AddError("Email", "Email cannot exceed 254 characters.");

			return validationResult;
		}


		/// <summary>
		/// Validates the specified email address and returns the result of the validation.
		/// </summary>
		/// <param name="email">The email address to validate. Cannot be null, empty, or consist only of white-space characters.</param>
		/// <returns>A ValidationResult object that contains any validation errors found for the email address. If the email is valid,
		/// the result will contain no errors.</returns>
		public virtual ValidationResult ValidatePassword(string password)
		{
			ValidationResult validationResult = new ValidationResult();

			if (string.IsNullOrWhiteSpace(password))
				validationResult.AddError("Password", "Password is required.");
			else if (password.Length < 8)
				validationResult.AddError("Password", "Password must be at least 8 characters.");
			else if (password.Length > 128)
				validationResult.AddError("Password", "Password cannot exceed 128 characters.");

			return validationResult;
		}
	}
}
