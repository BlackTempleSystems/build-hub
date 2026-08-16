using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildHub.API.Observability;

public static class ObservabilityExtensions
{
	public static IServiceCollection AddBuildHubObservability(
		this IServiceCollection services,
		IConfiguration configuration,
		IHostEnvironment environment)
	{
		var serviceName = configuration["OTEL_SERVICE_NAME"] ?? environment.ApplicationName;
		var endpoint = new Uri(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317");
		var protocol = configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]?.Equals(
			"http/protobuf",
			StringComparison.OrdinalIgnoreCase) == true
				? OtlpExportProtocol.HttpProtobuf
				: OtlpExportProtocol.Grpc;

		services.AddOpenTelemetry()
			.ConfigureResource(resource =>
				resource.AddService(serviceName))
			.WithTracing(tracing => tracing
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddSqlClientInstrumentation()
				.AddOtlpExporter(options =>
				{
					options.Endpoint = endpoint;
					options.Protocol = protocol;
				}))
			.WithMetrics(metrics => metrics
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddRuntimeInstrumentation()
				.AddOtlpExporter(options =>
				{
					options.Endpoint = endpoint;
					options.Protocol = protocol;
				}));

		return services;
	}
}
