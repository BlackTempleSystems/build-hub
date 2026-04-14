#region
using BuildHub.API.Messages;
using BuildHub.API.Services.HealthCheck;
using BuildHub.API.Startup;
using BuildHub.Common.Logger;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
#endregion

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
Logger.LogInformation(ApplicationMessages.APPLICATION_STARTING);
Logger.LogInformation(ApplicationMessages.APPLICATION_HEADER);
Logger.LogInformation(ApplicationMessages.APPLICATION_ENVIRONMENT, app.Environment.EnvironmentName);
Logger.LogInformation(ApplicationMessages.APPLICATION_VERSION, Assembly.GetExecutingAssembly().GetName().Version);
Logger.LogInformation(ApplicationMessages.APPLICATION_SEPARATOR);

app.Lifetime.ApplicationStarted.Register(() =>
{
    Logger.LogInformation(ApplicationMessages.APPLICATION_SEPARATOR);
    Logger.LogInformation(ApplicationMessages.APPLICATION_STARTED);
    Logger.LogInformation(ApplicationMessages.APPLICATION_LISTENING, string.Join(", ", app.Urls));
    Logger.LogInformation(ApplicationMessages.APPLICATION_SEPARATOR);
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    Logger.LogInformation(ApplicationMessages.APPLICATION_SEPARATOR);
    Logger.LogInformation(ApplicationMessages.APPLICATION_SHUTDOWN);
    Logger.LogInformation(ApplicationMessages.APPLICATION_SEPARATOR);
});

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
app.MapGet("/", () => Results.Redirect("api-docs")).ExcludeFromDescription();
app.MapHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
	app.Run();
}
catch(OperationCanceledException)
{
	Logger.LogInformation("BuildHub server is shutting down gracefully. All services stopped.");
}
finally
{
	Logger.Shutdown();
}
