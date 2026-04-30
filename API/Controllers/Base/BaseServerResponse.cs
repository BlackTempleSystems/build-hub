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
		public DateTime DateTimeStamp { get; set; }
		public bool IsSuccessful { get; set; }
		public ResponseData? Response { get; set; }

		public BaseServerResponse()
		{
			this.DateTimeStamp = DateTime.Now;
			this.IsSuccessful = false;
		}
	}
}
