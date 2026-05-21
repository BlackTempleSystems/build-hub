namespace BuildHub.Domain.Autehntication.Users.Models
{
	/// <summary>
	/// A User transport model
	/// </summary>
	public class UserModel
	{
		/// <summary>
		/// 
		/// </summary>
		public Guid UserGuid { get; set; }
		/// <summary>
		/// Gets or sets the user name associated with the account.
		/// </summary>
		public string UserName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		public string Email { get; set; } = string.Empty;
	}
}
