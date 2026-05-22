namespace BuildHub.Application.Services.Authentication
{
	#region
	using BuildHub.Application.Messages;
	using BuildHub.Application.Services.Authentication.Jwt;
	using BuildHub.Application.Services.Authentication.RefreshToken;
	using BuildHub.Application.Services.Authentication.Validators;
	using BuildHub.Application.Services.CryptographicService;
	using BuildHub.Common.Logger;
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Queries;
	using BuildHub.DataEngine.Transactions;
	using BuildHub.Domain.Autehntication.RefreshTokens;
	using BuildHub.Domain.Autehntication.RefreshTokens.Entities;
	using BuildHub.Domain.Autehntication.UserCredentials;
	using BuildHub.Domain.Autehntication.UserCredentials.Entities;
	using BuildHub.Domain.Autehntication.Users;
	using BuildHub.Domain.Autehntication.Users.Entities;
	using BuildHub.Domain.Autehntication.Users.Models;
	using BuildHub.Domain.Results;
	using FluentValidation;
	using Microsoft.Extensions.DependencyInjection;
	using Models;

	#endregion
	/// <summary>
	/// Provides authentication-related services for managing userEntity sign-in, sign-out, and identity verification operations.
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

		/// <summary>
		/// Provides access to the service responsible for managing refresh tokens.
		/// </summary>
		private readonly IRefreshTokenService _refreshTokenService;

		public AuthenticationService(IServiceProvider serviceProvider)
		{
			this._cryptographicService = serviceProvider.GetService<ICryptographicService>()!;
			this._jwtService = serviceProvider.GetService<IJwtService>()!;
			this._refreshTokenService = serviceProvider.GetService<IRefreshTokenService>()!;
		}

		/// <summary>
		/// Authenticates a userEntity based on the provided login request data.
		/// </summary>
		/// <param name="loginRequestData">The login request information containing userEntity credentials to be authenticated. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous authentication operation.</returns>
		public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequestData)
		{
			LoginRequestValidator loginRequestValidator = new LoginRequestValidator();
			loginRequestValidator.ValidateAndThrow(loginRequestData);

			UsersTable usersTable = new UsersTable();
			var userEntity = usersTable.GetByCondition(user => user.Email, loginRequestData.Email).FirstOrDefault();

			if (userEntity is null || userEntity.Id <= 0)
			{
				Logger.LogError(ApplicationMessages.AccountWithEmailDoesntExists);

				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			UserCredentialsTable userCredentialsTable = new UserCredentialsTable();
			var userCredentialsEntity = userCredentialsTable.GetByCondition(userCredentials => userCredentials.UserId, userEntity.Id).FirstOrDefault();

			if (userCredentialsEntity is null)
			{
				Logger.LogError(ApplicationMessages.InvalidEmailOrUsernameOrPassword);

				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			if (!_cryptographicService.VerifyPassword(loginRequestData.Password, userCredentialsEntity.HashedPassword!))
			{
				Logger.LogError(ApplicationMessages.InvalidEmailOrUsernameOrPassword);

				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			var refreshTokenModel = this._refreshTokenService.GenerateAndSaveRefreshToken(userEntity);

			RefreshTokensTable refreshTokensTable = new RefreshTokensTable();
			var refreshTokenEntity = refreshTokensTable.GetByCondition(refreshToken => refreshToken.UserId, userEntity.Id).FirstOrDefault();
			if (refreshTokenEntity is null)
			{
				//TODO ERROR.
				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}
			refreshTokenEntity.RefreshToken = refreshTokenModel.RefreshToken;
			refreshTokenEntity.IsRevoked = false;
			refreshTokenEntity.ExpirationDate = refreshTokenModel.ExpirationDate;

			if (!refreshTokensTable.Update(refreshTokenEntity))
			{
				//TODO ERROR.
				return Result<LoginResponse>.Failure(ApplicationMessages.InvalidEmailOrUsernameOrPassword,
					ResultStatus.Unauthorized);
			}

			var jwtModel = this._jwtService.GenerateSecurityToken(userEntity);


			var loginResponse = new LoginResponse()
			{
				User = new UserModel()
				{
					UserGuid = userEntity.Guid,
					UserName = userEntity.UserName!,
					Email = userEntity.Email!
				},
				TokenPair = new TokenPairModel()
				{
					Jwt = jwtModel,
					RefreshToken = refreshTokenModel
				}
			};

			return Result<LoginResponse>.Success(loginResponse);
		}

		/// <summary>
		/// Registers a new userEntity with the specified registration details.
		/// </summary>
		/// <param name="registerRequest">An object containing the information required to register the userEntity. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous registration operation.</returns>
		public async Task<Result<RegisterUserResponse>> RegisterAsync(RegisterRequest registerRequest)
		{
			RegisterRequestValidator registerRequestValidator = new RegisterRequestValidator();
			registerRequestValidator.ValidateAndThrow(registerRequest);

			var queryBuilder = new QueryBuilder();
			queryBuilder.Where<UserEntity>(user => user.Email, registerRequest.Email);
			//.WhereOr(newUser, userEntity => userEntity.UserName!);

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
			if (newUserEntity is null)
			{
				Logger.LogError($"Failed to insert userEntity during registration.");
				return Result<RegisterUserResponse>.Failure("Could not register userEntity", ResultStatus.DatabaseFailure);
			}

			var salt = this._cryptographicService.GenerateSalt();
			var hashedPassword = this._cryptographicService.HashPassword(registerRequest.Password, salt);

			var userCredentials = new UserCredentialsEntity();
			userCredentials.UserId = newUserEntity.Id;
			userCredentials.HashedPassword = hashedPassword;

			UserCredentialsTable userCredentialsTable = new UserCredentialsTable();
			if (userCredentialsTable.Insert(userCredentials) is null)
			{
				Logger.LogError($"Failed to insert credentials for userEntity ID '{newUserEntity.Id}' during registration.");
				return Result<RegisterUserResponse>.Failure("Could not register userEntity", ResultStatus.DatabaseFailure);
			}

			var refreshTokenModel = this._refreshTokenService.GenerateAndSaveRefreshToken(newUserEntity);

			var jwtModel = this._jwtService.GenerateSecurityToken(newUserEntity);
			if (jwtModel is null)
			{
				//TODO ERROR.
				return Result<RegisterUserResponse>.Failure("Could not register userEntity", ResultStatus.Unauthorized);
			}

			if (!scopedTransaction.Commit())
			{
				Logger.LogError($"Transaction commit failed while registering userEntity '{newUserEntity.Email}'.");
				return Result<RegisterUserResponse>.Failure("Could not register userEntity", ResultStatus.DatabaseFailure);
			}

			var registerUserResponse = new RegisterUserResponse()
			{
				User = new UserModel()
				{
					UserGuid = newUserEntity.Guid,
					UserName = newUserEntity.UserName!,
					Email = newUserEntity.Email!
				},
				TokenPair = new TokenPairModel()
				{
					Jwt = jwtModel,
					RefreshToken = refreshTokenModel
				}
			};

			return Result<RegisterUserResponse>.Success(registerUserResponse);
		}

		/// <summary>
		/// Retrieves the userEntity from the database by guid.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public async Task<Result<UserModel>> GetUserByGuidAsync(Guid userGuid)
		{
			UsersTable usersTable = new UsersTable();
			UserEntity? user = usersTable.GetByGuid(userGuid);

			if (user is null)
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

		/// <summary>
		/// Gets a task that represents the asynchronous operation of refreshing the authentication token.
		/// </summary>
		/// <remarks>Await this task to ensure that the authentication token is refreshed before proceeding with
		/// operations that require a valid token.</remarks>
		public Task<Result<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest refreshTokenRequest)
		{

			UsersTable usersTable = new UsersTable();
			UserEntity? user = usersTable.GetByGuid(refreshTokenRequest.UserGuid);

			if (user is null)
			{
				//TODO ERROR

				//Logger.LogError("User doesn't exist");
				//return Result<UserModel>.Failure("User doesn't exist", ResultStatus.DatabaseFailure);
			}

			var refreshToken = _refreshTokenService.GetRefreshTokenForUser(user.Id);
			if(refreshToken is  null)
			{
				//TODO ERROR

			}

			//TODO VALIDATE REVODED EXPIRATION DATE ETC HASH.

			return Result<UserModel>.Success(userModel);
		}
	}
}
