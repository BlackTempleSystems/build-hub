using FluentValidation;
using FluentValidation.Results;

namespace BuildHub.Application.Services.Authentication.Validators
{
	using Models;

	/// <summary>
	/// Register user request validator.
	/// </summary>
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(registerRequest => registerRequest.Email)
			.NotEmpty()
			.EmailAddress()
			.MaximumLength(128);
		}
	}
}
