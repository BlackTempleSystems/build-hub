using BuildHub.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace BuildHub.API.Controllers.Base
{
	/// <summary>
	/// Base api controller, every controller should derive form this.
	/// The class provides basic api controller support and automatic url path computation
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class BaseApiController : ControllerBase
	{
		protected BaseApiController()
			: base()
		{
		}

		/// <summary>
		/// Creates an HTTP 200 OK response containing a standardized success payload with the specified response data.
		/// </summary>
		/// <remarks>Use this method to return successful API responses in a consistent format. The response structure
		/// includes a success indicator and the provided data.</remarks>
		/// <typeparam name="ResponseData">The type of the data to include in the response body.</typeparam>
		/// <param name="responseData">The data to include in the success response payload. Can be null if no data is required.</param>
		/// <returns>An IActionResult representing a 200 OK response with a standardized success payload containing the specified data.</returns>
		protected IActionResult ApiOk<ResponseData>(ResponseData responseData) =>
		  Ok(BuildResponse(true, responseData));

		/// <summary>
		/// Creates an unauthorized HTTP response with the specified response data.
		/// </summary>
		/// <typeparam name="ResponseData">The type of the response data to include in the unauthorized result.</typeparam>
		/// <param name="responseData">The response data to include in the body of the unauthorized response.</param>
		/// <returns>An <see cref="IActionResult"/> representing a 401 Unauthorized response containing the specified response data.</returns>
		protected IActionResult ApiUnauthorized<ResponseData>(ResponseData responseData) =>
			Unauthorized(BuildResponse(false, responseData));

		/// <summary>
		/// Creates a Bad Request (400) response with the specified response data wrapped in a standardized API response
		/// format.
		/// </summary>
		/// <typeparam name="ResponseData">The type of the response data to include in the response body.</typeparam>
		/// <param name="responseData">The response data to include in the body of the Bad Request response.</param>
		/// <returns>An IActionResult representing a Bad Request (400) response containing the specified response data.</returns>
		protected IActionResult ApiBadRequest<ResponseData>(ResponseData responseData) =>
			BadRequest(BuildResponse(false, responseData));

		/// <summary>
		/// Creates a 404 Not Found response with the specified response data.
		/// </summary>
		/// <typeparam name="ResponseData">The type of the response data to include in the Not Found result.</typeparam>
		/// <param name="responseData">The response data to include in the body of the Not Found result.</param>
		/// <returns>A 404 Not Found result containing the specified response data.</returns>
		protected IActionResult ApiNotFound<ResponseData>(ResponseData responseData) =>
			NotFound(BuildResponse(false, responseData));

		/// <summary>
		/// Creates a 409 status code response.
		/// </summary>
		/// <typeparam name="ResponseData"></typeparam>
		/// <param name="responseData"></param>
		/// <returns></returns>
		protected IActionResult ApiConflict<ResponseData>(ResponseData responseData) =>
			Conflict(BuildResponse(false, responseData));

		/// <summary>
		/// Creates API responses base on the result pattern.
		/// </summary>
		/// <typeparam name="ResponseData"></typeparam>
		/// <param name="responseData"></param>
		/// <returns></returns>
		protected IActionResult FromResult<T>(Result<T> result)
			=> result.Status switch
			{
				ResultStatus.Ok => ApiOk(result.Data),
				ResultStatus.Unauthorized => ApiUnauthorized(result.ErrorMessage),
				ResultStatus.NotFound => ApiNotFound(result.ErrorMessage),
				ResultStatus.Conflict => ApiConflict(result.ErrorMessage),
				_ => throw new Exception("Unhandled result status")
			};

		/// <summary>
		/// Creates a new BaseServerResponse<T> instance containing the specified response data and success status.
		/// </summary>
		/// <typeparam name="ResponseData">The type of the response data to include in the server response.</typeparam>
		/// <param name="isSuccessful">A value indicating whether the operation was successful. Set to <see langword="true"/> if the operation succeeded;
		/// otherwise, <see langword="false"/>.</param>
		/// <param name="responseData">The response data to include in the server response. Can be null if no data is available.</param>
		/// <returns>A BaseServerResponse<T> object containing the provided response data and success status, with the timestamp set to
		/// the current UTC time.</returns>
		private BaseServerResponse<ResponseData> BuildResponse<ResponseData>(bool isSuccessful, ResponseData responseData)
		{
			BaseServerResponse<ResponseData> baseServerResponse = new();
			baseServerResponse.DateTimeStamp = DateTime.UtcNow;
			baseServerResponse.IsSuccessful = isSuccessful;
			baseServerResponse.ResultData = responseData;

			return baseServerResponse;
		}
	}
}
