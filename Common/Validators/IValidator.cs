namespace BuildHub.Common.Validators
{
	/// <summary>
	/// Defines a contract for validating entities of a specified type.
	/// </summary>
	/// <typeparam name="T">The type to validate.</typeparam>
	public interface IValidator<TData>
	{
		/// <summary>
		/// Validates the specified data and returns the result of the validation.
		/// </summary>
		/// <param name="data">The data to validate. Cannot be null unless the implementation allows null values.</param>
		/// <returns>A ValidationResult that indicates whether the data is valid and contains any associated validation errors.</returns>
		
		ValidationResult Validate(TData data);
	}
}
