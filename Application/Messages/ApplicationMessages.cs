namespace BuildHub.Application.Messages
{
	/// <summary>
	/// 
	/// </summary>
	public static class ApplicationMessages
	{
		/// <summary>
		/// 
		/// </summary>
		public const string RegistrationFailedUserAlreadyExists = "Registration failed: user with email '{0}' already exists.";

		/// <summary>
		/// 
		/// </summary>
		public const string AccountWithEmailDoesntExists = "An account with this email doesn't exist";

		/// <summary>
		/// 
		/// </summary>
		public const string InvalidEmailOrUsernameOrPassword = "Invalid email/username or password";

		/// <summary>
		/// 
		/// </summary>
		public const string BuildHubShuttingDown = "BuildHub server is shutting down gracefully. All services stopped.";
	}
}
