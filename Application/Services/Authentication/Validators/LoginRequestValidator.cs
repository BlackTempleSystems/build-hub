using BuildHub.Application.Services.Authentication.Models;
using BuildHub.Application.Services.Authentication.Validators.Base;
using BuildHub.Common.Validators;

namespace BuildHub.Application.Services.Authentication.Validators
{
	/// <summary>
	/// Provides validation logic for login requests.
	/// </summary>
	/// <remarks>This class implements the <see cref="IValidator{T}"/> interface for <see cref="LoginRequest"/>
	/// objects. It is typically used to ensure that login request data meets required criteria before processing
	/// authentication.</remarks>
	public sealed class LoginRequestValidator : BaseAuthenticationValidator, IValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
		}

		/// <summary>
		/// Validates the specified login request data and returns the result of the validation.
		/// </summary>
		/// <param name="data">The login request data to validate. Cannot be null.</param>
		/// <returns>A ValidationResult object that indicates whether the login request data is valid and contains any validation
		/// errors.</returns>
		/// <exception cref="NotImplementedException">The method is not implemented.</exception>
		public ValidationResult Validate(LoginRequest loginRequest)
		{
			var validationResult = new ValidationResult();
			validationResult += ValidateEmail(loginRequest.Email);

			return validationResult;
		}
	}
}
