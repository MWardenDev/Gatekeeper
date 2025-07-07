using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Configuration;

public static class LoggerConfigurator {
    
    public static void Configure(IConfiguration configuration) {
        var loggingOptions = configuration.GetSection("LoggingOptions");
        var useAppInsights = loggingOptions.GetValue<bool>("UseAppInsights");
        var appInsightsKey = loggingOptions.GetValue<string>("AppInsightsConnectionString");

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration) // Pull base config (sinks, enrichers, templates)
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service.name", configuration["Serilog:Properties:service.name"] ?? "UnknownService")
            .Enrich.WithProperty("service.version", configuration["Serilog:Properties:service.version"] ?? "0.0.0")
            .Enrich.WithProperty("service.instance.id", Environment.MachineName ?? configuration["Serilog:Properties:service.instance.id"])
            .Enrich.WithProperty("AspNetCoreEnvironment", configuration["Serilog:Properties:AspNetCoreEnvironment"] ?? "Unknown")
            .Enrich.WithProperty("trace_id", configuration["Serilog:Properties:trace_id"] ?? "00000000000000000000000000000000")
            .Enrich.WithProperty("span_id", configuration["Serilog:Properties:span_id"] ?? "0000000000000000");

        // Optional: conditional sinks if needed
        if(bool.TryParse(configuration["Serilog:UseAppInsights"], out useAppInsights) && useAppInsights) {
            var aiConnectionString = configuration["Serilog:AppInsightsConnectionString"];
            if(!string.IsNullOrWhiteSpace(aiConnectionString)) {
                loggerConfig.WriteTo.ApplicationInsights(
                    aiConnectionString,
                    TelemetryConverter.Traces,
                    restrictedToMinimumLevel: LogEventLevel.Information
                );
            }
        }

        Log.Logger = loggerConfig.CreateLogger();
    }
}
