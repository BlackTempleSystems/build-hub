using BuildHub.API.Services.HealthCheck;
using BuildHub.API.Startup;
using BuildHub.Infrastructure.Services.Database.Startup;
using BuildHub.Infrastructure.Services.Startup.Logger;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddSingleton<IDatabaseStartupService, DatabaseStartupService>();
builder.Services.AddSingleton<ILoggerStartupService, LoggerStartupService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHostedService<LoggerHostedBootstrapService>();
builder.Services.AddHostedService<DatabaseHostedBootstrapService>();
builder.Services.AddHealthChecks()
	.AddCheck<DatabaseHealthCheck>("DatabaseHealthCheck")
	.AddResourceUtilizationHealthCheck();

var app = builder.Build();
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
app.MapHealthChecks("/health");

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
