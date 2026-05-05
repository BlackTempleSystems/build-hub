using BuildHub.Common.Validators;

namespace BuildHub.Common.Exceptions.Validators
{
	/// <summary>
	/// Validation exception (IValidator)
	/// </summary>
	public sealed class ValidationException : Exception
	{
		/// <summary>
		/// Readonly list of errors.
		/// </summary>
		private IReadOnlyList<ValidationError> Errors { get; }

		public ValidationException(IEnumerable<ValidationError> errors)
		{
			this.Errors = errors.ToList().AsReadOnly();
		}

		/// <summary>
		/// Message getter.
		/// </summary>
		public override string Message => string.Join(Environment.NewLine, Errors.Select(e => $"{e.Field}: {e.Message}"));
	}
}
