#region
using BuildHub.API.Auth;
using BuildHub.API.Messages;
using BuildHub.API.Observability;
using BuildHub.Common.Logger;
using BuildHub.Infrastructure.Databases;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddBuildHubAuth(builder.Configuration);
builder.Services.AddBuildHubDatabases(builder.Configuration);
builder.Services.AddHealthChecks().AddResourceUtilizationHealthCheck();

builder.Services.AddBuildHubObservability(
	builder.Configuration,
	builder.Environment);

var app = builder.Build();

Logger.Initialize();
Logger.LogInformation("BuildHub API v{Version} starting... [{Environment}]", Assembly.GetExecutingAssembly().GetName().Version,
	app.Environment.EnvironmentName);

app.Lifetime.ApplicationStarted.Register(() =>
{
	Logger.LogInformation("Listening on {URLS}", string.Join(", ", app.Urls));
});

app.Lifetime.ApplicationStopping.Register(() =>
{
	Logger.LogInformation(ApplicationMessages.BUILD_HUB_SHUTING_DOWN);
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

app.MapHealthChecks("/health");

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
	Logger.LogInformation(ApplicationMessages.BUILD_HUB_SHUTING_DOWN);
}
finally
{
	Logger.Shutdown();
}
