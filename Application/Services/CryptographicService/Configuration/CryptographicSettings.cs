using BuildHub.Common.Configuration.Base;

namespace BuildHub.Application.Services.CryptographicService.Configuration
{
	/// <summary>
	/// Strongly-typed configuration settings for the <see cref="CryptographicService"/>.
	/// </summary>
	public sealed class CryptographicSettings : IConfigurationModel
	{
		public int SaltSize { get; init; }
		public int HashSize { get; init; }
		public int Iterations { get; init; }
		public int MemorySize { get; init; }
		public int DegreeOfParallelism { get; init; }
	}
}