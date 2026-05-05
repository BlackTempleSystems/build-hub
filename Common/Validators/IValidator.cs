namespace BuildHub.Common.Validators
{
	/// <summary>
	/// Defines a contract for validating entities of a specified type.
	/// </summary>
	/// <typeparam name="T">The type to validate.</typeparam>
	public interface IValidator<T>
	{
		ValidationResult Validate(T value);
	}
}
