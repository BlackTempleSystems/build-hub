#region
using BuildHub.Common.Logger;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using BuildHub.Application.Services.Bootstrap;
using BuildHub.Application.Services.Authentication.Extensions;
using BuildHub.API.Middleware;
using BuildHub.Application.Messages;
#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddBuildHubAuthentication();
builder.Services.AddHostedService<DatabaseBootstrapService>();

var applicationFrontEndOriginPolicy = "ApplicationFrontendOriginPolicy";
var frontendApplicationUrl = builder.Configuration["FrontendApplicationSettings:BaseUrl"] 
	?? throw new InvalidOperationException("FrontendApplicationSettings:BaseUrl is not configured.");
builder.Services.AddCors(options =>
{
	options.AddPolicy(applicationFrontEndOriginPolicy,
		policy =>
		{
			policy.WithOrigins(frontendApplicationUrl)
				  .AllowCredentials()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

var app = builder.Build();
app.UseCors(applicationFrontEndOriginPolicy);

Logger.Initialize();
Logger.LogInformation("BuildHub API v{Version} starting... [{Environment}]", Assembly.GetExecutingAssembly().GetName().Version, 
	app.Environment.EnvironmentName);

app.Lifetime.ApplicationStarted.Register(() =>
{
    Logger.LogInformation("Listening on {URLS}" , string.Join(", ", app.Urls));
});

app.Lifetime.ApplicationStopping.Register(() =>
{
	Logger.LogInformation(ApplicationMessages.BuildHubShuttingDown);
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

app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
	app.Run();
}
catch (Exception)
{
	Logger.LogInformation(ApplicationMessages.BuildHubShuttingDown);
}
finally
{
	Logger.Shutdown();
}
