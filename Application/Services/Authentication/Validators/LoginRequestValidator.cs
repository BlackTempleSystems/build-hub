using FluentValidation;

namespace BuildHub.Application.Services.Authentication.Validators
{
	using Models;

	/// <summary>
	/// Provides validation logic for login requests.
	/// </summary>
	/// <remarks>This class implements the <see cref="IValidator{T}"/> interface for <see cref="LoginRequest"/>
	/// objects. It is typically used to ensure that login request data meets required criteria before processing
	/// authentication.</remarks>
	public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{

		}
	}
}
