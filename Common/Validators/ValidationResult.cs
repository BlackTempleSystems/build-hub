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
		/// Adds the errors from the specified <see cref="ValidationResult"/> to this instance.
		/// </summary>
		/// <param name="validationResult">The <see cref="ValidationResult"/> whose errors are to be added. Cannot be null.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="validationResult"/> is null.</exception>
		public void operator +=(ValidationResult validationResult)
		{
			if (validationResult is null)
				throw new ArgumentNullException();


			Errors.AddRange(validationResult.Errors);
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
