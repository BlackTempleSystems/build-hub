namespace BuildHub.Application.Messages
{
	/// <summary>
	/// Provides application-level message templates used for user-facing responses and operational logging.
	/// </summary>
	public static class ApplicationMessages
	{
		/// <summary>
		/// User-facing message returned when sign-in cannot be completed.
		/// </summary>
		public const string AuthenticationFailed = "Authentication failed. Verify the supplied credentials and try again.";

		/// <summary>
		/// Operational log message used when account lookup does not produce an authenticated principal.
		/// </summary>
		public const string AuthenticationAccountLookupFailed = "Authentication attempt failed during account lookup.";

		/// <summary>
		/// Operational log message used when credential data cannot be resolved for an authentication attempt.
		/// </summary>
		public const string AuthenticationCredentialLookupFailed = "Authentication attempt failed during credential lookup.";

		/// <summary>
		/// Operational log message used when credential verification fails.
		/// </summary>
		public const string AuthenticationCredentialVerificationFailed = "Authentication attempt failed during credential verification.";

		/// <summary>
		/// User-facing message returned when registration data conflicts with an existing account.
		/// </summary>
		public const string RegistrationAccountConflict = "Registration could not be completed with the provided account details.";

		/// <summary>
		/// User-facing message returned when registration cannot be completed due to an internal failure.
		/// </summary>
		public const string RegistrationFailed = "Registration could not be completed at this time. Please try again later.";

		/// <summary>
		/// Operational log message used when user account creation fails.
		/// </summary>
		public const string RegistrationUserPersistenceFailed = "Registration failed while creating the user account record.";

		/// <summary>
		/// Operational log message used when credential creation fails.
		/// </summary>
		public const string RegistrationCredentialPersistenceFailed = "Registration failed while creating credential records.";

		/// <summary>
		/// Operational log message used when token generation fails during registration.
		/// </summary>
		public const string RegistrationTokenGenerationFailed = "Registration failed because token generation did not return a valid token.";

		/// <summary>
		/// Operational log message used when the registration transaction cannot be finalized.
		/// </summary>
		public const string RegistrationTransactionCommitFailed = "Registration failed while finalizing account creation.";

		/// <summary>
		/// User-facing message returned when a requested user profile cannot be resolved.
		/// </summary>
		public const string UserProfileNotFound = "The requested user profile could not be found.";

		/// <summary>
		/// Operational log message used when token refresh cannot resolve an eligible account.
		/// </summary>
		public const string RefreshTokenAccountLookupFailed = "Refresh token request failed during account lookup.";

		/// <summary>
		/// User-facing message returned when token refresh cannot be authorized.
		/// </summary>
		public const string RefreshTokenFailed = "Token refresh could not be completed. Sign in again to continue.";

		/// <summary>
		/// Operational log message used when refresh token rotation fails.
		/// </summary>
		public const string RefreshTokenRotationFailed = "Refresh token request failed during token rotation.";

		/// <summary>
		/// Operational log message used when a refresh token cannot be persisted.
		/// </summary>
		public const string RefreshTokenPersistenceFailed = "Refresh token operation failed while persisting token state.";

		/// <summary>
		/// Operational log message used when a refresh token cannot be found for rotation.
		/// </summary>
		public const string RefreshTokenLookupFailed = "Refresh token operation failed because no active token record was found.";

		/// <summary>
		/// Operational log message used when a revoked refresh token is submitted.
		/// </summary>
		public const string RefreshTokenRevoked = "Refresh token operation failed because the submitted token has been revoked.";

		/// <summary>
		/// Operational log message used when an expired refresh token is submitted.
		/// </summary>
		public const string RefreshTokenExpired = "Refresh token operation failed because the submitted token has expired.";

		/// <summary>
		/// Operational log message used when a submitted refresh token does not match the stored token hash.
		/// </summary>
		public const string RefreshTokenVerificationFailed = "Refresh token operation failed during token verification.";

		/// <summary>
		/// Operational log message used when refresh token configuration is missing.
		/// </summary>
		public const string RefreshTokenConfigurationMissing = "Refresh token operation failed because token configuration is missing.";

		/// <summary>
		/// Operational log message used when the application is stopping.
		/// </summary>
		public const string BuildHubShuttingDown = "BuildHub server is shutting down gracefully. All services stopped.";
	}
}
