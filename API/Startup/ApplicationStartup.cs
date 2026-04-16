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
Logger.LogInformation("BuildHub API v{Version} starting... [{Environment}]", Assembly.GetExecutingAssembly().GetName().Version, 
	app.Environment.EnvironmentName);

app.Lifetime.ApplicationStarted.Register(() =>
{
    Logger.LogInformation("Listening on {URLS}" , string.Join(", ", app.Urls));
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
	Logger.LogInformation(ApplicationMessages.BUILD_HUB_SHUTING_DOWN);
}
finally
{
	Logger.Shutdown();
}
