namespace BuildHub.Application.Services.Authentication
{
	using BuildHub.Application.Messages;
	using BuildHub.Application.Services.Authentication.Jwt;
	using BuildHub.Application.Services.CryptographicService;
	using BuildHub.Common.Logger;
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Queries;
	using BuildHub.DataEngine.Transactions;
	using BuildHub.Domain.Results;
	using BuildHub.Domain.UserCredentials;
	using BuildHub.Domain.UserCredentials.Entities;
	using BuildHub.Domain.Users;
	using Domain.Users.Models;
	using BuildHub.Domain.Users.Entities;
	using Models;
	using BuildHub.Application.Services.Authentication.Validators;
	using FluentValidation;

	/// <summary>
	/// Provides authentication-related services for managing user sign-in, sign-out, and identity verification operations.
	/// </summary>
	public sealed class AuthenticationService : IAuthenticationService 
	{
		/// <summary>
		/// Instance to the cryptographic service.
		/// </summary>
		private readonly ICryptographicService _cryptographicService;

		/// <summary>
		/// Instance to the jwt service.
		/// </summary>
		private readonly IJwtService _jwtService;

		public AuthenticationService(ICryptographicService cryptographicService
			, IJwtService jwtService)
		{
			this._cryptographicService = cryptographicService;
			this._jwtService = jwtService;
		}

		/// <summary>
		/// Authenticates a user based on the provided login request data.
		/// </summary>
		/// <param name="loginRequestData">The login request information containing user credentials to be authenticated. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous authentication operation.</returns>
		public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequestData)
		{
			LoginRequestValidator loginRequestValidator = new LoginRequestValidator();
			loginRequestValidator.ValidateAndThrow(loginRequestData);

			var queryBuilder = new QueryBuilder();
			queryBuilder.Where<UserEntity>(user => user.Email, loginRequestData.Email);

			UsersTable usersTable = new UsersTable();
			var user = usersTable.GetByCondition(queryBuilder).FirstOrDefault();

			if (user is null || user.Id <= 0)
			{
				Logger.LogError(ApplicationMessages.AccountWithEmailDoesntExists);

				return Result<LoginResponse>.Failure(ApplicationMessages.AccountWithEmailDoesntExists,
					ResultStatus.Unauthorized);
			}

			queryBuilder.Reset();
			queryBuilder.Where<UserCredentialsEntity>(userCredentials => userCredentials.UserId, user.Id);

			UserCredentialsTable userCredentialsTable = new UserCredentialsTable();
			var userCredentialsEntity = userCredentialsTable.GetByCondition(queryBuilder).FirstOrDefault();

			if(userCredentialsEntity is null)
			{
				Logger.LogError(ApplicationMessages.InvalidEmailOrUsernameOrPassword);

				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			if(!_cryptographicService.VerifyPassword(loginRequestData.Password, userCredentialsEntity.HashedPassword!))
			{
				Logger.LogError(ApplicationMessages.InvalidEmailOrUsernameOrPassword);

				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			var jwtModel = this._jwtService.GenerateSecurityToken(user);

			var loginResponse = new LoginResponse()
			{
				User = new UserModel()
				{
					UserGuid = user.Guid,
					UserName =  user.UserName!,
					Email = user.Email!
				},
				Jwt = jwtModel
			};

			return Result<LoginResponse>.Success(loginResponse);
		}

		/// <summary>
		/// Registers a new user with the specified registration details.
		/// </summary>
		/// <param name="registerRequest">An object containing the information required to register the user. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous registration operation.</returns>
		public async Task<Result<RegisterUserResponse>> RegisterAsync(RegisterRequest registerRequest)
		{
			RegisterRequestValidator registerRequestValidator = new RegisterRequestValidator();
			registerRequestValidator.ValidateAndThrow(registerRequest);

			var queryBuilder = new QueryBuilder();
			queryBuilder.Where<UserEntity>(user => user.Email, registerRequest.Email);
				//.WhereOr(newUser, user => user.UserName!);

			UsersTable usersTable = new UsersTable();
			var existingUserEntity = usersTable.GetByCondition(queryBuilder).FirstOrDefault();

			if (existingUserEntity is not null && existingUserEntity.Id > 0)
			{
				Logger.LogError(ApplicationMessages.RegistrationFailedUserAlreadyExists, registerRequest.Email);

				return Result<RegisterUserResponse>.Failure(ApplicationMessages.RegistrationFailedUserAlreadyExists, 
					ResultStatus.Conflict, registerRequest.Email);
			}

			var newUserEntity = new UserEntity();
			newUserEntity.UserName = registerRequest.UserName;
			newUserEntity.Email = registerRequest.Email;
			newUserEntity.FirstName = registerRequest.FirstName;
			newUserEntity.LastName = registerRequest.LastName;

			using ScopedTransaction scopedTransaction = new ScopedTransaction(DatabaseSource.Users);
			newUserEntity = usersTable.Insert(newUserEntity);
			if(newUserEntity is null)
			{
				Logger.LogError($"Failed to insert user during registration.");
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			var salt = this._cryptographicService.GenerateSalt();
			var hashedPassword = this._cryptographicService.HashPassword(registerRequest.Password, salt);

			var userCredentials = new UserCredentialsEntity();
			userCredentials.UserId = newUserEntity.Id;
			userCredentials.HashedPassword = hashedPassword;

			UserCredentialsTable userCredentialsTable = new UserCredentialsTable();
			if (userCredentialsTable.Insert(userCredentials) is null)
			{
				Logger.LogError($"Failed to insert credentials for user ID '{newUserEntity.Id}' during registration.");
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			if (!scopedTransaction.Commit())
			{
				Logger.LogError($"Transaction commit failed while registering user '{newUserEntity.Email}'.");
				return Result<RegisterUserResponse>.Failure("Could not register user", ResultStatus.DatabaseFailure);
			}

			var jwtModel = this._jwtService.GenerateSecurityToken(newUserEntity);

			var registerUserResponse = new RegisterUserResponse()
			{
				User = new UserModel()
				{
					UserGuid = newUserEntity.Guid,
					UserName = newUserEntity.UserName!,
					Email = newUserEntity.Email!
				},
				Jwt = jwtModel
			};

			return Result<RegisterUserResponse>.Success(registerUserResponse);
		}

		/// <summary>
		/// Retrieves the user from the database by guid.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public async Task<Result<UserModel>> GetUserByGuidAsync(Guid userGuid)
		{
			UsersTable usersTable = new UsersTable();
			UserEntity? user = usersTable.GetByGuid(userGuid);

			if(user is null)
			{
				Logger.LogError("User doesn't exist");
				return Result<UserModel>.Failure("User doesn't exist", ResultStatus.DatabaseFailure);
			}

			var userModel = new UserModel();
			userModel.UserGuid = userGuid;
			userModel.UserName = user.UserName!;
			userModel.Email = user.Email!;

			return Result<UserModel>.Success(userModel);
		}
	}
}
