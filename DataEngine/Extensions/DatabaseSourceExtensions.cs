#region
using BuildHub.DataEngine.DatabaseConnection;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
#endregion

namespace BuildHub.DataEngine.Extensions
{
	/// <summary>
	/// Provides extension methods for the DataSource type.
	/// </summary>
	/// <remarks>This class contains static methods that extend the functionality of the DataSource
	/// enumeration, allowing for additional operations without modifying the original type.</remarks>
	public static class DatabaseSourceExtensions
	{
		/// <summary>
		/// Determines whether the specified database source is marked as required.
		/// </summary>
		/// <remarks>This method uses reflection to check for the presence of the RequiredAttribute on the
		/// type of the provided database source.</remarks>
		/// <param name="databaseSource">The database source to evaluate. This parameter cannot be null.</param>
		/// <returns>true if the database source is marked with the RequiredAttribute; otherwise, false.</returns>
		public static bool IsRequired(this DatabaseSource databaseSource)
		{
			var enumData = typeof(DatabaseSource)
			.GetMember(databaseSource.ToString())
			.FirstOrDefault();

			return enumData?.GetCustomAttribute<RequiredAttribute>() is not null;
		}
	}
}
