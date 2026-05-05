namespace BuildHub.Common.Validators
{
	/// <summary>
	/// Represents the result of a validation operation.
	/// </summary>
	public sealed class ValidationResult
	{
		/// <summary>
		/// Gets a value indicating whether the current object is in a valid state with no errors.
		/// </summary>
		public bool IsValid => Errors.Count == 0;

		/// <summary>
		/// Gets the collection of validation errors associated with the current operation.
		/// </summary>
		public List<ValidationError> Errors { get; }

		public ValidationResult()
		{
			this.Errors = new List<ValidationError>();		
		}

		/// <summary>
		/// Adds a validation error for the specified field with the provided error message.
		/// </summary>
		/// <param name="field">The name of the field associated with the validation error. Cannot be null.</param>
		/// <param name="message">The error message describing the validation issue. Cannot be null.</param>
		public void AddError(string field, string message)
			=> Errors.Add(new ValidationError(field, message));
	}
}
