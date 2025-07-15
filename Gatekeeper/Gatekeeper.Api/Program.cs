using Auth.Core;
using Gatekeeper.Api.Services;
using Logging.Core;
using Recovery.Core;
using Routing.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
LoggerConfigurator.Configure(builder.Configuration);

// Use Serilog for logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);  // ✅ Needed for DI into SerilogLoggerService


// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddScoped<IAuthenticator, FakeAuthenticator>();
builder.Services.AddScoped<ILoggerService, SerilogLoggerService>();
builder.Services.AddScoped<IRoutingStrategyService, RoutingStrategyService>();
builder.Services.AddScoped<IRouteExecutorService, RouteExecutorService>();
builder.Services.AddScoped<IRouteRepository, InMemoryRouteRepository>();
builder.Services.AddScoped<IMessageRetryService, MessageRetryService>();
builder.Services.AddHostedService<StartupRecoveryHostedService>();
builder.Services.AddScoped<IMessagePersistenceService>(provider => {
    var logger = provider.GetRequiredService<ILogger<FileMessagePersistenceService>>();
    var rootPath = Path.Combine(AppContext.BaseDirectory, "Recovery");
    return new FileMessagePersistenceService(rootPath, logger);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<Gatekeeper.Api.Middleware.TracingEnrichmentMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
