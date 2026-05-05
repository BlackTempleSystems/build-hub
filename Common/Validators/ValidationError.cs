namespace BuildHub.Common.Validators
{
	/// <summary>
	/// Represents a validation error for a specific field, including the field name and an associated error message.
	/// </summary>
	public sealed class ValidationError
	{
		/// <summary>
		/// Gets the value of the field represented as a string.
		/// </summary>
		public string Field { get; }

		/// <summary>
		/// Message of the error.
		/// </summary>
		public string Message { get; }

		public ValidationError(string field, string message)
		{
			Field = field;
			Message = message;
		}
	}
}
