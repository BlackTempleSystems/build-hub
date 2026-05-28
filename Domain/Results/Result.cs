namespace BuildHub.Domain.Results
{
	public enum ResultStatus
	{
		Ok,
		NotFound,
		Unauthorized,
		Conflict,
		ValidationFailure,
		DatabaseFailure
	}
}

namespace BuildHub.Domain.Results
{
	public sealed class Result<TData>
	{
		public TData? Data { get; init; }
		public bool IsSuccess { get; init; }
		public string? ErrorMessage { get; init; }
		public ResultStatus Status { get; init; }

		private Result()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Result<TData> Success(TData data) => new()
		{
			IsSuccess = true,
			Data = data,
			Status = ResultStatus.Ok
		};

		/// <summary>
		/// 
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <param name="status"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public static Result<TData> Failure(string errorMessage, ResultStatus status,
			params object[] arguments) => new()
			{
				IsSuccess = false,
				ErrorMessage = arguments.Length > 0 ? string.Format(errorMessage, arguments) : errorMessage,
				Status = status
			};
	}
}