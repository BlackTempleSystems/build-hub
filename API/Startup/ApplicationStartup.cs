using BuildHub.API.Services.HealthCheck;
using BuildHub.API.Startup;
using BuildHub.Common.Logger;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHostedService<DatabaseBootstrapService>();
builder.Services.AddHealthChecks()
	.AddCheck<DatabaseHealthCheck>("DatabaseHealthCheck")
	.AddResourceUtilizationHealthCheck();

var app = builder.Build();

Logger.Initialize();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference("/api-docs", options =>

	{
		options.Title = "BuildHub API";
		options.Theme = ScalarTheme.Saturn;
		options.HideClientButton = true;
	});
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
	app.Run();
}
catch (OperationCanceledException)
{
	Logger.LogInformation("BuildHub server is shutting down gracefully. All services stopped.");
}
finally
{
	Logger.Shutdown();
}
