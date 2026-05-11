using BuildHub.Common.Logger;
using FluentValidation;

namespace BuildHub.API.Middleware
{
	/// <summary>
	/// Global exception handler.
	/// </summary>
	public sealed class GlobalExceptionHandler
	{
		/// <summary>
		/// Holds the request delegate.
		/// </summary>
		private readonly RequestDelegate _requestDelegate;
		public GlobalExceptionHandler(RequestDelegate next) => _requestDelegate = next;

		/// <summary>
		/// Handles every request.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _requestDelegate(context);
			}
			catch (ValidationException exception)
			{
				Logger.LogError(exception, "A validation error occurred.");

				context.Response.StatusCode = 400;
				await context.Response.WriteAsJsonAsync(new
				{
					status = 400,
					errors = exception.Message
				});
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "An unexpected error occurred.");
				context.Response.StatusCode = 500;

				await context.Response.WriteAsJsonAsync(new
				{
					status = 500,
					message = "An unexpected error occurred"
				});
			}
		}
	}
}
