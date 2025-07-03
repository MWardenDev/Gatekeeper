using Auth.Core;
using Gatekeeper.Api.Services;
using Logging.Core;
using Routing.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddScoped<IAuthenticator, FakeAuthenticator>();
builder.Services.AddScoped<ILoggerService, ConsoleLogger>();
builder.Services.AddScoped<IMessageRouter, EchoRouter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
