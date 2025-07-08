using Auth.Core;
using Gatekeeper.Api.Services;
using Logging.Core;
using Routing.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
LoggerConfigurator.Configure(builder.Configuration);

// Use Serilog for logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddScoped<IAuthenticator, FakeAuthenticator>();
builder.Services.AddScoped<ILoggerService, SerilogLogger>();
builder.Services.AddScoped<IRoutingStrategyService, RoutingStrategyService>();
builder.Services.AddScoped<IRouteExecutorService, RouteExecutorService>();
builder.Services.AddScoped<IRouteRepository, InMemoryRouteRepository>();


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
