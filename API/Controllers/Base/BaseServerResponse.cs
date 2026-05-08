namespace BuildHub.API.Controllers.Base
{
	/// <summary>
	/// Represents a standard server response that includes a timestamp, a success indicator, and an optional response
	/// payload.
	/// </summary>
	/// <remarks>This class is typically used to encapsulate the result of a server operation, providing both the
	/// operation outcome and any associated data. The generic parameter allows flexibility in specifying the type of data
	/// returned by the server.</remarks>
	/// <typeparam name="ResponseData">The type of the response payload returned by the server.</typeparam>
	public sealed class BaseServerResponse<ResponseData>
	{
		/// <summary>
		/// Gets or sets the date and time associated with this instance.
		/// </summary>
		public DateTime DateTimeStamp { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the operation completed successfully.
		/// </summary>
		public bool IsSuccessful { get; set; }

		/// <summary>
		/// Gets or sets the response data associated with the result.
		/// </summary>
		public ResponseData? ResultData { get; set; }

		public BaseServerResponse()
		{
			this.DateTimeStamp = DateTime.Now;
			this.IsSuccessful = false;
		}
	}
}
