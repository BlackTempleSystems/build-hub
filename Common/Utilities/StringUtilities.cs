namespace BuildHub.Common.Utilities;

public class StringUtilities
{
	/// <summary>
	/// Wraps a value in double quotes for safe use in command-line arguments.
	/// </summary>
	/// <param name="value">Value to be quoted.</param>
	/// <returns>The quoted value.</returns>
	public static string AddQuotes(string value) => $@"""{value}""";

	/// <summary>
	/// Surrounds the given value with single quotes
	/// </summary>
	/// <param name="value"></param>
	/// <returns>The value surrounded by single quotes</returns>
	public static string Stringify(object value) => $"'{value}'";
}
