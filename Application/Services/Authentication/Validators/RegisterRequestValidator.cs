using FluentValidation;

namespace BuildHub.Application.Services.Authentication.Validators
{
	using Models;

	/// <summary>
	/// Register user request validator.
	/// </summary>
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public const int _EmailMaximumLenght = 128;

		public RegisterRequestValidator()
		{
			RuleFor(registerRequest => registerRequest.Email)
			.NotEmpty()
			.EmailAddress()
			.MaximumLength(_EmailMaximumLenght);

			RuleFor(registerRequest => registerRequest.UserName)
			.NotEmpty()
			.MaximumLength(32);

			RuleFor(registerRequest => registerRequest.FirstName)
			.NotEmpty()
			.MaximumLength(128);

			RuleFor(registerRequest => registerRequest.LastName)
			.NotEmpty()
			.MaximumLength(128);

			RuleFor(registerRequest => registerRequest.Password)
			.NotEmpty()
			.MaximumLength(128);

			RuleFor(registerRequest => registerRequest.Password).NotEmpty().WithMessage("Your password cannot be empty")
					.MinimumLength(8).WithMessage("Your password length must be at least 8.")
					.MaximumLength(16).WithMessage("Your password length must not exceed 16.")
					.Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
					.Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
					.Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
					.Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");

			RuleFor(registerRequest => registerRequest.ConfirmedPassword)
				.NotEmpty()
				.Equal(registerRequest => registerRequest.Password);
		}
	}
}
