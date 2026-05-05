namespace BuildHub.Application.Services.Authentication
{
	using BuildHub.Application.Messages;
	using BuildHub.Application.Services.Authentication.Jwt;
	using BuildHub.Application.Services.CryptographicService;
	using BuildHub.Common.Exceptions.Validators;
	using BuildHub.Common.Logger;
	using BuildHub.Common.Validators;
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Queries;
	using BuildHub.DataEngine.Transactions;
	using BuildHub.Domain.Results;
	using BuildHub.Domain.UserCredentials;
	using BuildHub.Domain.UserCredentials.Entities;
	using BuildHub.Domain.Users;
	using BuildHub.Domain.Users.Entities;
	using Models;

	/// <summary>
	/// Provides authentication-related services for managing user sign-in, sign-out, and identity verification operations.
	/// </summary>
	public sealed class AuthenticationService : IAuthenticationService 
	{
		/// <summary>
		/// Register user request validator.
		/// </summary>
		private readonly IValidator<RegisterUserRequest> _registerUserRequestValidator;
		/// <summary>
		/// Instance to the cryptographic service.
		/// </summary>
		private readonly ICryptographicService _cryptographicService;
		/// <summary>
		/// Instance to the jwt service.
		/// </summary>
		private readonly IJwtService _jwtService;

		public AuthenticationService(IValidator<RegisterUserRequest> registerUserRequestValidator
			, ICryptographicService cryptographicService
			, IJwtService jwtService)
		{
			this._registerUserRequestValidator = registerUserRequestValidator;
			this._cryptographicService = cryptographicService;
			this._jwtService = jwtService;
		}

		/// <summary>
		/// Authenticates a user based on the provided login request data.
		/// </summary>
		/// <param name="loginRequestData">The login request information containing user credentials to be authenticated. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous authentication operation.</returns>
		public async Task<Result<LoginResponse>> AuthenticateUser(Models.LoginRequest loginRequestData)
		{
			return Result<LoginResponse>.Success(null);
		}

		/// <summary>
		/// Registers a new user with the specified registration details.
		/// </summary>
		/// <param name="registerUserRequest">An object containing the information required to register the user. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous registration operation.</returns>
		public async Task<Result<RegisterUserResponse>> RegisterUser(RegisterUserRequest registerUserRequest)
		{
			var validationResult = _registerUserRequestValidator.Validate(registerUserRequest);
			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			UserEntity? newUser = new UserEntity();
			newUser.FirstName = registerUserRequest.FirstName;
			newUser.LastName = registerUserRequest.LastName;
			newUser.Email = registerUserRequest.Email;
			newUser.UserName = registerUserRequest.UserName;

			var queryBuilder = new QueryBuilder();
			queryBuilder.Where(newUser, user => user.Email); // OR USERNAME

			UsersTable usersTable = new UsersTable();
			var existingUser = usersTable.GetByCondition(queryBuilder).FirstOrDefault();

			if (existingUser is not null && existingUser.Id > 0)
			{
				Logger.LogError(ApplicationMessages.RegistrationFailedUserAlreadyExists, registerUserRequest.Email);

				return Result<RegisterUserResponse>.Failure(ApplicationMessages.RegistrationFailedUserAlreadyExists, 
					ResultStatus.Conflict, registerUserRequest.Email);
			}

			using ScopedTransaction scopedTransaction = new ScopedTransaction(DatabaseSource.Users);
			newUser = usersTable.Insert(newUser);
			if(newUser is null)
			{
				//TODO	ERROR.
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			var salt = this._cryptographicService.GenerateSalt();
			var hashedPassword = this._cryptographicService.HashPassword(registerUserRequest.Password, salt);

			var userCredentials = new UserCredentialsEntity();
			userCredentials.UserId = newUser!.Id;
			userCredentials.HashedPassword = hashedPassword;

			UserCredentialsTable userCredentialsTable = new UserCredentialsTable();
			if (userCredentialsTable.Insert(userCredentials) is null)
			{
				Logger.LogError($"Failed to insert credentials for user ID '{newUser.Id}' during registration.");
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			if (!scopedTransaction.Commit())
			{
				Logger.LogError($"Transaction commit failed while registering user '{newUser.Email}'.");
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			var jwtToken = this._jwtService.GenerateSecurityToken(newUser);

			var registerUserResponse = new RegisterUserResponse();
			registerUserResponse.UserName = newUser.UserName;
			registerUserResponse.Email = newUser.Email;
			registerUserResponse.AccessToken = jwtToken;

			return Result<RegisterUserResponse>.Success(registerUserResponse);
		}
	}
}
